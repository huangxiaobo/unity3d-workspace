Shader "Hidden/ShaderLib/22/Occlusion"
{
	Properties
	{
		_MainTex("Base (RGB)", 2D) = "white" {}
		_DepthMap("DepthMap", 2D) = "white" {}
		_OcclusionMap("OcclusionMap (RGB)", 2D) = "white" {}
		_Intensity("Intensity", Float) = 0.0
		_Tiling("Tiling", Vector) = (1.0, 1.0, 0.0, 0.0)
	}

	SubShader
	{
		Pass
		{
			ZTest Always
			ZWrite Off
			Cull Off
			Fog{ Mode Off }

			CGPROGRAM
			#include "UnityCG.cginc"
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest

			uniform sampler2D _MainTex;
			uniform sampler2D _DepthMap;
			uniform sampler2D _OcclusionMap;
			uniform sampler2D _CameraDepthNormalsTexture;
			fixed4 _MainTex_TexelSize;
			fixed _Intensity;
			fixed _Power;
			fixed4 _Tiling;

			struct a2v
			{
				fixed4 vertex : POSITION;
				fixed2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				fixed4 vertex : SV_POSITION;
				fixed2 uv : TEXCOORD0;
			};

			v2f vert(a2v v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.texcoord.xy;

				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				fixed4 c = tex2D(_MainTex, i.uv);
				float2 uv = float2(i.uv.x, 1 - i.uv.y);

				float depth;
				float3 normal;
				// DepthMap为单独绘制角色而来的深度法线图
				DecodeDepthNormal(tex2D(_DepthMap, uv), depth, normal);

				// CameraDepthNormalsTexture 是摄像机的深度纹理图
				fixed4 cameraDepthMap = tex2D(_CameraDepthNormalsTexture, uv);
				fixed cameraDepth = DecodeFloatRG(cameraDepthMap.zw);

				fixed4 o = c;
				// 将单独绘制的深度和摄像机的深度相比较，就可以知道角色哪一部分被遮挡了
				if (depth > 0 && cameraDepth < depth)
				{
					fixed2 uv = i.uv * _Tiling.xy + _Tiling.zw;
					fixed3 color = tex2D(_OcclusionMap, uv);
					fixed nf = saturate(dot(normal, fixed3(0, 0, 1)));
					nf = pow(nf, _Intensity);
					o.rgb = lerp(color, c.rgb, nf);
				}

				return o;
			}

			ENDCG
		}
	}

	Fallback off
}