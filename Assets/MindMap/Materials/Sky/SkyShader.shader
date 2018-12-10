Shader "Unlit/SkyShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_SkyColor ("Sky Color", Color) = (0.25, 0.25, 1.0, 1.0)
		_HorizonColor("Horizon Color", Color) = (0.5, 0.5, 0.5, 1.0)
		_GroundColor("Ground Color", Color) = (0.25, 1.0, 0.25, 1.0)
		_SunColor("Sun Color", Color) = (1.0, 1.0, 1.0, 1.0)
	}
	SubShader
	{
		Tags { "RenderType"="Background" }
		LOD 100

		Pass
		{
			Cull Off
			ZWrite Off
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				fixed3 normal : NORMAL;
			};

			struct v2f
			{
				float3 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
				fixed3 normal : NORMAL;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;

			fixed4 _SkyColor;
			fixed4 _HorizonColor;
			fixed4 _GroundColor;
			fixed4 _SunColor;

			fixed4 skyGradient(fixed3 normal) {
				float elevation = dot(normal, float3(0.0, 1.0, 0.0));
				fixed4 col = lerp(_HorizonColor, _GroundColor, -min(elevation, 0.0));
				col = lerp(col, _SkyColor, max(0.0, elevation));
				fixed sunR = 0.005;
				fixed sunI = 1.0 / sunR;
				sunR = 1.0 - sunR;
				fixed sunniness = (max(sunR, dot(normal, _WorldSpaceLightPos0.xyz)) - sunR) * sunI;
				sunniness *= sunniness;
				col = lerp(col, _SunColor, sunniness);
				return col;
			}
			
			v2f vert (appdata v)
			{
				v2f o;
				float4 vertex = v.vertex;
				vertex.x = -vertex.x;
				vertex = mul(UNITY_MATRIX_M, vertex);
				vertex.w = 0.0;
				float4x4 m = UNITY_MATRIX_V;
				m[3][0] = 0.0;
				m[3][1] = 0.0;
				m[3][2] = 0.0;
				vertex = mul(m, vertex);
				vertex.w = 1.0;
				vertex = mul(UNITY_MATRIX_P, vertex);
				o.vertex = vertex;// UnityObjectToClipPos(vertex);
				o.normal = v.normal;
				o.normal.x = -o.normal.x;
				o.uv = v.vertex.xyz;//TRANSFORM_TEX(v.uv, _MainTex);
				o.uv.x = -o.uv.x;
				UNITY_TRANSFER_FOG(o, o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv) * skyGradient(normalize(i.uv));
				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG
		}
	}
}
