Shader "Hidden/ShaderLib/15/AOTestBlur" 
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
			#pragma vertex vertAOBlur
			#pragma fragment fragAOBlur			
			#pragma target 3.0 
			#pragma glsl
			#pragma exclude_renderers flash
			
			#include "UnityCG.cginc" 
		
			#define EDGE_SHARPNESS     (1.0)
			#define SCALE               (2)
			
			#define R                   (4)
			static const float gaussian[5] = { 0.153170, 0.144893, 0.122649, 0.092902, 0.062970 };  // stddev = 2.0					
				
																																																																																																																																																																
			struct v2f_ao 
			{
				float4 pos : SV_POSITION;
	   			float2 uv : TEXCOORD0;			
			};
			
			float4 _MainTex_TexelSize;
			float2 _Axis;
														
			v2f_ao vertAOBlur (appdata_base vertInput)
			{
			    v2f_ao vertOutput;
			    vertOutput.pos = mul (UNITY_MATRIX_MVP, vertInput.vertex);
				vertOutput.uv = vertInput.texcoord.xy;
			    return vertOutput;
			}

			sampler2D _MainTex;
			
			/* Returns a number on (0, 1) */
			float unpackKey(float2 packedKey) 
			{
    			return packedKey.x * (256.0 / 257.0) + packedKey.y * (1.0 / 257.0);
			}			
			

			float4 fragAOBlur (v2f_ao fragInput) : COLOR
			{				
				float4 temp = tex2D(_MainTex, fragInput.uv);	
			
				float2 keyPacked = temp.gb;
				float key = unpackKey(keyPacked);
				
				float sum = temp.r;
				
			    // Base weight for depth falloff.  Increase this for more blurriness,
			    // decrease it for better edge discrimination
			    float BASE = gaussian[0];
			    float totalWeight = BASE;
			    sum *= totalWeight;	
			    
			    for (int r = -R; r <= R; ++r) 
			    {
			        // We already handled the zero case above.  This loop should be unrolled and the static branch optimized out,
			        // so the IF statement has no runtime cost
			        if (r != 0) 
			        {
						float2 uvOffset = (_Axis * r * SCALE) * _MainTex_TexelSize.xy;
			            temp = tex2Dlod(_MainTex, float4(fragInput.uv + uvOffset,0,0));
			            
		            	float tapKey = unpackKey(temp.gb);
			            float value  = temp.r;
			            
			            // spatial domain: offset gaussian tap
						//float weight = 0.3;
						//weight += gaussian[(int)abs((float)r)];
			            
						int index = r; if (index<0) index = -index;
						float weight = 0.3 + gaussian[index];

			            // range domain (the "bilateral" weight). As depth difference increases, decrease weight.
			            weight *= max(0.0, 1.0
			                - (EDGE_SHARPNESS * 2000.0) * abs(tapKey - key)
			                );
			
			            sum += value * weight;
			            totalWeight += weight;
			        }
			    }
			 
			    const float epsilon = 0.0001;
			    float result = sum / (totalWeight + epsilon);	
			    
			    return float4(result, keyPacked, 1);

		/*		float2 uvOffset = _Axis * _MainTex_TexelSize.xy * (2 * SCALE);
			    temp = tex2D(_MainTex, fragInput.uv + uvOffset);
				float tapKey = unpackKey(temp.gb);

				return float4(totalWeight, totalWeight, totalWeight,1);*/
			}			
			
			ENDCG
		}
	} 
	FallBack off
}
