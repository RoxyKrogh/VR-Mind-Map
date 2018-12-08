Shader "MindMap/UnityBubble"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_TextureOpacity("Texture Opacity", Range(0.0, 1.0)) = 0.0
		_Color ("Color", Color) = (0,0,1,1)
		_EdgeColor ("Color Amount", Range(0.0, 1.0)) = 0.7
		_LightAlphaWeight ("Lighting Opacity", Range(0.0, 1.0)) = 0.2
		_EdgeWeight ("Edge Weight", Range(1, 8)) = 1
		_InnerEdgeWeight ("Inner Edge Weight", Range(1, 8)) = 6
	}
	SubShader
	{
		Tags { "Queue" = "Transparent" "RenderType"="Transparent" "LightMode"="ForwardBase" }
		LOD 100
		Blend SrcAlpha OneMinusSrcAlpha
		
		Pass
		{	
			Name "Back"
			CULL FRONT

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			//#define FORWARD_ALPHA_DISABLED
			#define FINAL_ALPHA_MULTIPLIER 1.0
			#define EDGE_WEIGHT _InnerEdgeWeight
			#include "BubblePass.cginc"
			ENDCG
		}

		Pass
		{	
			Name "Front"
			CULL BACK

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#define FINAL_ALPHA_MULTIPLIER 1.0
			#include "BubblePass.cginc"
			ENDCG
		}
	}
}