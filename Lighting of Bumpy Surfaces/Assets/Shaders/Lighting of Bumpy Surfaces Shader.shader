Shader "GLSL normal mapping" {
	Properties {
		_BumpMap ("Normal Map", 2D) = "bump" {}
		_Color ("Diffuse Material Color", Color) = (1,1,1,1) 
		_SpecColor ("Specular Material Color", Color) = (1,1,1,1) 
		_Shininess ("Shininess", Float) = 10
	}
	SubShader {
		Pass {      
			Tags { "LightMode" = "ForwardBase" } 
				// pass for ambient light and first light source
 
			GLSLPROGRAM
 
			// User-specified properties
			uniform sampler2D _BumpMap;	
			uniform vec4 _BumpMap_ST;
			uniform vec4 _Color; 
			uniform vec4 _SpecColor; 
			uniform float _Shininess;
 
			// The following built-in uniforms (except _LightColor0) 
			// are also defined in "UnityCG.glslinc", 
			// i.e. one could #include "UnityCG.glslinc" 
			uniform vec3 _WorldSpaceCameraPos; 
				// camera position in world space
			uniform mat4 _Object2World; // model matrix
			uniform mat4 _World2Object; // inverse model matrix
			uniform vec4 _WorldSpaceLightPos0; 
				// direction to or position of light source
			uniform vec4 _LightColor0; 
				// color of light source (from "Lighting.cginc")
 
			varying vec4 position; 
				// position of the vertex (and fragment) in world space 
			varying vec4 textureCoordinates; 
			varying mat3 localSurface2World; // mapping from local 
				// surface coordinates to world coordinates
 
			#ifdef VERTEX
 
			attribute vec4 Tangent;
 
			void main()
			{                                
				mat4 modelMatrix = _Object2World;
				mat4 modelMatrixInverse = _World2Object; // unity_Scale.w 
					// is unnecessary because we normalize vectors
 
				localSurface2World[0] = normalize(vec3(
					modelMatrix * vec4(vec3(Tangent), 0.0)));
				localSurface2World[2] = normalize(vec3(
					vec4(gl_Normal, 0.0) * modelMatrixInverse));
				localSurface2World[1] = normalize(
					cross(localSurface2World[2], localSurface2World[0]) 
					* Tangent.w); // factor Tangent.w is specific to Unity
 
				position = modelMatrix * gl_Vertex;
				textureCoordinates = gl_MultiTexCoord0;
				gl_Position = gl_ModelViewProjectionMatrix * gl_Vertex;
			}
 
			#endif
 
			#ifdef FRAGMENT
 
			void main()
			{
				// in principle we have to normalize the columns of 
				// "localSurface2World" again; however, the potential 
				// problems are small since we use this matrix only to
				// compute "normalDirection", which we normalize anyways
 
				vec4 encodedNormal = texture2D(_BumpMap, 
					_BumpMap_ST.xy * textureCoordinates.xy 
					+ _BumpMap_ST.zw);
				vec3 localCoords = 
					vec3(2.0 * encodedNormal.ag - vec2(1.0), 0.0);
				localCoords.z = sqrt(1.0 - dot(localCoords, localCoords));
					// approximation without sqrt: localCoords.z = 
					// 1.0 - 0.5 * dot(localCoords, localCoords);
				vec3 normalDirection = 
					normalize(localSurface2World * localCoords);
 
				vec3 viewDirection = 
					normalize(_WorldSpaceCameraPos - vec3(position));
				vec3 lightDirection;
				float attenuation;
 
				if (0.0 == _WorldSpaceLightPos0.w) // directional light?
				{
					attenuation = 1.0; // no attenuation
					lightDirection = normalize(vec3(_WorldSpaceLightPos0));
				} 
				else // point or spot light
				{
					vec3 vertexToLightSource = 
						vec3(_WorldSpaceLightPos0 - position);
					float distance = length(vertexToLightSource);
					attenuation = 1.0 / distance; // linear attenuation 
					lightDirection = normalize(vertexToLightSource);
				}
 
				vec3 ambientLighting = 
					vec3(gl_LightModel.ambient) * vec3(_Color);
 
				vec3 diffuseReflection = 
					attenuation * vec3(_LightColor0) * vec3(_Color) 
					* max(0.0, dot(normalDirection, lightDirection));
 
				vec3 specularReflection;
				if (dot(normalDirection, lightDirection) < 0.0) 
					// light source on the wrong side?
				{
					specularReflection = vec3(0.0, 0.0, 0.0); 
						// no specular reflection
				}
				else // light source on the right side
				{
					specularReflection = attenuation * vec3(_LightColor0) 
						* vec3(_SpecColor) * pow(max(0.0, dot(
						reflect(-lightDirection, normalDirection), 
						viewDirection)), _Shininess);
				}
 
				gl_FragColor = vec4(ambientLighting 
					+ diffuseReflection + specularReflection, 1.0);
			}
 
			#endif
 
			ENDGLSL
		}
 
		Pass {      
			Tags { "LightMode" = "ForwardAdd" } 
				// pass for additional light sources
			Blend One One // additive blending 
 
		  GLSLPROGRAM
 
			// User-specified properties
			uniform sampler2D _BumpMap;	
			uniform vec4 _BumpMap_ST;
			uniform vec4 _Color; 
			uniform vec4 _SpecColor; 
			uniform float _Shininess;
 
			// The following built-in uniforms (except _LightColor0) 
			// are also defined in "UnityCG.glslinc", 
			// i.e. one could #include "UnityCG.glslinc" 
			uniform vec3 _WorldSpaceCameraPos; 
				// camera position in world space
			uniform mat4 _Object2World; // model matrix
			uniform mat4 _World2Object; // inverse model matrix
			uniform vec4 _WorldSpaceLightPos0; 
				// direction to or position of light source
			uniform vec4 _LightColor0; 
				// color of light source (from "Lighting.cginc")
 
			varying vec4 position; 
				// position of the vertex (and fragment) in world space 
			varying vec4 textureCoordinates; 
			varying mat3 localSurface2World; // mapping from 
				// local surface coordinates to world coordinates
 
			#ifdef VERTEX
 
			attribute vec4 Tangent;
 
			void main()
			{                                
				mat4 modelMatrix = _Object2World;
				mat4 modelMatrixInverse = _World2Object; // unity_Scale.w 
					// is unnecessary because we normalize vectors
 
				localSurface2World[0] = normalize(vec3(
					modelMatrix * vec4(vec3(Tangent), 0.0)));
				localSurface2World[2] = normalize(vec3(
					vec4(gl_Normal, 0.0) * modelMatrixInverse));
				localSurface2World[1] = normalize(
					cross(localSurface2World[2], localSurface2World[0]) 
					* Tangent.w); // factor Tangent.w is specific to Unity
 
				position = modelMatrix * gl_Vertex;
				textureCoordinates = gl_MultiTexCoord0;
				gl_Position = gl_ModelViewProjectionMatrix * gl_Vertex;
			}
 
			#endif
 
			#ifdef FRAGMENT
 
			void main()
			{
				// in principle we have to normalize the columns of 
				// "localSurface2World" again; however, the potential 
				// problems are small since we use this matrix only to
				// compute "normalDirection", which we normalize anyways
 
				vec4 encodedNormal = texture2D(_BumpMap, 
					_BumpMap_ST.xy * textureCoordinates.xy 
					+ _BumpMap_ST.zw);
				vec3 localCoords = 
					vec3(2.0 * encodedNormal.ag - vec2(1.0), 0.0);
				localCoords.z = sqrt(1.0 - dot(localCoords, localCoords));
					// approximation without sqrt: localCoords.z = 
					// 1.0 - 0.5 * dot(localCoords, localCoords);
				vec3 normalDirection = 
					normalize(localSurface2World * localCoords);
 
				vec3 viewDirection = 
					normalize(_WorldSpaceCameraPos - vec3(position));
				vec3 lightDirection;
				float attenuation;
 
				if (0.0 == _WorldSpaceLightPos0.w) // directional light?
				{
					attenuation = 1.0; // no attenuation
					lightDirection = normalize(vec3(_WorldSpaceLightPos0));
				} 
				else // point or spot light
				{
					vec3 vertexToLightSource = 
						vec3(_WorldSpaceLightPos0 - position);
					float distance = length(vertexToLightSource);
					attenuation = 1.0 / distance; // linear attenuation 
					lightDirection = normalize(vertexToLightSource);
				}
 
				vec3 diffuseReflection = 
					attenuation * vec3(_LightColor0) * vec3(_Color) 
					* max(0.0, dot(normalDirection, lightDirection));
 
				vec3 specularReflection;
				if (dot(normalDirection, lightDirection) < 0.0) 
					// light source on the wrong side?
				{
					specularReflection = vec3(0.0, 0.0, 0.0); 
						// no specular reflection
				}
				else // light source on the right side
				{
					specularReflection = attenuation * vec3(_LightColor0) 
						* vec3(_SpecColor) * pow(max(0.0, dot(
						reflect(-lightDirection, normalDirection), 
						viewDirection)), _Shininess);
				}
 
				gl_FragColor = 
					vec4(diffuseReflection + specularReflection, 1.0);
			}
 
			#endif
 
			ENDGLSL
		}
	} 
	// The definition of a fallback shader should be commented out 
	// during development:
	// Fallback "Bumped Specular"
}