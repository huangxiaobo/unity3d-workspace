Shader "ShaderLib/RimLight" {
Properties {
	_MainColor ("Main Color", Color) = (1,1,1,1)
	_SpecColor ("Specular Color", Color) = (0.5, 0.5, 0.5, 1)
	_Shininess ("Shininess", Range (0.01, 1)) = 0.078125
	_MainTex ("Base (RGB) Gloss (A)", 2D) = "white" {}
	_NormalTex ("Normal (RGB) ", 2D) = "white" {}
	_RimColor ("Rim Color", Color) = (0.26,0.19,0.16,0.0)
    _RimPower ("Rim Power", Range(0.5,8.0)) = 2.0
}

SubShader {
	pass {
		Tags { "RenderType"="Opaque" }
		LOD 300
			
		CGPROGRAM
		#pragma vertex vert  
		#pragma fragment frag 

		#include "UnityCG.cginc"
		uniform float4 _LightColor0; 
			
		sampler2D _MainTex;
		sampler2D _NormalTex;
		fixed4 _MainColor;
		fixed4 _SpecColor;
		half _Shininess;
		float4 _RimColor;
		float _RimPower;
		
		struct vertexInput {
			float4 vertex : POSITION;
			float3 normal : NORMAL;
		};
		
		struct vertexOutput {
			float4 pos : SV_POSITION;
			float4 posWorld  : TEXCOORD0;
			float3 normalWorld : TEXCOORD01;
		};
		
		vertexOutput vert(vertexInput v) {
			vertexOutput output;
			float4x4 modelMatrix = _Object2World;
			float4x4 modelMatrixInverse = _World2Object; 
			
			output.pos = mul(UNITY_MATRIX_MVP, v.vertex);
			output.posWorld = mul(modelMatrix, v.vertex);
			output.normalWorld = normalize(mul(float4(v.normal, 0.0), modelMatrixInverse).rgb);
			
			return output;
		}
		
		float4 frag(vertexOutput o):COLOR { 
		
			return float4(1, 0, 0, 1);
		}
		ENDCG
	}
}

// Fallback "VertexLit"
}
