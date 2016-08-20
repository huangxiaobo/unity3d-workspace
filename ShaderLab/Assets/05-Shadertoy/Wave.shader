Shader "Hidden/ShaderLib/05/Wave"
{
	Properties
	{
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed3 COLOR1 = fixed3(0.0,0.0,0.3);  
				fixed3 COLOR2 = fixed3(0.5,0.0,0.0);  
				float BLOCK_WIDTH = 0.03;  

				float2 uv = i.uv;
				fixed3 final_color = fixed3(1.0, 1.0, 1.0);  
				fixed3 bg_color = fixed3(0.0, 0.0, 0.0); 
				fixed3 wave_color = fixed3(0.0, 0.0, 0.0);


				// 背景
				float c1 = fmod(uv.x, 2 * BLOCK_WIDTH);
				c1 = step(BLOCK_WIDTH, c1);
				float c2 = fmod(uv.y, 2 * BLOCK_WIDTH);
				c2 = step(BLOCK_WIDTH, c2);
				bg_color = lerp(uv.x * COLOR1, uv.y * COLOR2, c1 * c2);

				// 波形
				uv = 1 - 2 * uv;
				for (float i = 0.0; i < 7.0; ++i) {
					uv.y += (0.17 * sin(uv.x +  i / 3 + _Time.y));
					float wave_width = abs(1 / (150 * uv.y));
					wave_color += (wave_width * 1.9, wave_width, wave_width * 1.5);					
				}

				final_color = bg_color + wave_color;
				return fixed4(final_color, 1.0);
			}
			ENDCG
		}
	}
}
