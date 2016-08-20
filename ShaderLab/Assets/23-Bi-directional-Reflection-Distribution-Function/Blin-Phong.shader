Shader "Hidden/ShaderLib/23/Blin-Phong"
{
	Properties
	{	
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_DiffuseColor ("DiffuseColor", Color) = (0, 0, 0, 0)
		_SpecularColor ("SpecularColor", Color) = (0, 0, 0, 0)
		_SpecularPower ("SpecularPower", Range(0, 30)) = 5
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100


		CGPROGRAM
		#pragma surface surf Phong  
		float4 _DiffuseColor;
		float4 _SpecularColor;
		float _SpecularPower;

		sampler2D _MainTex;
		
		inline fixed4 LightingPhong (SurfaceOutput s, fixed3 lightDir, fixed3 viewDir, fixed atten)
		{
			float3 ambient = (s.Albedo * _LightColor0.rgb * dot(s.Normal, lightDir));

			float3 diffuse = _DiffuseColor.rgb * dot(lightDir, s.Normal) * _DiffuseColor.a;

			float3 halfVector = normalize(lightDir + viewDir);
			float spec = pow(max(0, dot(halfVector, s.Normal)), _SpecularPower);
			float3 specular = _LightColor0.rgb * _SpecularColor * spec;


			float4 c;
			c.rgb = ambient + diffuse + specular;
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