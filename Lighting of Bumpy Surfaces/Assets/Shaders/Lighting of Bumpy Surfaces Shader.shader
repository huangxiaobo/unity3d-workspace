// Upgrade NOTE: commented out 'float4x4 _Object2World', a built-in variable
// Upgrade NOTE: commented out 'float4x4 _World2Object', a built-in variable

Shader "CG normal mapping" {
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
 
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// User-specified properties
			uniform sampler2D _BumpMap;	
			uniform float4 _BumpMap_ST;
			uniform float4 _Color; 
			uniform float4 _SpecColor; 
			uniform float _Shininess;
 
			// The following built-in uniforms (except _LightColor0) 
			// are also defined in "UnityCG.glslinc", 
			// i.e. one could #include "UnityCG.glslinc" 
			// uniform float3 _WorldSpaceCameraPos; 
				// camera position in world space
			// uniform float4x4 _Object2World; // model matrix
			// uniform float4x4 _World2Object; // inverse model matrix
			uniform float4 _WorldSpaceLightPos0; 
				// direction to or position of light source
			uniform float4 _LightColor0; 
				// color of light source (from "Lighting.cginc")
 
			varying float4 position; 
				// position of the vertex (and fragment) in world space 
			varying float4 textureCoordinates; 
			varying float3x3 localSurface2World; // mapping from local 
				// surface coordinates to world coordinates
 
			
  
			void main()
			{                                
				float4x4 modelMatrix = _Object2World;
				float4x4 modelMatrixInverse = _World2Object; // unity_Scale.w 
					// is unnecessary because we normalize vectors
 
				localSurface2World[0] = normalize(float3(
					modelMatrix * float4(float3(Tangent), 0.0)));
				localSurface2World[2] = normalize(float3(
					float4(gl_Normal, 0.0) * modelMatrixInverse));
				localSurface2World[1] = normalize(
					cross(localSurface2World[2], localSurface2World[0]) 
					* Tangent.w); // factor Tangent.w is specific to Unity
 
				position = modelMatrix * gl_Vertex;
				textureCoordinates = gl_MultiTexCoord0;
				gl_Position = gl_ModelViewProjectionMatrix * gl_Vertex;
			}
 

 
			void main()
			{
				// in principle we have to normalize the columns of 
				// "localSurface2World" again; however, the potential 
				// problems are small since we use this matrix only to
				// compute "normalDirection", which we normalize anyways
 
				float4 encodedNormal = texture2D(_BumpMap, 
					_BumpMap_ST.xy * textureCoordinates.xy 
					+ _BumpMap_ST.zw);
				float3 localCoords = 
					float3(2.0 * encodedNormal.ag - vec2(1.0), 0.0);
				localCoords.z = sqrt(1.0 - dot(localCoords, localCoords));
					// approximation without sqrt: localCoords.z = 
					// 1.0 - 0.5 * dot(localCoords, localCoords);
				float3 normalDirection = 
					normalize(localSurface2World * localCoords);
 
				float3 viewDirection = 
					normalize(_WorldSpaceCameraPos - float3(position));
				float3 lightDirection;
				float attenuation;
 
				if (0.0 == _WorldSpaceLightPos0.w) // directional light?
				{
					attenuation = 1.0; // no attenuation
					lightDirection = normalize(float3(_WorldSpaceLightPos0));
				} 
				else // point or spot light
				{
					float3 vertexToLightSource = 
						float3(_WorldSpaceLightPos0 - position);
					float distance = length(vertexToLightSource);
					attenuation = 1.0 / distance; // linear attenuation 
					lightDirection = normalize(vertexToLightSource);
				}
 
				float3 ambientLighting = 
					float3(gl_LightModel.ambient) * float3(_Color);
 
				float3 diffuseReflection = 
					attenuation * float3(_LightColor0) * float3(_Color) 
					* max(0.0, dot(normalDirection, lightDirection));
 
				float3 specularReflection;
				if (dot(normalDirection, lightDirection) < 0.0) 
					// light source on the wrong side?
				{
					specularReflection = float3(0.0, 0.0, 0.0); 
						// no specular reflection
				}
				else // light source on the right side
				{
					specularReflection = attenuation * float3(_LightColor0) 
						* float3(_SpecColor) * pow(max(0.0, dot(
						reflect(-lightDirection, normalDirection), 
						viewDirection)), _Shininess);
				}
 
				gl_FragColor = float4(ambientLighting 
					+ diffuseReflection + specularReflection, 1.0);
			}
 
			ENDCG
		}
 
		Pass {      
			Tags { "LightMode" = "ForwardAdd" } 
				// pass for additional light sources
			Blend One One // additive blending 
 
		  GLSLPROGRAM
 
			// User-specified properties
			uniform sampler2D _BumpMap;	
			uniform float4 _BumpMap_ST;
			uniform float4 _Color; 
			uniform float4 _SpecColor; 
			uniform float _Shininess;
 
			// The following built-in uniforms (except _LightColor0) 
			// are also defined in "UnityCG.glslinc", 
			// i.e. one could #include "UnityCG.glslinc" 
			uniform float3 _WorldSpaceCameraPos; 
				// camera position in world space
			uniform float4x4 _Object2World; // model matrix
			uniform float4x4 _World2Object; // inverse model matrix
			uniform float4 _WorldSpaceLightPos0; 
				// direction to or position of light source
			uniform float4 _LightColor0; 
				// color of light source (from "Lighting.cginc")
 
			varying float4 position; 
				// position of the vertex (and fragment) in world space 
			varying float4 textureCoordinates; 
			varying float3x3 localSurface2World; // mapping from 
				// local surface coordinates to world coordinates
 
			#ifdef VERTEX
 
			attribute float4 Tangent;
 
			void main()
			{                                
				float4x4 modelMatrix = _Object2World;
				float4x4 modelMatrixInverse = _World2Object; // unity_Scale.w 
					// is unnecessary because we normalize vectors
 
				localSurface2World[0] = normalize(float3(
					modelMatrix * float4(float3(Tangent), 0.0)));
				localSurface2World[2] = normalize(float3(
					float4(gl_Normal, 0.0) * modelMatrixInverse));
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
 
				float4 encodedNormal = texture2D(_BumpMap, 
					_BumpMap_ST.xy * textureCoordinates.xy 
					+ _BumpMap_ST.zw);
				float3 localCoords = 
					float3(2.0 * encodedNormal.ag - vec2(1.0), 0.0);
				localCoords.z = sqrt(1.0 - dot(localCoords, localCoords));
					// approximation without sqrt: localCoords.z = 
					// 1.0 - 0.5 * dot(localCoords, localCoords);
				float3 normalDirection = 
					normalize(localSurface2World * localCoords);
 
				float3 viewDirection = 
					normalize(_WorldSpaceCameraPos - float3(position));
				float3 lightDirection;
				float attenuation;
 
				if (0.0 == _WorldSpaceLightPos0.w) // directional light?
				{
					attenuation = 1.0; // no attenuation
					lightDirection = normalize(float3(_WorldSpaceLightPos0));
				} 
				else // point or spot light
				{
					float3 vertexToLightSource = 
						float3(_WorldSpaceLightPos0 - position);
					float distance = length(vertexToLightSource);
					attenuation = 1.0 / distance; // linear attenuation 
					lightDirection = normalize(vertexToLightSource);
				}
 
				float3 diffuseReflection = 
					attenuation * float3(_LightColor0) * float3(_Color) 
					* max(0.0, dot(normalDirection, lightDirection));
 
				float3 specularReflection;
				if (dot(normalDirection, lightDirection) < 0.0) 
					// light source on the wrong side?
				{
					specularReflection = float3(0.0, 0.0, 0.0); 
						// no specular reflection
				}
				else // light source on the right side
				{
					specularReflection = attenuation * float3(_LightColor0) 
						* float3(_SpecColor) * pow(max(0.0, dot(
						reflect(-lightDirection, normalDirection), 
						viewDirection)), _Shininess);
				}
 
				gl_FragColor = 
					float4(diffuseReflection + specularReflection, 1.0);
			}
 
			#endif
 
			ENDGLSL
		}
	} 
	// The definition of a fallback shader should be commented out 
	// during development:
	// Fallback "Bumped Specular"
}