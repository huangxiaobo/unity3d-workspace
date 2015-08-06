Shader "ShaderLib/01-01" {
	Properties {
		_MainColor ("Main Color", Color) = (0, 0, 0)
		_SpecularColor ("Speculor Color", Color) = (1, 1, 1)
	}
	SubShader {
		Pass {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		
		#include "UnityCG.cginc"
		#pragma vertex vert
		#pragma fragment frag

		sampler2D _MainTex;

		struct v2f {
			float4 pos : SV_POSITION;
			float3 eye_direction : TEXCOORD1;
		};
						
		v2f vert(appdata_base v) {
			v2f o;
			
			o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
			
			o.eye_direction = normalize(_WorldSpaceCameraPos.xyz - mul(v.vertex, _Object2World).xyz);
			return o;
		}
		
		float4 frag (v2f v):COLOR {
			return float4(abs(v.eye_direction.xyz), 1);
		}
		
		
		ENDCG
	} 
	}
	FallBack "Diffuse"
}
