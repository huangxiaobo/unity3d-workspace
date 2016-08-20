Shader "Hidden/ShaderLib/05/Waite" {
	Properties {
		_CornerRadius("Corner Radius", Float) = 0.1
	}
	SubShader {
		// No culling or depth
		Blend SrcAlpha  OneMinusSrcAlpha
		Cull Off ZWrite Off ZTest Always

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
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.uv;
				return o;
			}

			fixed calcDot(fixed a, fixed ca, fixed2 uv) {  
				a /= 57.295779513;  
				ca /= 57.295779513;  
				fixed tt = 180/57.295779513;  
				uv = (fixed2(cos(a), sin(a)) * 0.2+ uv)*10;  
				fixed adit = tt*2*step(tt, a-ca);  
				fixed r = 1-step(ca + adit, a);  
				r *= lerp(0.2, -1, saturate((ca-a+adit)/25))*2;  
				return smoothstep(r-0.2, r, length(uv.xy));  
			}  	
			
			fixed4 frag (v2f i) : SV_Target
			{
				float2 uv = i.uv;
				uv = uv.xy - float2(0.5, 0.5);

				// 背景
				float rx = fmod(uv.x, 0.4);
				float ry = fmod(uv.y, 0.4);
				float mx = step(0.4, abs(uv.x));
				float my = step(0.4, abs(uv.y));
				float alpha = 1 - mx * my * step(0.1, length(float2(rx, ry)));

				fixed4 foreColor = fixed4(1, 1, 1, 1);  
				fixed4 bgColor = fixed4(fixed3(0.4, 0.4, 0.4),alpha);  
				fixed4 result = bgColor;  
				// 圆点
				float ca = fmod(_Time.y, 2) * 180;	//角度
				bgColor = lerp(foreColor, bgColor, calcDot(0, ca, uv));  
				bgColor = lerp(foreColor, bgColor, calcDot(30, ca, uv));  
				bgColor = lerp(foreColor, bgColor, calcDot(60, ca, uv));  
				bgColor = lerp(foreColor, bgColor, calcDot(90, ca, uv));  
				bgColor = lerp(foreColor, bgColor, calcDot(120, ca, uv));  
				bgColor = lerp(foreColor, bgColor, calcDot(150, ca, uv));  
				bgColor = lerp(foreColor, bgColor, calcDot(180, ca, uv));  
				bgColor = lerp(foreColor, bgColor, calcDot(210, ca, uv));  
				bgColor = lerp(foreColor, bgColor, calcDot(240, ca, uv));  
				bgColor = lerp(foreColor, bgColor, calcDot(270, ca, uv));  
				bgColor = lerp(foreColor, bgColor, calcDot(300, ca, uv));  
				bgColor = lerp(foreColor, bgColor, calcDot(330, ca, uv));  

				return bgColor;
			}
			ENDCG
		}
	} 
	FallBack "Diffuse"
}
