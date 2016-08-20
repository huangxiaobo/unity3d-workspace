Shader "Hidden/ShaderLib/05/BasicTexShader" { // defines the name of the shader 
	Properties {
		_Color ("Diffuse Material Color", Color) = (1,1,1,1) 
		_SpecColor ("Specular Material Color", Color) = (1,1,1,1) 
		_Shininess ("Shininess", Float) = 10
	}
	SubShader {
		Pass {	
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
			};

			struct v2f {
				float4 pos 		: SV_POSITION;
				float4 scr_pos : TEXCOORD1;
			};

			v2f vert(appdata v) {
				v2f o;
				o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
				o.scr_pos = ComputeScreenPos(o.pos);
				return o;
			}

			fixed4 frag(v2f i) : COLOR0 {
				float2 wcoord = (i.scr_pos.xy/i.scr_pos.w);
				fixed4 color;

				if (fmod(20.0*wcoord.x, 2.0) < 1.0) {
					color = fixed4(wcoord.xy, 0.0, 1.0);
				} else {
					color = fixed4(0.3, 0.3, 0.3, 1.0);
				}
				return color;
			}

			ENDCG
		}
 
		Pass {	
			Tags { "LightMode" = "ForwardAdd" } 
			// pass for additional light sources
			Blend One One // additive blending 

			CGPROGRAM

			ENDCG
		}
	} 
	// The definition of a fallback shader should be commented out 
	// during development:
	// Fallback "Specular"
}