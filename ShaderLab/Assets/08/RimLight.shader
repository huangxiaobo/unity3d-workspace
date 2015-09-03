Shader "ShaderLib/RimLight" {
Properties {
	_MainTex ("Base (RGBA) Gloss (A)", 2D) = "white" {}
	_SpecColor ("Specular Color", Color) = (0.5, 0.5, 0.5, 1)
	_Shininess ("Shininess", Range (0.01, 100)) = 0.078125
	_RimColor ("Rim Color", Color) = (0.26,0.19,0.16,0.0)
    _RimPower ("Rim Power", Range(0.5,8.0)) = 2.0
}

SubShader {
	pass {
		Tags { "LightMode"="ForwardBase" }
			
		CGPROGRAM
		#pragma vertex vert  
		#pragma fragment frag 

		#include "UnityCG.cginc"
		uniform float4 _LightColor0; 
			
		sampler2D _MainTex;
		fixed4 _SpecColor;
		half _Shininess;
		float4 _RimColor;
		float _RimPower;
		
		struct vertexInput {
			float4 vertex : POSITION;
			float3 normal : NORMAL;
			float4 texcoord : TEXCOORD0;
		};
		
		struct vertexOutput {
			float4 pos : SV_POSITION;
			float4 tex : TEXCOORD0;
			float4 posWorld  : TEXCOORD1;
			float3 normalDir : TEXCOORD2;
		};
		
		vertexOutput vert(vertexInput v) {
			vertexOutput output;
			float4x4 modelMatrix = _Object2World;
			float4x4 modelMatrixInverse = _World2Object; 
			
			output.pos = mul(UNITY_MATRIX_MVP, v.vertex);
			output.tex = v.texcoord;
			output.posWorld = mul(modelMatrix, v.vertex);
			output.normalDir = normalize(mul(float4(v.normal, 0.0), modelMatrixInverse).xyz);
			
			return output;
		}
		
		float4 frag(vertexOutput o):COLOR { 
			float3 normalDirection = normalize(o.normalDir);
			float3 viewDirection = normalize(_WorldSpaceCameraPos - o.posWorld.xyz);
			float3 lightDirection;
			float attenuation;
			
			float4 textureColor = tex2D(_MainTex, o.tex.xy);
			
			if (0.0 == _WorldSpaceLightPos0.w) {
				attenuation = 1.0;
				lightDirection = normalize(_WorldSpaceLightPos0.xyz);
			}
			else {
				float3 vertextToLightSource = _WorldSpaceLightPos0.xyz - o.posWorld.xyz;
				float distance = length(vertextToLightSource);
				attenuation = 1.0 / distance;
				lightDirection = normalize(vertextToLightSource);
			}
			
			float3 ambientLighting = textureColor.rgb * UNITY_LIGHTMODEL_AMBIENT.rgb;
			
			float3 diffuseReflection = textureColor.rgb * attenuation * _LightColor0.rgb * max(0.0, dot(normalDirection, lightDirection));
			
			float3 specularReflection;
			if (dot(normalDirection, viewDirection) < 0.0) {
				specularReflection = float3(0.0, 0.0, 0.0);
			}
			else {
				float3 reflectLight = reflect(-lightDirection, normalDirection);
				specularReflection = attenuation * _LightColor0.rgb * _SpecColor.rgb
					* pow( 	max(0.0, dot(reflectLight, viewDirection)),  _Shininess	);
				//specularReflection = float3(1.0, 1.0, 0.0);
				
			}
			
			half rim = 1.0 - saturate(dot(viewDirection, normalDirection));
			float3 rimReflection = _RimColor.rgb * pow(rim, _RimPower);

			return float4(diffuseReflection + specularReflection + rimReflection, 1.0);
		}
		ENDCG
	}

	pass {
		Tags { "LightMode"="ForwardAdd" }
		Blend One One
			
		CGPROGRAM
		#pragma vertex vert  
		#pragma fragment frag 

		#include "UnityCG.cginc"
		uniform float4 _LightColor0; 
			
		sampler2D _MainTex;
		fixed4 _SpecColor;
		half _Shininess;
		float4 _RimColor;
		float _RimPower;
		
		struct vertexInput {
			float4 vertex : POSITION;
			float3 normal : NORMAL;
			float4 texcoord : TEXCOORD0;
		};
		
		struct vertexOutput {
			float4 pos : SV_POSITION;
			float4 tex : TEXCOORD0;
			float4 posWorld  : TEXCOORD1;
			float3 normalDir : TEXCOORD2;
		};
		
		vertexOutput vert(vertexInput v) {
			vertexOutput output;
			float4x4 modelMatrix = _Object2World;
			float4x4 modelMatrixInverse = _World2Object; 
			
			output.pos = mul(UNITY_MATRIX_MVP, v.vertex);
			output.tex = v.texcoord;
			output.posWorld = mul(modelMatrix, v.vertex);
			output.normalDir = normalize(mul(float4(v.normal, 0.0), modelMatrixInverse).xyz);
			
			return output;
		}
		
		float4 frag(vertexOutput o):COLOR { 
			float3 normalDirection = normalize(o.normalDir);
			float3 viewDirection = normalize(_WorldSpaceCameraPos - o.posWorld.xyz);
			float3 lightDirection;
			float attenuation;
			
			float4 textureColor = tex2D(_MainTex, o.tex.xy);
			
			if (0.0 == _WorldSpaceLightPos0.w) {
				attenuation = 1.0;
				lightDirection = normalize(_WorldSpaceLightPos0.xyz);
			}
			else {
				float3 vertextToLightSource = _WorldSpaceLightPos0.xyz - o.posWorld.xyz;
				float distance = length(vertextToLightSource);
				attenuation = 1.0 / distance;
				lightDirection = normalize(vertextToLightSource);
			}
			
			float3 diffuseReflection = textureColor.rgb * attenuation * _LightColor0.rgb * max(0.0, dot(normalDirection, lightDirection));
			
			float3 specularReflection;
			if (dot(normalDirection, viewDirection) < 0.0) {
				specularReflection = float3(0.0, 0.0, 0.0);
			}
			else {
				float3 reflectLight = reflect(-lightDirection, normalDirection);
				specularReflection = attenuation * _LightColor0.rgb * _SpecColor.rgb
					* pow( 	max(0.0, dot(reflectLight, viewDirection)),  _Shininess	);
			}

			half rim = 1.0 - saturate(dot(viewDirection, normalDirection));
			float3 rimReflection = _RimColor.rgb * pow(rim, _RimPower);

			return float4(diffuseReflection + specularReflection + rimReflection, 1.0);
		}
		ENDCG
	}	

}

Fallback "VertexLit"
}
