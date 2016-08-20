Shader "Hidden/ShaderLib/01/Vertex" {
	Properties {
		_MainColor ("Main Color", Color) = (0, 0, 0)
	}
	SubShader {
		Pass {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		
		#include "UnityCG.cginc"
		#pragma vertex vert
		#pragma fragment frag

		float4 _MainColor;

		struct v2f {
			float4 pos : POSITION;
			float2 uv : TEXCOORD0;
		};
						
		v2f vert(appdata_base v) {
			v2f o;
			
			o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
			o.uv = v.texcoord.xy;
			
			return o;
		}
		
		float4 frag (v2f v):COLOR {
			return _MainColor * sin(fmod(_Time.y, 1.0) * 3.14);
		}
		
		
		ENDCG
	} 
	}
	FallBack "Diffuse"
}
