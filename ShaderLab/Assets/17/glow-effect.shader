Shader "ShaderLib/17/GlowEffect"
{
	Properties {
		_MainTex ("Base (RGB)", 2D) = "" {}
	}
	 
	CGINCLUDE
		 
	#include "UnityCG.cginc"
	 
	struct v2f {
		float4 pos : POSITION;
		float2 uv : TEXCOORD0;
	};
		 
	sampler2D _MainTex;

	v2f vert( appdata_img v ) 
	{
		v2f o;
		o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
		o.uv =  v.texcoord.xy;
		return o;
	}
	 
	half4 frag(v2f i) : COLOR 
	{		  
		float4 final_col = float4(1, 0, 0, 1);
		float4 main_col = tex2D(_MainTex, i.uv.xy);
		if (main_col.a < 0.0001) {
			final_col = float4(1, 0, 0, 1);
		}
		else {
			// final_col = float4(main_col.xyz, 0.0);
			final_col = float4(0, 0, 0, 1.0);
		}
		return final_col;
	}
 
	ENDCG
	 
Subshader {
	 
 Pass {
	  ZTest Always Cull Off ZWrite Off
	  Fog { Mode off }      
 
	  CGPROGRAM
	  #pragma fragmentoption ARB_precision_hint_fastest
	  #pragma vertex vert
	  #pragma fragment frag
	  ENDCG
  }
}
 
Fallback off
} // shader