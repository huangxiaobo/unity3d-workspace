Shader "Shaders/BasicDiffuse" { // defines the name of the shader 
Properties {
      _MainTex ("Texture Image", 2D) = "white" {} 
      _AmbientColor ("Ambient Color", Color) = (0.5, 0.5, 0.5, 0.5)
      _SpecularColor ("SpecularColor", Color) = (1.0, 1.0, 1.0, 1)
      _DiffuseColor ("DiffuseColor", Color) = (0.0, 0.5, 0.5, 1)
      _Shinniess ("Shiniess", Float) = 10
         // a 2D texture property that we call "_MainTex", which should
         // be labeled "Texture Image" in Unity's user interface.
         // By default we use the built-in texture "white"  
         // (alternatives: "black", "gray" and "bump").
   }
   SubShader {
      Pass {	
         GLSLPROGRAM
 
         uniform sampler2D _MainTex;	
         uniform vec4 _AmbientColor;
         uniform vec4 _SpecularColor;
         uniform vec4 _DiffuseColor;
         uniform float _Shiniess;
         
            // a uniform variable refering to the property above
            // (in fact, this is just a small integer specifying a 
            // "texture unit", which has the texture image "bound" 
            // to it; Unity takes care of this).
 
         varying vec4 textureCoordinates;
         varying vec4 diffuseColor;
         varying vec4 specularColor;
      
            // the texture coordinates at the vertices,
            // which are interpolated for each fragment
 
         #ifdef VERTEX
 
         void main()
         {
            textureCoordinates = gl_MultiTexCoord0;
               // Unity provides default longitude-latitude-like 
               // texture coordinates at all vertices of a 
               // sphere mesh as the attribute "gl_MultiTexCoord0".
            gl_Position = gl_ModelViewProjectionMatrix * gl_Vertex;
         }
 
         #endif
 
         #ifdef FRAGMENT
 
         void main()
         {
            gl_FragColor = 
               texture2D(_MainTex, vec2(textureCoordinates));	
               // look up the color of the texture image specified by 
               // the uniform "_MainTex" at the position specified by 
               // "textureCoordinates.x" and "textureCoordinates.y" 
               // and return it in "gl_FragColor"             
         }
 
         #endif
 
         ENDGLSL
      }
   }
   // The definition of a fallback shader should be commented out 
   // during development:
   // Fallback "Unlit/Texture"
}