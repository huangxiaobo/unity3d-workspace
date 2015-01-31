Shader "GLSL shader using blending (including back faces)" {
	SubShader {
		Tags { "Queue" = "Transparent" } 
			// draw after all opaque geometry has been drawn
		Pass { 
			Cull Front // first pass renders only back faces 
				 // (the "inside")
			ZWrite Off // don't write to depth buffer 
				 // in order not to occlude other objects
			Blend SrcAlpha OneMinusSrcAlpha // use alpha blending
 
			GLSLPROGRAM
 
			#ifdef VERTEX
 
			void main()
			{
				gl_Position = gl_ModelViewProjectionMatrix * gl_Vertex;
			}
 
			#endif
 
 
			#ifdef FRAGMENT
 
			void main()
			{
				gl_FragColor = vec4(1.0, 0.0, 0.0, 0.3);
					// the fourth component (alpha) is important: 
					// this is semitransparent red
			}
 
			#endif
 
			ENDGLSL
		}
 
		Pass {
			Cull Back // second pass renders only front faces 
				// (the "outside")
			ZWrite Off // don't write to depth buffer 
				// in order not to occlude other objects
			Blend SrcAlpha OneMinusSrcAlpha 
				// standard blend equation "source over destination"
 
			GLSLPROGRAM
 
			#ifdef VERTEX
 
			void main()
			{
				gl_Position = gl_ModelViewProjectionMatrix * gl_Vertex;
			}
 
			#endif
 
 
			#ifdef FRAGMENT
 
			void main()
			{
				gl_FragColor = vec4(0.0, 1.0, 0.0, 0.3);
					// fourth component (alpha) is important: 
					// this is semitransparent green
			}
 
			#endif
 
			ENDGLSL
		}
	}
}