Shader "ShaderLib/05/Better Filtering"
{
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	SubShader
	{
		Pass
		{
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			#pragma target 3.0
			
			#include "UnityCG.cginc"

			sampler2D _MainTex;
	
			float4 frag(v2f_img i) : SV_Target {
				float2 p = i.uv;
				float2 uv = i.uv * 0.1;
				
				//---------------------------------------------	
				// regular texture map filtering
				//---------------------------------------------	
				float3 colA = tex2D( _MainTex, uv ).xyz;

				//---------------------------------------------	
				// my own filtering 
				//---------------------------------------------	
				float textureResolution = 64.0;
				uv = uv * textureResolution + 0.5;
				float2 iuv = floor( uv );
				float2 fuv = frac( uv );
				uv = iuv + fuv*fuv*(3.0 - 2.0 * fuv); // fuv*fuv*fuv*(fuv*(fuv*6.0-15.0)+10.0);;
				uv = (uv - 0.5) / textureResolution;
				float3 colB = tex2D( _MainTex, uv ).xyz;
				
				//---------------------------------------------	
				// mix between the two colors
				//---------------------------------------------	
				float f = sin(3.14 * p.x + 1.7 * _Time);
				float3 col = lerp( colA, colB, smoothstep( -0.1, 0.1, f ) );
				col *= smoothstep( 0.0, 0.01, abs(f-0.0) );
				
				return float4( col, 1.0 );	
			}
			ENDCG
		}
	}
	FallBack "Diffuse"
}
