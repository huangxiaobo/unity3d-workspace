// Upgrade NOTE: replaced 'glstate.matrix.invtrans.modelview[0]' with 'UNITY_MATRIX_IT_MV'
// Upgrade NOTE: replaced 'glstate.matrix.mvp' with 'UNITY_MATRIX_MVP'

Shader "Hidden/ShadowVolume/Extrusion" {
	Properties {
		_Extrusion ("Extrusion", Range(0,30)) = 5.0
	}

	CGINCLUDE

	#include "UnityCG.cginc"

	struct appdata {
		float4 vertex : POSITION;
		float3 normal : TEXCOORD0;
	};

	float _Extrusion;

	// camera space light position
	// xyz = position, w = 1 for point/spot lights
	// xyz = direction, w = 0 for directional lights
	float4 _LightPosition;

	float4 vert( appdata v ) : POSITION {
		
		// point to light vector
		float4 objLightPos = mul( _LightPosition, UNITY_MATRIX_IT_MV );
		float3 toLight = normalize(objLightPos.xyz - v.vertex.xyz * objLightPos.w);
		
		// dot with normal
		float backFactor = dot( toLight, v.normal );
		
		float extrude = (backFactor < 0.0) ? 1.0 : 0.0;
		v.vertex.xyz -= toLight * (extrude * _Extrusion);
		
		return mul( UNITY_MATRIX_MVP, v.vertex );
	}
	ENDCG


	SubShader {
		Tags { "Queue" = "Transparent+10" }
		
		ZWrite Off
		ColorMask A
		Offset 1,1
		
		// Draw front faces, doubling the value in alpha channel
		Pass {
			Cull Back
			Blend DstColor One

			CGINCLUDE
			#pragma vertex vert
			ENDCG

			SetTexture[_MainTex] { constantColor(1,1,1,1) combine constant }		
		
		}
		
		// Draw back faces, halving the value in alpha channel
		Pass {
			Cull Front
			Blend DstColor Zero

			CGINCLUDE
			#pragma vertex vert
			ENDCG

			SetTexture[_MainTex] { constantColor(0.5,0.5,0.5,0.5) combine constant }
		}
	} 

	FallBack Off
}
