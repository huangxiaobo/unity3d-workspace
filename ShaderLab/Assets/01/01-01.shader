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
			float4 pos : POSITION;
			float4 col : TEXCOORD0;
		};
		
		float rand(float3 co)
 		{
     		return frac(sin( dot(co.xyz ,float3(12.9898,78.233,45.5432) )) * 43758.5453);
 		}
 				
		v2f vert(appdata_full v) {
			v2f o;
			
			o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
			o.col = rand(o.pos);
			
			return o;
		}
		
		float4 frag (v2f v):Color {
			// half4 c = tex2D (_MainTex, IN.uv_MainTex);
			// o.Albedo = c.rgb;
			//o.Alpha = c.a;
			return v.col;
		}
		
		
		ENDCG
	} 
	}
	FallBack "Diffuse"
}
