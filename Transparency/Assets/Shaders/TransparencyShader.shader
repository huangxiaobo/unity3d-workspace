Shader "CG shader using blending (including back faces)" {
	SubShader {
		Tags { "Queue" = "Transparent" } 
			// draw after all opaque geometry has been drawn
		Pass { 
			Cull Front // first pass renders only back faces 
				 // (the "inside")
			ZWrite Off // don't write to depth buffer 
				 // in order not to occlude other objects
			Blend SrcAlpha OneMinusSrcAlpha // use alpha blending
 
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			struct appdata {
				float4 vertex : POSITION;
			};
			
			struct v2f
			{
				float4 pos : POSITION;
				float3 color : COLOR;
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				return o;
			}

			float4 frag(v2f io) : COLOR
			{
				return float4(1.0, 0.0, 0.0, 0.3);
			}
			ENDCG
		}
 
		Pass {
			Cull Back // second pass renders only front faces 
				// (the "outside")
			ZWrite Off // don't write to depth buffer 
				// in order not to occlude other objects
			Blend SrcAlpha OneMinusSrcAlpha 
				// standard blend equation "source over destination"
 
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			struct appdata {
				float4 vertex : POSITION;
			};
			
			struct v2f
			{
				float4 pos : POSITION;
				float3 color : COLOR;
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				return o;
			}

			float4 frag(v2f io) : COLOR
			{
				return float4(0.0, 1.0, 0.0, 0.3);
			}
			ENDCG
		}
	}
}