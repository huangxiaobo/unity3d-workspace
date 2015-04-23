Shader "CG shader for RGB cube" {
	SubShader {
		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			struct appdata
			{
				float4 vertex	: POSITION;
				float normal 	: NORMAL;
			};

			struct v2f
			{
				float4 pos : POSITION;
				float4 color : COLOR;
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.color = v.normal;
				return o;
			}

			float4 frag(v2f i) : COLOR
			{
				return i.color;
			}
			ENDCG // here begin the vertex and the fragment shader
		}
	}
}