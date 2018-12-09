Shader "Hidden/Glow"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_DoodleBuffer ("Doodle Buffer", 2D) = "white" {}
		_GlowRadius ("Glow Radius", Float) = 1.0
	}
		SubShader
		{
			// No culling or depth
			Cull Off ZWrite Off ZTest Always

			Pass
			{
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag

				#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				UNITY_VERTEX_OUTPUT_STEREO
			};

			//float4x4 _WorldM;
			float4x4 _ViewM;
			float4x4 _ProjM;

			v2f vert (appdata v)
			{
				v2f o;
				float4x4 m = mul(_ViewM, UNITY_MATRIX_M);
				m = mul(_ProjM, m);

				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTex;
			sampler2D _DoodleBuffer;

			float _GlowRadius;

			fixed4 frag (v2f i) : SV_Target
			{
				float2 uv = UnityStereoTransformScreenSpaceTex(i.uv);

				fixed4 col = tex2D(_MainTex, i.uv);
				fixed4 col0 = tex2D(_DoodleBuffer, uv);

				float2 smplRad = float2(_GlowRadius, _GlowRadius) / _ScreenParams.xy;
				fixed4 col1 = tex2D(_DoodleBuffer, uv + float2(smplRad.x, 0));
				col1 += tex2D(_DoodleBuffer, uv - float2(smplRad.x, 0));
				col1 += tex2D(_DoodleBuffer, uv + float2(0, smplRad.y));
				col1 += tex2D(_DoodleBuffer, uv - float2(0, smplRad.y));
				col1 += tex2D(_DoodleBuffer, uv + smplRad);
				col1 += tex2D(_DoodleBuffer, uv - smplRad);
				col1 += tex2D(_DoodleBuffer, uv + float2(-smplRad.x, smplRad.y));
				col1 += tex2D(_DoodleBuffer, uv + float2(smplRad.x, -smplRad.y));
				col1 /= 10.0;
				
				smplRad += smplRad;
				fixed4 col2 = tex2D(_DoodleBuffer, uv + float2(smplRad.x, 0));
				col2 += tex2D(_DoodleBuffer, uv - float2(smplRad.x, 0));
				col2 += tex2D(_DoodleBuffer, uv + float2(0, smplRad.y));
				col2 += tex2D(_DoodleBuffer, uv - float2(0, smplRad.y));
				col2 += tex2D(_DoodleBuffer, uv + smplRad);
				col2 += tex2D(_DoodleBuffer, uv - smplRad);
				col2 += tex2D(_DoodleBuffer, uv + float2(-smplRad.x, smplRad.y));
				col2 += tex2D(_DoodleBuffer, uv + float2(smplRad.x, -smplRad.y));
				col2 /= 12.0;

				fixed3 white = fixed3(1, 1, 1);
				float brightness = max(0.0, dot(col0.xyz, white));
				brightness = max(brightness, dot(col1.xyz, white));
				brightness = max(brightness, dot(col2.xyz, white));
				brightness *= abs(brightness);
				
				return lerp(col, fixed4(white, 1.0), brightness);
			}
			ENDCG
		}
	}
}
