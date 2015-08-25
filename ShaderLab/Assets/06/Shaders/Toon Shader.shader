Shader "ShaderLib/06-01" {
	Properties {
		_Color ("Diffuse Color 散射颜色", Color) = (1,1,1,1) 
		_UnlitColor ("Unlit Diffuse Color 未照亮的散射颜色", Color) = (0.5,0.5,0.5,1) 
		_DiffuseThreshold ("Threshold for Diffuse Colors 散射颜色的阈值", Range(0,1)) 
			= 0.1 
		_OutlineColor ("Outline Color 轮廓颜色", Color) = (0,0,0,1)
		_LitOutlineThickness ("Lit Outline Thickness 照亮轮廓的厚度", Range(0,1)) = 0.1
		_UnlitOutlineThickness ("Unlit Outline Thickness 未照亮的轮廓厚度", Range(0,1)) 
			= 0.4
		_SpecColor ("Specular Color 镜面颜色", Color) = (1,1,1,1) 
		_Shininess ("Shininess 高亮系数", Float) = 10
	}
	SubShader {
		Pass {      
			Tags { "LightMode" = "ForwardBase" } 
			// pass for ambient light and first light source 环境光和第一个光源
 
			CGPROGRAM
 
			#pragma vertex vert  
			#pragma fragment frag 
 
			#include "UnityCG.cginc"
			uniform float4 _LightColor0; 
			// color of light source (from "Lighting.cginc") 光源的颜色
 
			// User-specified properties
			uniform float4 _Color; 
			uniform float4 _UnlitColor;
			uniform float _DiffuseThreshold;
			uniform float4 _OutlineColor;
			uniform float _LitOutlineThickness;
			uniform float _UnlitOutlineThickness;
			uniform float4 _SpecColor; 
			uniform float _Shininess;
 
			struct vertexInput {
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			};
			struct vertexOutput {
				float4 pos : SV_POSITION;
				float4 posWorld : TEXCOORD0;
				float3 normalDir : TEXCOORD1;
			};
 
			vertexOutput vert(vertexInput input) 
			{
				vertexOutput output;
 
				float4x4 modelMatrix = _Object2World;
				float4x4 modelMatrixInverse = _World2Object; 
				// multiplication with unity_Scale.w is unnecessary 
				// because we normalize transformed vectors
 
				output.posWorld = mul(modelMatrix, input.vertex);
				output.normalDir = normalize(mul(float4(input.normal, 0.0), modelMatrixInverse).xyz);
				output.pos = mul(UNITY_MATRIX_MVP, input.vertex);
				return output;
			}
 
			float4 frag(vertexOutput input) : COLOR
			{
				float3 normalDirection = normalize(input.normalDir);
 
				float3 viewDirection = normalize(
					_WorldSpaceCameraPos - input.posWorld.xyz);
				float3 lightDirection;
				float attenuation;
 
				if (0.0 == _WorldSpaceLightPos0.w) // 方向光
				{
					attenuation = 1.0; // 无衰减
					lightDirection = normalize(_WorldSpaceLightPos0.xyz);
				} 
				else // 点光源或者聚光源
				{
					float3 vertexToLightSource = _WorldSpaceLightPos0.xyz - input.posWorld.xyz;
					float distance = length(vertexToLightSource);
					attenuation = 1.0 / distance; // 线性衰减
					lightDirection = normalize(vertexToLightSource);
				}
  
				// 默认: unlit  未照亮的散射颜色
				float3 fragmentColor = _UnlitColor.rgb; 
 
				// 低优先: 散射颜色
				if (attenuation * max(0.0, dot(normalDirection, lightDirection)) >= _DiffuseThreshold)
				{
					fragmentColor = _LightColor0.rgb * _Color.rgb; 
				}
 
				// 高优先: 轮廓
				if (dot(viewDirection, normalDirection) < lerp(_UnlitOutlineThickness, _LitOutlineThickness, max(0.0, dot(normalDirection, lightDirection))))
				{
					fragmentColor = _LightColor0.rgb * _OutlineColor.rgb; 
				}
 
				// 最高优先: 高亮光
				if (dot(normalDirection, lightDirection) > 0.0 && attenuation *  pow(max(0.0, dot(reflect(-lightDirection, normalDirection), viewDirection)), _Shininess) > 0.5) // light source on the right side? more than half highlight intensity? 
				{
					fragmentColor = _SpecColor.a * _LightColor0.rgb * _SpecColor.rgb + (1.0 - _SpecColor.a) * fragmentColor;
				}
				return float4(fragmentColor, 1.0);
			}
			ENDCG
		}
 
		Pass {      
			Tags { "LightMode" = "ForwardAdd" } 
				// 额外光源通道
			Blend SrcAlpha OneMinusSrcAlpha 
				// 镜面高亮混合 blend specular highlights over framebuffer
 
			CGPROGRAM
 
			#pragma vertex vert  
			#pragma fragment frag 
 
			#include "UnityCG.cginc"
			uniform float4 _LightColor0; // 光源的颜色 (from "Lighting.cginc")
 
			// 自定义属性
			uniform float4 _Color; 
			uniform float4 _UnlitColor;
			uniform float _DiffuseThreshold;
			uniform float4 _OutlineColor;
			uniform float _LitOutlineThickness;
			uniform float _UnlitOutlineThickness;
			uniform float4 _SpecColor; 
			uniform float _Shininess;
 
			struct vertexInput {
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			};
			struct vertexOutput {
				float4 pos : SV_POSITION;
				float4 posWorld : TEXCOORD0;
				float3 normalDir : TEXCOORD1;
			};
 
			vertexOutput vert(vertexInput input) 
			{
				vertexOutput output;
 
				float4x4 modelMatrix = _Object2World;
				float4x4 modelMatrixInverse = _World2Object; 
					// multiplication with unity_Scale.w is unnecessary 
					// because we normalize transformed vectors
 
				output.posWorld = mul(modelMatrix, input.vertex);
				output.normalDir = normalize(mul(float4(input.normal, 0.0), modelMatrixInverse).rgb);
				output.pos = mul(UNITY_MATRIX_MVP, input.vertex);
				return output;
			}
 
			float4 frag(vertexOutput input) : COLOR
			{
				float3 normalDirection = normalize(input.normalDir);
 
				float3 viewDirection = normalize(_WorldSpaceCameraPos - input.posWorld.rgb);
				float3 lightDirection;
				float attenuation;
 
				if (0.0 == _WorldSpaceLightPos0.w) // 如果是方向光
				{
					attenuation = 1.0; // 无衰减
					lightDirection = normalize(_WorldSpaceLightPos0.xyz);
				} 
				else // 点光源或者聚光源
				{
					float3 vertexToLightSource = _WorldSpaceLightPos0.xyz - input.posWorld.xyz;
					float distance = length(vertexToLightSource);
					attenuation = 1.0 / distance; // 线性衰减
					lightDirection = normalize(vertexToLightSource);
				}
 
				float4 fragmentColor = float4(0.0, 0.0, 0.0, 0.0);
				if (dot(normalDirection, lightDirection) > 0.0 && attenuation *  pow(max(0.0, dot(reflect(-lightDirection, normalDirection), viewDirection)), _Shininess) > 0.5) 
				// 光源在正确的方向，并且强度大于高亮强度的一半
				{
					fragmentColor = float4(_LightColor0.rgb, 1.0) * _SpecColor;
				}
				return fragmentColor;
			}
			ENDCG
		}
	} 
	// The definition of a fallback shader should be commented out 
	// during development:
	// Fallback "Specular"
}