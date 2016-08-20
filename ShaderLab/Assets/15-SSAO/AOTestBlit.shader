Shader "ShaderLib/15/AOTestBlit" 
{
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	SubShader 
	{
		Pass
		{
			ZTest Always Cull Off ZWrite Off
			Fog { Mode off }
			
			CGPROGRAM
			#pragma vertex vertAOBlit
			#pragma fragment fragAOBlit			
			#pragma target 3.0 
			#pragma glsl
			#pragma exclude_renderers flash
			
			#include "UnityCG.cginc" 
																																																																																																																																																												
			struct v2f_ao 
			{
				float4 pos : SV_POSITION;
				float2 uvAO : TEXCOORD0; 
	   			float2 uvScreen : TEXCOORD1;			
			};
			
			float4 _MainTex_TexelSize;
													
			v2f_ao vertAOBlit (appdata_base v)
			{
			    v2f_ao o;
			    o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
				o.uvAO = v.texcoord.xy;
				o.uvScreen = v.texcoord.xy;
				#if UNITY_UV_STARTS_AT_TOP
				if (_MainTex_TexelSize.y < 0)
					o.uvScreen.y = 1-o.uvScreen.y;
				#endif			    	    
			    return o;
			}

			sampler2D _MainTex;
			sampler2D _AOTexture;
			sampler2D _LightBuffer;

			float4 fragAOBlit (v2f_ao i) : COLOR
			{			
				float4 color = tex2D(_MainTex, i.uvScreen);    
			    float ao = tex2D(_AOTexture, i.uvAO).r;
		
				return color * float4(ao, ao, ao, 1);
			}			 
			
			ENDCG
		}
	} 
	FallBack off
}
