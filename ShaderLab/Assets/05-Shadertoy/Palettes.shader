Shader "Hidden/ShaderLib/05/Palettes"
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

			float3 pal( in float t, in float3 a, in float3 b, in float3 c, in float3 d )
			{
				return a + b*cos( 6.28318*(c*t+d) );
			}	

			float4 frag(v2f_img i) : SV_Target {
				float2 p = i.uv.xy;
				
				// animate
				p.x += 0.1 * _Time;
				
				// compute colors
				float3 				col = pal( p.x, float3(0.5,0.5,0.5),float3(0.5,0.5,0.5),float3(1.0,1.0,1.0),float3(0.0,0.33,0.67) );
				if( p.y>(1.0/7.0) ) col = pal( p.x, float3(0.5,0.5,0.5),float3(0.5,0.5,0.5),float3(1.0,1.0,1.0),float3(0.0,0.10,0.20) );
				if( p.y>(2.0/7.0) ) col = pal( p.x, float3(0.5,0.5,0.5),float3(0.5,0.5,0.5),float3(1.0,1.0,1.0),float3(0.3,0.20,0.20) );
				if( p.y>(3.0/7.0) ) col = pal( p.x, float3(0.5,0.5,0.5),float3(0.5,0.5,0.5),float3(1.0,1.0,0.5),float3(0.8,0.90,0.30) );
				if( p.y>(4.0/7.0) ) col = pal( p.x, float3(0.5,0.5,0.5),float3(0.5,0.5,0.5),float3(1.0,0.7,0.4),float3(0.0,0.15,0.20) );
				if( p.y>(5.0/7.0) ) col = pal( p.x, float3(0.5,0.5,0.5),float3(0.5,0.5,0.5),float3(2.0,1.0,0.0),float3(0.5,0.20,0.25) );
				if( p.y>(6.0/7.0) ) col = pal( p.x, float3(0.8,0.5,0.4),float3(0.2,0.4,0.2),float3(2.0,1.0,1.0),float3(0.0,0.25,0.25) );
				
				// band
				float f = frac(p.y*7.0);
				// borders
				col *= smoothstep( 0.49, 0.47, abs(f-0.5) );
				// shadowing
				col *= 0.5 + 0.5*sqrt(4.0*f*(1.0-f));
				// dithering
				col += (1.0/255.0)*tex2D( _MainTex, i.uv.xy).xyz;

				return float4( col, 1.0 );

			}
			ENDCG
		}
	}
	FallBack "Diffuse"
}

