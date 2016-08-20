Shader "Hidden/ShaderLib/05/CirclesMask" {
	Properties {
		_CirclesX ("Circles in X", Float) = 20
		_CirclesY ("Circles in Y", Float) = 10
		_Fade ("Fade", Range (0.1,1.0)) = 0.5
	}
	SubShader {
		Pass {

			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			uniform float _CirclesX;
			uniform float _CirclesY;
			uniform float _Fade;

			float4 vert(appdata_base v) : POSITION {
				return mul (UNITY_MATRIX_MVP, v.vertex);
			}

			float4 frag(float4 sp:VPOS) : COLOR {
				float2 wcoord = sp.xy/_ScreenParams.xy;
				float4 color;
				if (length(fmod(float2(_CirclesX*wcoord.x,_CirclesY*wcoord.y),2.0)-1.0)<_Fade) {
					color = float4(sp.xy/_ScreenParams.xy,0.0,1.0);
				} else {
					color = float4(0.3,0.3,0.3,1.0);
				} 
				return color;
			}
			ENDCG
		}
	}
	FallBack "Diffuse"
}
