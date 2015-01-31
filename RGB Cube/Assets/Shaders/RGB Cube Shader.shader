Shader "GLSL shader for RGB cube" {
	SubShader {
		Pass {
			GLSLPROGRAM // here begin the vertex and the fragment shader
 
			varying vec4 position; 
				// this line is part of the vertex and the fragment shader 
 
			#ifdef VERTEX 
				// here begins the part that is only in the vertex shader
 
			void main()
			{
				position = gl_Vertex + vec4(0.5, 0.5, 0.5, 0.0);
				gl_Position = gl_ModelViewProjectionMatrix * gl_Vertex;
			}
 
			#endif 
				// here ends the part that is only in the vertex shader
 
			#ifdef FRAGMENT 
				// here begins the part that is only in the fragment shader
 
			void main()
			{
				gl_FragColor = position;
			}
 
			#endif 
				// here ends the part that is only in the fragment shader
 
			ENDGLSL // here end the vertex and the fragment shader
		}
	}
}