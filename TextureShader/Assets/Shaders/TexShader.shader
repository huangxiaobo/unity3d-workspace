Shader "Shaders/TexShader" { // defines the name of the shader 
   Properties {
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
 
         varying vec4 color; 
            // the Phong lighting computed in the vertex shader
 
         #ifdef VERTEX
 
         void main()
         {				
            mat4 modelMatrix = _Object2World;
            mat4 modelMatrixInverse = _World2Object; // unity_Scale.w 
               // is unnecessary because we normalize vectors
 
            vec3 normalDirection = normalize(vec3(
               vec4(gl_Normal, 0.0) * modelMatrixInverse));
            vec3 viewDirection = normalize(vec3(
               vec4(_WorldSpaceCameraPos, 1.0) 
               - modelMatrix * gl_Vertex));
            vec3 lightDirection;
            float attenuation;
 
            if (0.0 == _WorldSpaceLightPos0.w) // directional light?
            {
               attenuation = 1.0; // no attenuation
               lightDirection = normalize(vec3(_WorldSpaceLightPos0));
            } 
            else // point or spot light
            {
               vec3 vertexToLightSource = vec3(_WorldSpaceLightPos0 
                  - modelMatrix * gl_Vertex);
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
 
            color = vec4(ambientLighting + diffuseReflection 
               + specularReflection, 1.0);
            gl_Position = gl_ModelViewProjectionMatrix * gl_Vertex;
         }
 
         #endif
 
         #ifdef FRAGMENT
 
         void main()
         {
            gl_FragColor = color;
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
 
         varying vec4 color; 
            // the diffuse lighting computed in the vertex shader
 
         #ifdef VERTEX
 
         void main()
         {				
            mat4 modelMatrix = _Object2World;
            mat4 modelMatrixInverse = _World2Object; // unity_Scale.w 
               // is unnecessary because we normalize vectors
 
            vec3 normalDirection = normalize(vec3(
               vec4(gl_Normal, 0.0) * modelMatrixInverse));
            vec3 viewDirection = normalize(vec3(
               vec4(_WorldSpaceCameraPos, 1.0) 
               - modelMatrix * gl_Vertex));
            vec3 lightDirection;
            float attenuation;
 
            if (0.0 == _WorldSpaceLightPos0.w) // directional light?
            {
               attenuation = 1.0; // no attenuation
               lightDirection = normalize(vec3(_WorldSpaceLightPos0));
            } 
            else // point or spot light
            {
               vec3 vertexToLightSource = vec3(_WorldSpaceLightPos0 
                  - modelMatrix * gl_Vertex);
               float distance = length(vertexToLightSource);
               attenuation = 1.0 / distance; // linear attenuation 
               lightDirection = normalize(vertexToLightSource);
            }
 
            // vec3 ambientLighting = 
            //    vec3(gl_LightModel.ambient) * vec3(_Color);
 
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
 
            color = vec4(diffuseReflection + specularReflection, 1.0); 
               // no ambient lighting in this pass
            gl_Position = gl_ModelViewProjectionMatrix * gl_Vertex;
         }
 
         #endif
 
         #ifdef FRAGMENT
 
         void main()
         {
            gl_FragColor = color;
         }
 
         #endif
 
         ENDGLSL
      }
   } 
   // The definition of a fallback shader should be commented out 
   // during development:
   // Fallback "Specular"
}