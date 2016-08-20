Shader "ShaderLib/23/BRDF"
{
	Properties 
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_DiffuseColor ("DiffuseColor", Color) = (0, 0, 0, 0)
		_SpecularColor ("Specular Color", Color) = (1,1,1,1)
		_SpecularPower ("Specular Power", Range (0.1, 100)) = 0.078125
	}
	
	SubShader 
	{
		Tags { "RenderType"="Opaque" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf MetallicSoft
		#pragma target 3.0

		sampler2D _MainTex;
		float4 _DiffuseColor;
		float _SpecularPower;
		float4 _SpecularColor;
		
		inline fixed4 LightingMetallicSoft (SurfaceOutput s, fixed3 lightDir, half3 viewDir, fixed atten)
		{

			//半角向量：求（点到光源+点到摄像机）的单位向量，他们的中间平均值  
			float3 halfVector = normalize(lightDir + viewDir);

			float NdotL = saturate(dot(s.Normal, normalize(lightDir)));
			float NdotH_raw = dot(s.Normal, halfVector);
			float NdotH = saturate(dot(s.Normal, halfVector));
			float NdotV = saturate(dot(s.Normal, normalize(viewDir)));
			float VdotH = saturate(dot(halfVector, normalize(viewDir)));
			float HdotL = saturate(dot(halfVector, normalize(lightDir)));
			
			//套用Cook-Torrance公式
			float D = (_SpecularPower + 2) / 8 * pow(NdotH, _SpecularPower);
			float F = _SpecularColor + (1 - _SpecularColor) * pow((1 - HdotL), 5);
			float K = 2 / sqrt(4 * (_SpecularPower + 2));
			float V = 1 / ((NdotL * (1 - K) + K) * (NdotV * (1 - K) + K));
			float cook = D * F * V;

			//漫反射系数【点到光源单位向量与法线向量的余弦值】  
			float diff = dot(s.Normal, lightDir);    
			diff = (1 - cook) * diff; //处于能量守恒考虑，漫反射做了镜面反射就少  
			float4 c;
			c.rgb = s.Albedo *  (diff + cook) * _LightColor0.rgb;
			c.a = 1;
			return c;
		}

		struct Input 
		{
			float2 uv_MainTex;
		};

		void surf (Input IN, inout SurfaceOutput o) 
		{
			half4 c = tex2D (_MainTex, IN.uv_MainTex) * _DiffuseColor;
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	} 
}
