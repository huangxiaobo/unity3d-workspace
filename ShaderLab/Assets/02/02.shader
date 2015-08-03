Shader "ShaderLib/02" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_ScrollSppedX ("Scroll Speed X", Range(-10, 10)) = 2
		_ScrollSppedY ("Scroll Speed Y", Range(-10, 10)) = 2
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Lambert

		sampler2D _MainTex;
		fixed _ScrollSppedX;
		fixed _ScrollSppedY;

		struct Input {
			float2 uv_MainTex;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			fixed2 uv = IN.uv_MainTex;
			fixed _x = _ScrollSppedX * _Time;
			fixed _y = _ScrollSppedY * _Time;
			uv += fixed2(_x, _y);
			half4 c = tex2D (_MainTex, uv);
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
