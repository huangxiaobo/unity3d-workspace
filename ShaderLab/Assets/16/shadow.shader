Shader "ShaderLib/16/Shadow" {
	Properties {
		_Color ("Object's Color", Color) = (0,1,0,1)
		_ShadowColor ("Shadow's Color", Color) = (0,0,0,1)
	}
	SubShader {
		Pass {      
			Tags { "LightMode" = "ForwardBase" } // rendering of object
 
			CGPROGRAM
			// 此pass画出图形
 
			#pragma vertex vert 
			#pragma fragment frag
 
			// User-specified properties
			uniform float4 _Color; 

 
			float4 vert(float4 vertexPos : POSITION) : SV_POSITION 
			{
				return mul(UNITY_MATRIX_MVP, vertexPos);
			}
 
			float4 frag(void) : COLOR
			{
				return _Color; 
			}
 
			ENDCG 
		}
 
		Pass {   
			Tags { "LightMode" = "ForwardBase" } 
			// 渲染投影出的阴影
			// 此语句用两个参数（Facto和Units）来定义深度偏移。
			// Factor参数表示 Z缩放的最大斜率的值。
			// Units参数表示可分辨的最小深度缓冲区的值。
			// 于是，我们就可以强制使位于同一位置上的两个集合体中的一个几何体绘制在另一个的上层。例如偏移量Offset 设为0, -1（即Factor=0, Units=-1）的值使得靠近摄像机的几何体忽略几何体的斜率，而偏移量为-1,-1（即Factor =-1, Units=-1）时，则会让几何体偏移一个微小的角度，让观察使看起来更近些。
			Offset -1.0, -2.0 
			// 确保阴影多边形在阴影接受者的上面 make sure shadow polygons are on top of shadow receiver
 
			CGPROGRAM
			// 此pass画出阴影
 
			// 定义顶点和片段入口程序
			#pragma vertex vert 
			#pragma fragment frag
 
			#include "UnityCG.cginc"
 
			// 用户定义属性 User-specified uniforms
			uniform float4 _ShadowColor;
			uniform float4x4 _World2Receiver; // 世界坐标系到平面所在坐标系的变换信息
 
			float4 vert(float4 vertexPos : POSITION) : SV_POSITION
			{
				// 对象左边系到世界坐标系
				float4x4 modelMatrix = _Object2World;
				// 世界坐标到对象坐标系
				float4x4 modelMatrixInverse = _World2Object; 
				// 观察坐标系
				float4x4 viewMatrix = mul(UNITY_MATRIX_MV, modelMatrixInverse);
 
				// 顶点在世界坐标系中的坐标
				float4 vertexInWorldSpace = mul(modelMatrix, vertexPos);
				// 光源方向
				float4 lightDirection;
				if (0.0 != _WorldSpaceLightPos0.w) 
				{
					// 如果是点光源或者是聚光灯光源
					// 点光源的计算：直接坐标系中顶点坐标减光源坐标，然后变换到世界坐标系
					lightDirection = normalize(mul(modelMatrix, vertexPos) - _WorldSpaceLightPos0);
				}     
				else 
				{
					// 方向光源方向：从光源位置指向世界坐标原点 directional light
					lightDirection = -normalize(_WorldSpaceLightPos0); 
				}

				// 因为 _World2Receiver是通过ShadowReceiver脚本设置的
				float4 world2ReceiverRow1 = float4(_World2Receiver[1][0], _World2Receiver[1][1], _World2Receiver[1][2], _World2Receiver[1][3]);
				// 计算顶点到投影平面的距离 从原点指向顶点，所以distanceOfVertex为正
				float distanceOfVertex = dot(world2ReceiverRow1, vertexInWorldSpace); 
				// 另一种方法计算就是降世界坐标系中的点转换到投影屏幕坐标系，然后取y值
				// = (_World2Receiver * vertexInWorldSpace).y
				// = height over plane 
				// 计算光源到投影平面的距离，lightDirection是从光源执行顶点，所以lengthOfLightDirectionInY是负的
				float lengthOfLightDirectionInY = dot(world2ReceiverRow1, lightDirection); 
				// 另一种方法同上
				// = (_World2Receiver * lightDirection).y 
				// = length in y direction
 
 				// 比较顶点到投影屏幕的距离和光源到投影平面的距离
 				// lengthOfLightDirectionInY计算时，光源方向从光源指向顶点，distanceOfVertex计算时是从原点指向顶点
 				// 所以两者正负不同
				if (distanceOfVertex > 0.0 && lengthOfLightDirectionInY < 0.0)
				{
					// 顶点和光源都在投影平面上部
					// 计算顶点到投影位置的位移向量  vertexDirection / lightDirection = distanceOfVertex / (-lengthOfLightDirectionInY)
					lightDirection = lightDirection * (distanceOfVertex / (-lengthOfLightDirectionInY));
				}
				else
				{
					// 没有投影，所以顶点不需要偏移
					lightDirection = float4(0.0, 0.0, 0.0, 0.0); 
					// don't move vertex
				}
 
 				// 顶点加上偏移，就是就是最终在世界坐标系中的投影，然后经过VP变换
 				// 不需要世界矩阵变换，因为本来就在世界坐标系总
				return mul(UNITY_MATRIX_VP, vertexInWorldSpace + lightDirection);
			}
 
			float4 frag(void) : COLOR 
			{
				// 阴影使用这种颜色，不产生阴影的地方也是使用这种颜色，没有被光照到的对象也被使用这种颜色渲染了
				return _ShadowColor;
			}
 
			ENDCG 
		}
	}
}
