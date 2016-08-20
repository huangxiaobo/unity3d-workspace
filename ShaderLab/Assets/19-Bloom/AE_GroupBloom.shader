Shader "ShaderLib/19/AE_GroupBloom" {
	Properties {
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_BloomTex ("BloomTex (RGB)", 2D) = "white" {}
		_BloomSpeed("Bloom Speed",Range(0,10)) = 2.0
	}
	SubShader {
		ZTest Always ZWrite Off // 深度测试总是通过;不更新深度信息，意味着总是更新每个像素的颜色，但是不更新对应像素的深度缓存
		
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0

			#include "UnityCG.cginc"
			
			uniform sampler2D _MainTex;
			uniform sampler2D _BloomTex;
			float _BloomSpeed;

			v2f_img vert(appdata_img v) : POSITION {
				v2f_img o;
				o.pos=mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv=v.texcoord.xy;
				return o;
			}
			fixed4 frag(v2f_img i):COLOR
			{
				// 得到原来的屏幕颜色
				fixed4 mainColor = tex2D(_MainTex, i.uv);
				// 得到bloom纹理
				fixed4 bloomColor= tex2D(_BloomTex, i.uv);
				// 得到最终的颜色
				return bloomColor * abs(1 + sin(_Time.y * _BloomSpeed))  + mainColor;
			}
			ENDCG
		}
	} 
	FallBack off
}