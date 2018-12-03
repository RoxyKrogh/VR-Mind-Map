#pragma vertex vert
#pragma fragment frag
// make fog work
#pragma multi_compile_fog

#include "UnityCG.cginc"
#include "Lighting.cginc"

struct appdata
{
	float4 vertex : POSITION;
	float2 uv : TEXCOORD0;
	float3 normal : NORMAL;
};

struct v2f
{
	float2 uv : TEXCOORD0;
	UNITY_FOG_COORDS(1)
	float4 vertex : SV_POSITION;
	float3 normal : NORMAL;
};

sampler2D _MainTex;
float4 _MainTex_ST;

fixed4 _Color;
fixed _EdgeColor;
fixed _TextureOpacity;
fixed _LightAlphaWeight;

float4x4 _WorldMatrix;
// override _WorldMatrix with the default transform, to disable it
#define _WorldMatrix UNITY_MATRIX_M
// use a macro to reference the custom world transform, to make it easier to change
#define WORLDMATRIX _WorldMatrix

#ifndef FINAL_ALPHA_MULTIPLIER
#define FINAL_ALPHA_MULTIPLIER 1.0
#endif

#ifdef EDGE_WEIGHT
fixed EDGE_WEIGHT;
#else
fixed _EdgeWeight;
#define EDGE_WEIGHT _EdgeWeight
#endif

v2f vert(appdata v)
{
	float4x4 mvp = mul(UNITY_MATRIX_VP, WORLDMATRIX);
	v2f o;
	o.vertex = mul(mvp, v.vertex);	// local to projection vertex position
	o.uv = TRANSFORM_TEX(v.uv, _MainTex);
	o.normal = normalize(mul(WORLDMATRIX, v.normal)); // local to world normal
	UNITY_TRANSFER_FOG(o, o.vertex);
	return o;
}

fixed4 frag(v2f i) : SV_Target
{
	// apply the bubble color and lighting color
	float4 col = _Color * _LightColor0;

	float3 worldNormal = i.normal;	// get world normal from vertex shader
	
#ifndef FORWARD_ALPHA_DISABLED
	// make pixels with normal vectors facing the camera more transparent
	float3 worldLookDir = normalize(mul(WORLDMATRIX, UNITY_MATRIX_IT_MV[2].xyz));
	float a = abs(dot(worldLookDir, worldNormal));
	a = 1.0 - (a * a);
	col.a = a;
#endif

	// apply diffuse lighting
	float light = dot(_WorldSpaceLightPos0.xyz, worldNormal);
	light = clamp(light, 0.1, 1.0);
	float3 lcol = fixed3(1, 1, 1) * light;
	col.rgb = lerp(col.rgb, lcol, 1.0 - (col.a * col.a * _EdgeColor));

	col.a = clamp(col.a * EDGE_WEIGHT, 0.0, 1.0);

	float4 texcol = tex2D(_MainTex, i.uv);
	col.rgb *= texcol.rgb;
	col.a = lerp(col.a * texcol.a, texcol.a, _TextureOpacity);

#ifndef FORWARD_ALPHA_DISABLED
	// make pixels with greater light levels slightly more opaque
	col.a = (col.a * (1.0 - _LightAlphaWeight)) + (light * light * 0.5 * _LightAlphaWeight);
#endif

	col.a = clamp(col.a * FINAL_ALPHA_MULTIPLIER, 0.0, 1.0);

	// apply fog
	UNITY_APPLY_FOG(i.fogCoord, col);
	return col;
}
