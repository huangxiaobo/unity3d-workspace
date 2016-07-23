Shader "Unlit/Mirror"
{
	Properties
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		[HideInspector] _ReflectionTex("", 2D) = "white" {}
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
				float4 refl : TEXCOORD1;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _ReflectionTex;
			
			v2f vert (appdata v)
			{
				v2f o;
				// 把顶点位置从模型空间转换到投影空间
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				// 计算uv坐标，即根据mesh上的uv坐标来计算真正的纹理上对应的坐标
				// 宏TRANSFORM_TEX的定义：#define TRANSFORM_TEX(tex,name) (tex.xy*name##_ST.xy+name##_ST.zw)
				// 故需要在shader中定义一个名为_YourTextureName_ST(也就是纹理名字加一个_ST后缀的变量)
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				// 获得一个投影点对应的屏幕坐标点
				o.refl = ComputeScreenPos(o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv);
				// 二维投影纹理查询
				fixed4 refl = tex2Dproj(_ReflectionTex, UNITY_PROJ_COORD(i.refl));
				return col * (1 - refl.a) + refl * refl.a;
			}
			ENDCG
		}
	}
}
