﻿Shader "Unlit/glow"
{
	Properties
	{
		_MainTex ("Main Texture", 2D) = "white" {}
		_NormalTex ("Normal Texture", 2D) = "white" {}
		_GlowTex ("Glow Texture", 2D) = "white" {}
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
			sampler2D _GlowTex;
			float4 _MainTex_ST;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 final_col = fixed4(0, 0, 0, 0);

				fixed4 main_col = tex2D(_MainTex, i.uv);
				fixed4 glow_col = tex2D(_GlowTex, i.uv);
				if (glow_col.x  * glow_col.y  * glow_col.z > 0.0001) {
					final_col = fixed4(glow_col.xyz, 0.0);
				}
				else {
					final_col = fixed4(main_col.xyz, 1.0);
				}
				return final_col;
			}
			ENDCG
		}
	}
}
