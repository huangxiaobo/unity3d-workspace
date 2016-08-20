Shader "Hidden/ShaderLib/20/DissolveShader"
{
	Properties
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_DissolveTex ("DissolveTex (RGB)", 2D) = "white" {}
		_Tile ("DissolveTile", Range(0.1, 1)) = 1

		_Amount ("DissAmount", Range(0, 1)) = 0.5
		_DissSize ("DissSize", Range(0, 1)) = 0.1

		_DissColor ("DissColor", Color) = (1, 0, 0, 1)
		_AddColor ("AddColor", Color) = (1, 1, 0, 1)
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
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _DissolveTex;
			float4 _DissolveTex_ST;
			half _Tile;
			half _Amount;
			half _DissSize;

			float4 _DissColor;
			float4 _AddColor;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				float ClipCol = tex2D(_DissolveTex, i.uv / _Tile);
				float _Amount = 0.5 * sin(_Time.y) + 0.5;
				float ClipAmount = ClipCol.r - _Amount;
				if (_Amount > 0) {
					if (ClipAmount < 0) {
						// return ClipCol;
						// return tex2D(_DissolveTex, i.uv);
						// return float4(ClipTex, ClipTex, ClipTex, 1);
						clip(-0.1);
					}
					else {
						if (ClipAmount < _DissSize) {
							float4 finalColor = lerp(_DissColor, _AddColor, ClipAmount / _DissSize) * 2;
							col = col * finalColor;
						}
						// col.r = col.g = 0;
					}
				}
				return col;
			}
			ENDCG
		}
	}
}
