Shader "Hidden/ShaderLib/15/TISSAO"
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
	sampler2D _CameraDepthTexture;
			 
	v2f vert( appdata_img v ) 
	{
		v2f o;
		o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
		o.uv =  v.texcoord.xy;
#if SHADER_API_D3D11 || SHADER_API_D3D9
		o.uv.y = 1 - o.uv.y;
#endif
		return o;
	}
	 
	half4 frag(v2f i) : COLOR 
	{
		float d = UNITY_SAMPLE_DEPTH(tex2D(_CameraDepthTexture, i.uv.xy));
		d = Linear01Depth(d);
					
		if(d>0.99999)
			return half4(1,1,1,1);
		else
			return half4(d,d,d,1);//EncodeFloatRGBA(d); 
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
} // shader