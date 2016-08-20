Shader "Hidden/ShaderLib/23/Lambert"
{
	Properties
	{	
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100


		CGPROGRAM
		#pragma surface surf Lamb  

		sampler2D _MainTex;
		
		inline fixed4 LightingLamb (SurfaceOutput s, fixed3 lightDir, fixed atten)
		{
			float diff = dot(s.Normal, lightDir);
			fixed4 c;
			c.rgb = (s.Albedo * _LightColor0.rgb * diff);
			c.a = 1.0;
			return c;
		}


		struct Input {
			float2 uv_MainTex;
		};

		void surf(Input IN, inout SurfaceOutput o) {
			half4 c = tex2D (_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		
		ENDCG
	}
}