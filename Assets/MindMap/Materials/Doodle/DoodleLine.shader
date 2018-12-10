Shader "Geometry/DoodleLine"
{
	Properties
	{
		_LineColor("Color", Color) = (1,1,1,1)
		_Thickness("Thickness", float) = 0.03
	}
		SubShader
	{
		Tags { "RenderType" = "Opaque" "LightMode" = "ForwardBase" }
		LOD 100

		Pass
		{
			CULL BACK
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma geometry geom
#define OVERRIDE_TRANSFORM
#include "DoodleLine.cginc"
			ENDCG
		}
	}
}
