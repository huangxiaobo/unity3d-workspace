Shader "Hidden/ShaderLib/25/Water"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float3 normal : NORMAL;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				half3 worldNormal: TEXCOORD1;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;

			static float pi = 3.14159f;
			// 波浪数量
			static int _WaveNum = 4;
			// 波幅
			static float _WaveAmp[4] = {0.02f, 0.04f, 0.06f, 0.08f};
			// 波长
			static float _WaveLen[4] = {1.00f, 1.20f, 1.30f, 1.40f};
			// 波速
			static float _WaveSpd[4] = {5.0f, 10.0f, 15.0f, 20.0f};
			// 方向
			static float2 _WaveDir[4] = {{0.2, 0.8}, {0.4, 0.6}, {0.6, 0.4}, {0.8, 0.2}};

			float wave(int i, float x, float y)
			{
				float frequency = 2 * pi / _WaveLen[i];
				float theta = dot(_WaveDir[i], float2(x, y));
				float phase = _WaveSpd[i] * 2 * pi / _WaveLen[i];

				return _WaveAmp[i] * sin(theta * frequency + _Time * phase);
			}

			float waveHeight(float x, float y)
			{
				float height = 0.0f;
				for (int i = 0; i < _WaveNum; ++i)
				{
					height += wave(i, x, y);
				}	
				return height;
			}

			float dWavedx(int i, float x, float y) {
				float frequency = 2*pi/_WaveLen[i];
				float phase = _WaveSpd[i] * frequency;
				float theta = dot(_WaveDir[i], float2(x, y));
				float A = _WaveAmp[i] * _WaveDir[i].x * frequency;
				return A * cos(theta * frequency + _Time * phase);
			}

			float dWavedy(int i, float x, float y) {
				float frequency = 2*pi/_WaveLen[i];
				float phase = _WaveSpd[i] * frequency;
				float theta = dot(_WaveDir[i], float2(x, y));
				float A = _WaveAmp[i] * _WaveDir[i].y * frequency;
				return A * cos(theta * frequency + _Time * phase);
			}

			float3 waveNormal(float x, float y) {
				float dx = 0.0;
				float dy = 0.0;
				for (int i = 0; i < _WaveNum; ++i) {
					dx += dWavedx(i, x, y);
					dy += dWavedy(i, x, y);
				}
				float3 n = float3(-dx, -dy, 1.0);
				return normalize(n);
			}
			
			v2f vert (appdata v)
			{
				v2f o;
				
				v.vertex.y = waveHeight(v.vertex.x, v.vertex.z);
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);

				o.uv = TRANSFORM_TEX(v.uv, _MainTex);

				float3 normal = waveNormal(v.vertex.x, v.vertex.z);
				o.worldNormal = UnityObjectToWorldNormal(normal);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);
				col.xyz *= i.worldNormal * 0.5 + 0.5;
				return col;
			}
			ENDCG
		}
	}
}
