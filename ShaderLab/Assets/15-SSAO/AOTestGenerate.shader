Shader "ShaderLib/15/AOTestGenerate" 
{
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	SubShader 
	{
		Pass
		{
			ZTest Always Cull Off ZWrite Off
			Fog { Mode off }
			
			CGPROGRAM
			#pragma vertex vertAO
			#pragma fragment fragAO			
			#pragma target 3.0 
			#pragma glsl
			#pragma exclude_renderers flash
			
			#include "UnityCG.cginc" 
			
			#define FAR_PLANE_Z (300.0)			
			#define NUM_SAMPLES (11)
			#define NUM_SPIRAL_TURNS (7)

			struct v2f_ao 
			{
				float4 pos : SV_POSITION;
	   			float2 uvDepth : TEXCOORD0;
			};
			
			float4 _CameraDepthTexture_TexelSize;
			float4 _CameraDepthTexture_ST;
						
			v2f_ao vertAO (appdata_base v)
			{
			    v2f_ao o;
			    o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
				o.uvDepth = v.texcoord.xy;	    
			    return o;
			}

			float3 _ClipInfo;
			float4 _ProjInfo;

			float _NoiseScale;
			float _ProjScale;
			float _Radius;
			float _Radius2;
			float _Bias;
			float _IntensityDivR6;

			sampler2D _MainTex;
			sampler2D _CameraDepthTexture;
			sampler2D _NoiseTexture;
			sampler2D _RandTexture;

			//From near to far clip value
			float LinearDepthValue(float depth)
			{	
				return _ClipInfo[0] / ((_ClipInfo[1] * depth) + _ClipInfo[2]);			
			}
		
			/* 
				Reconstruct camera-space P.xyz from screen-space S = (x, y) in
			    pixels and camera-space z < 0.  Assumes that the upper-left pixel center
			    is at (0.5, 0.5) [but that need not be the location at which the sample tap 
			    was placed!]

			    Costs 3 MADD.  Error is on the order of 10^3 at the far plane, partly due to z precision.
		  	*/
			float3 reconstructCameraSpacePosition(float2 uv01, float linearDepth) 
			{
				//uv01.xy * _CameraDepthTexture_TexelSize.zw => screen space coordinate

				float2 screenSpacePosition = uv01.xy * _CameraDepthTexture_TexelSize.zw;
				#ifdef UNITY_HALF_TEXEL_OFFSET
				screenSpacePosition += float2(0.5, 0.5);
				#endif
			
			    return float3((screenSpacePosition * _ProjInfo.xy + _ProjInfo.zw) * linearDepth, linearDepth);
			}

			/** Reconstructs screen-space unit normal from screen-space position */
			float3 reconstructCameraSpaceNormal(float3 cameraSpacePosition) 
			{
    			return normalize(cross(ddy(cameraSpacePosition), ddx(cameraSpacePosition)));
			}

			/** Used for packing Z into the GB channels */
			float LinearDepthToKey(float linearDepth) 
			{
				return saturate(linearDepth * (1.0 / FAR_PLANE_Z));
			}


			/** Used for packing Z into the GB channels */
			void packKey(float keyToPack, out float2 packedValue) 
			{
				// Round to the nearest 1/256.0
				float temp = floor(keyToPack * 256.0);

				// Integer part
				packedValue.x = temp * (1.0 / 256.0);

				// Fractional part
				packedValue.y = keyToPack * 256.0 - temp;
			}

			float unpackKey(float2 packedKey) 
			{
    			return packedKey.x * (256.0 / 257.0) + packedKey.y * (1.0 / 257.0);
			}	

			float2 tapLocation(int sampleNumber, float spinAngle, out float ssR)
			{
				// Radius relative to ssR
				float alpha = float(sampleNumber + 0.5) * (1.0 / NUM_SAMPLES);
				float angle = alpha * (NUM_SPIRAL_TURNS * 6.28) + spinAngle;

				ssR = alpha;
				return float2(cos(angle), sin(angle));
			}

			float3 getOffsetPosition(float2 uvDepth, float2 unitOffset, float screenSpaceRadius) 
			{
				float2 uvDepthOffset = uvDepth + (screenSpaceRadius * unitOffset * _CameraDepthTexture_TexelSize.xy); 
    
				float3 cameraSpacePosition;
				cameraSpacePosition.z = LinearDepthValue(UNITY_SAMPLE_DEPTH(tex2D(_CameraDepthTexture, uvDepthOffset)));
				cameraSpacePosition = reconstructCameraSpacePosition(uvDepthOffset, cameraSpacePosition.z); 

				return cameraSpacePosition;
			}

			float sampleAO(float2 uvDepth, float3 cameraSpacePosition, float3 cameraSpaceNormal, float diskRadiusScreenSpace, int tapIndex, float randomPatternRotationAngle)
			{
				// Offset on the unit disk, spun for this pixel
				float screenSpaceRadius;
				float2 unitOffset = tapLocation(tapIndex, randomPatternRotationAngle, screenSpaceRadius);
				screenSpaceRadius *= diskRadiusScreenSpace;
        
				// The occluding point in camera space
				float3 cameraSpacePositionOccluding = getOffsetPosition(uvDepth, unitOffset, screenSpaceRadius);

				float3 positionVectorDelta = cameraSpacePositionOccluding - cameraSpacePosition;

				float DeltaDelta = dot(positionVectorDelta, positionVectorDelta);
				float DeltaNormal = dot(positionVectorDelta, cameraSpaceNormal);

				const float epsilon = 0.01;
    
				// A: From the HPG12 paper
				// Note large epsilon to avoid overdarkening within cracks
				// return float(vv < radius2) * max((vn - bias) / (epsilon + vv), 0.0) * radius2 * 0.6;

				// B: Smoother transition to zero (lowers contrast, smoothing out corners). [Recommended]
				float f = max(_Radius2 - DeltaDelta, 0.0); 
				return f * f * f * max((DeltaNormal - _Bias) / (epsilon + DeltaDelta), 0.0);

				// C: Medium contrast (which looks better at high radii), no division.  Note that the 
				// contribution still falls off with radius^2, but we've adjusted the rate in a way that is
				// more computationally efficient and happens to be aesthetically pleasing.
				// return 4.0 * max(1.0 - vv * invRadius2, 0.0) * max(vn - bias, 0.0);

				// D: Low contrast, no division operation
				// return 2.0 * float(vv < radius * radius) * max(vn - bias, 0.0);
			}

			float4 fragAO (v2f_ao fragInput) : COLOR
			{
				float linearDepth = LinearDepthValue(UNITY_SAMPLE_DEPTH(tex2D(_CameraDepthTexture, fragInput.uvDepth)));
							  	
			  	float3 cameraSpacePosition = reconstructCameraSpacePosition(fragInput.uvDepth, linearDepth);
			  
			  	float2 bilateralKey;
			  	packKey(LinearDepthToKey(cameraSpacePosition.z), bilateralKey);
			  			  	
				int2 uvDepthScreenSpace = fragInput.uvDepth * _CameraDepthTexture_TexelSize.zw;					  	
				float randomPatternRotationAngle = (3 * _NoiseScale * DecodeFloatRGBA(tex2D(_NoiseTexture, fragInput.uvDepth)) + uvDepthScreenSpace.x * uvDepthScreenSpace.y) * 10;
				//float randomPatternRotationAngle = tex2D(_RandTexture, fragInput.uvDepth*12.0).x * 1000.0;

			  	float3 cameraSpaceNormal = reconstructCameraSpaceNormal(cameraSpacePosition);
			    		  			  		  	
				float diskRadiusScreenSpace = -_ProjScale * _Radius / cameraSpacePosition.z;
    
				float sum = 0.0;
				for (int i = 0; i < NUM_SAMPLES; ++i) 
				{
					sum += sampleAO(fragInput.uvDepth, cameraSpacePosition, cameraSpaceNormal, diskRadiusScreenSpace, i, randomPatternRotationAngle);
				}

				float A = max(0.0, 1.0 - sum * _IntensityDivR6 * (5.0 / NUM_SAMPLES));				

				float depthDifference = max( abs(ddx(cameraSpacePosition.z)), abs(ddy(cameraSpacePosition.z)));
				if( cameraSpacePosition.z > 150.0 || depthDifference > _Radius )
				{
					A = 1.0;
				}


				return float4 (A, bilateralKey , 1);
			}			
			
			ENDCG
		}
	} 
	FallBack off
}
