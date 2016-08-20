Shader "Hidden/ShaderLib/16/ShadowVolume/Extrusion" {
	Properties {
		_Extrusion ("Extrusion", Range(0,30)) = 5.0
	}

	CGINCLUDE

	#include "UnityCG.cginc"

	struct appdata {
		float4 vertex : POSITION;
		float3 normal : NORMAL;
	};

	uniform float _Extrusion;

	// camera space light position
	// xyz = position, w = 1 for point/spot lights
	// xyz = direction, w = 0 for directional lights
	uniform float4 _LightPosition;

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

	float4 frag (float4 pos:POSITION)  : COLOR
	{
		return float4(1, 0, 0, 1);
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
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			ENDCG
		
			SetTexture[_MainTex] { constantColor(1,1,1,1) combine constant }		
		}
		
		// Draw back faces, halving the value in alpha channel
		Pass {
			Cull Front
			Blend DstColor Zero
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			ENDCG
		
			SetTexture[_MainTex] { constantColor(0.5,0.5,0.5,0.5) combine constant }
			
		}
	} 

	FallBack Off
}
