Shader "OutlineShader" {
	Properties {
		_MainTex ("Texture Image", 2D) = "white"  {}
		_OutlineColor ("Outline Color", Color) = (1, 0, 0, 1)
		_Outline ("Outline width", Range (.002, 0.03)) = .005 
		
	}
	SubShader {
		Pass {	
			// Tags { "LightMode" = "Always" } 
			// pass for ambient light and first light source
			Cull Front  
			ZWrite Off  
			ColorMask RGB 	 
			
			GLSLPROGRAM
			
			uniform vec4 _OutlineColor;
			uniform float _Outline;
			uniform sampler2D _MainTex;

			uniform mat4 UNITY_MATRIX_P;
			uniform mat4 _Object2World; // model matrix
			uniform mat4 _World2Object; // inverse model matrix
	 
			#ifdef VERTEX
					 
			vec2 TransformViewToProjection (vec2 v) {
				return vec2(v.x*UNITY_MATRIX_P[0][0], v.y*UNITY_MATRIX_P[1][1]);
			}
	 
			void main()
			{				
				vec4 position = _Object2World * gl_Vertex;
				vec3 varyingNormalDirection = normalize(vec3(vec4(gl_Normal, 0.0) * _World2Object));

				vec2 offset = TransformViewToProjection(varyingNormalDirection.xy);
				gl_Position = gl_ModelViewProjectionMatrix * gl_Vertex;
				gl_Position.xy += offset * gl_Position.z * _Outline;
			}

			#endif

			#ifdef FRAGMENT

			void main()
			{
				gl_FragColor = _OutlineColor;
			}

			#endif

			ENDGLSL
		}
		
		Pass {	
			// Tags { "LightMode" = "Always" } 
			// pass for ambient light and first light source
			Cull Back  
			ZWrite On  
			ColorMask RGB  
			Blend SrcAlpha OneMinusSrcAlpha 
			
			GLSLPROGRAM
			
			uniform vec4 _OutlineColor;
			uniform float _Outline;
			uniform sampler2D _MainTex;

			uniform mat4 UNITY_MATRIX_P;
			uniform mat4 _Object2World; // model matrix
			uniform mat4 _World2Object; // inverse model matrix
			
			varying vec4 textureCoordinates; 
	 
			#ifdef VERTEX
					 

			void main()
			{
				textureCoordinates = gl_MultiTexCoord0;
				gl_Position = gl_ModelViewProjectionMatrix * gl_Vertex;
			}

			#endif

			#ifdef FRAGMENT

			void main()
			{
				gl_FragColor = texture2D(_MainTex, vec2(textureCoordinates));	
			}

			#endif

			ENDGLSL
		}
	} 	
	//FallBack "Diffuse"
}
