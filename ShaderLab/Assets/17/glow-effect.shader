Shader "ShaderLib/17/GlowEffect"
{
	Properties {
		_MainTex ("Base (RGB)", 2D) = "" {}
		_GlowMap ("GlowMap Texture", 2D) = "white" {}
	}

	CGINCLUDE
	#include "UnityCG.cginc"
	struct v2f {
		float4 pos : POSITION;
		float2 uv : TEXCOORD0;
	};

	uniform sampler2D _MainTex;
	float4 _MainTex_ST;
	uniform sampler2D _GlowMap;
	float4 _GlowMap_ST;

	v2f vert( appdata_img v )
	{
		v2f o;
		o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
		o.uv = TRANSFORM_TEX(v.texcoord.xy, _MainTex);
		return o;
	}

	half4 frag(v2f i) : COLOR
	{
		float4 main_col = tex2D(_MainTex, i.uv.xy);
		float4 glow_col = tex2D(_GlowMap, fixed2(i.uv.x, 1 - i.uv.y));
		return glow_col * 2 + main_col;
	}
	ENDCG

	Subshader {

		Pass {
			ZTest Always Cull Off ZWrite Off

			CGPROGRAM
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma vertex vert
			#pragma fragment frag
			ENDCG
		}
	}

	Fallback off
}