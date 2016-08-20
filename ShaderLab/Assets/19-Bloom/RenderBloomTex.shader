Shader "Hidden/ShaderLib/19/RenderBloomTex" {//modified from "Unlit/Color"
	Properties {
		_MainTex ("Base (RGB)", 2D) = "" {}
		_Color ("Main Color", Color) = (1,1,1,1)
	}
	SubShader { //this subShader is same with "Unlit/Color" shader, except the RenderType change to "GroupBloom"
		// 使用GroupBloom的对象的渲染shader替换成这个,和"Unlit/Color"一样,只是RenderType改成了"GroupItem"
		Tags { "RenderType"="GroupBloom" }
		LOD 100
		Pass {  
				CGPROGRAM
					#pragma vertex vert
					#pragma fragment frag
					
					#include "UnityCG.cginc"

					struct appdata_t {
						float4 vertex : POSITION;
						float2 uv : TEXCOORD0;
					};
					struct v2f {
						float4 vertex : SV_POSITION;
						float2 uv : TEXCOORD0;
					};


					fixed4 _Color;
					sampler2D _MainTex;
					float4 _MainTex_ST;
					
					v2f vert (appdata_t v)
					{
						v2f o;
						o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
						o.uv = TRANSFORM_TEX(v.uv, _MainTex);
						return o;
					}
					
					fixed4 frag (v2f i) : COLOR
					{
						// 正常渲染
						fixed4 col = tex2D(_MainTex, i.uv);
						return col;
					}
				ENDCG
			}
	} // SubShader

	SubShader { //because this subShader renders pure black, so we need not support fog
		// 不是使用GroupBloom的对象的shader替换成这个,其实是渲染成纯黑色
		Tags { "RenderType"="Opaque" }
		LOD 100
		
		Pass {  
			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				
				#include "UnityCG.cginc"

				struct appdata_t {
					float4 vertex : POSITION;
				};

				struct v2f {
					float4 vertex : SV_POSITION;
				};

				fixed4 _Color;
				
				v2f vert (appdata_t v)
				{
					v2f o;
					o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
					return o;
				}
				
				fixed4 frag (v2f i) : COLOR
				{
					fixed4 col = float4(0, 0, 0, 1);
					return col;
				}
			ENDCG
		}
	} // SubShader
}
