Shader "Hidden/ShaderLib/05/Smooth Lines" {
	SubShader {
		Pass {
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			#pragma target 3.0

			#include "UnityCG.cginc"

			float4 frag(v2f_img i) : COLOR {
				float Frequency = 16;
				float v = 3;

				float2 uv = -1.0 + 2.0 *(i.uv.xy / _ScreenParams.xy);


				
				float sawtooth 	= frac(i.uv.y * Frequency);
				float tri 	= abs(2.0 * sawtooth - 1.0);
				float dp 		= length(float2(ddx(i.uv.x), ddy(i.uv.y)));
				float edge 		= dp * Frequency * 2.0;
				float square 	= smoothstep(0.5 - edge, 0.5 + edge, tri);
	

				return  float4(square, square, square,1.0);
			}
			ENDCG
		}
	}
	FallBack "Diffuse"
}
