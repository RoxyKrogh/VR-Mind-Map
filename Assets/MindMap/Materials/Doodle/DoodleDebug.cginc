			#pragma vertex vert
			#pragma fragment frag
			#pragma geometry geom

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float4 tangent : TANGENT;
			};

			struct v2g
			{
				float4 vertex : SV_POSITION;
				float3 normal : NORMAL;
				float4 tangent : TANGENT;
			};

			struct g2f
			{
				float4 vertex : SV_POSITION;
			};

			fixed4 _LineColor;
			float _Thickness;

// If OVERRIDE_TRANSFORM keyword is enabled, use a custom model matrix
#ifdef OVERRIDE_TRANSFORM
			float4x4 _ModelMatrix;
			float4x4 _ModelMatrix_IT;
#else
// else use built-in Unity matrices
#define _ModelMatrix UNITY_MATRIX_M
#define _ModelMatrix_IT unity_ObjectToWorld
#endif

			v2g vert(appdata v)
			{
				v2g o;
				o.vertex = mul(_ModelMatrix, v.vertex); // vertex from local to world space
				o.normal = normalize(mul(_ModelMatrix_IT, v.normal)); // normal from local to world space
				float4 tang = v.tangent;
				tang.w = 1.0;
				tang.xyz += v.vertex.xyz;
				o.tangent = mul(_ModelMatrix, tang); // tangent from local to world space
				return o;
			}

			[maxvertexcount(16)]
			void geom(line v2g input[2], inout LineStream<g2f> outstream)
			{
				// This macro is used to output a quad, as a triangle strip (1 quad = 2 primitives = 4 vertices)
#define OUT_TRI(vert1, vert2, vert3, vert4) {																				\
					out1.vertex = mul(UNITY_MATRIX_VP, vert1);	/* point 1	*/												\
					out2.vertex = mul(UNITY_MATRIX_VP, vert2);	/* point 2	*/												\
					out3.vertex = mul(UNITY_MATRIX_VP, vert3);	/* point 3	*/												\
					out4.vertex = mul(UNITY_MATRIX_VP, vert4);	/* point 4	*/												\
					float4 normal = float4(normalize(cross((vert2 - vert1).xyz, (vert3 - vert1).xyz)) * 0.003,0.0);			\
					outstream.Append(out1);																					\
					out5.vertex = mul(UNITY_MATRIX_VP, vert1 + normal);														\
					outstream.Append(out5);																					\
					outstream.RestartStrip();	/* end the triangle strip, to keep edges sharp */							\
					outstream.Append(out2);																					\
					out5.vertex = mul(UNITY_MATRIX_VP, vert2 + normal);														\
					outstream.Append(out5);																					\
					outstream.RestartStrip();	/* end the triangle strip, to keep edges sharp */							\
					outstream.Append(out3);																					\
					out5.vertex = mul(UNITY_MATRIX_VP, vert3 + normal);														\
					outstream.Append(out5);																					\
					outstream.RestartStrip();	/* end the triangle strip, to keep edges sharp */							\
					outstream.Append(out4);																					\
					out5.vertex = mul(UNITY_MATRIX_VP, vert4 + normal);														\
					outstream.Append(out5);																					\
					outstream.RestartStrip();	/* end the triangle strip, to keep edges sharp */							\
				}

				// get the starting and ending point of the input line segment
				float4 v0 = input[0].vertex;
				float4 v1 = input[1].vertex;

				// calculate extent vectors, perpendicular to the input line segment
				float4 norm0 = float4(input[0].normal, 0);
				float4 norm1 = float4(input[1].normal, 0);
				float4 right0 = float4(cross(input[0].tangent.xyz - input[0].vertex.xyz, norm0.xyz), 0);
				float4 right1 = float4(cross(input[1].tangent.xyz - input[1].vertex.xyz, norm1.xyz), 0);

				// apply the line thickness variable to the extent vectors
				right0 *= _Thickness;
				right1 *= _Thickness;
				norm0 *= _Thickness;
				norm1 *= _Thickness;

				// generate primitives with these vertices:
				float4 point00 = v0 + right0 + norm0;
				float4 point10 = v0 + right0 - norm0;
				float4 point20 = v0 - right0 - norm0; 
				float4 point30 = v0 - right0 + norm0;
				float4 point01 = v1 + right1 + norm1; 
				float4 point11 = v1 + right1 - norm1;
				float4 point21 = v1 - right1 - norm1;
				float4 point31 = v1 - right1 + norm1;

				// declare 4 output points, to generate 1 quad at a time (1 quad = 4 vertices)
				g2f out1 = (g2f)0;
				g2f out2 = (g2f)0;
				g2f out3 = (g2f)0;
				g2f out4 = (g2f)0;
				g2f out5 = (g2f)0;

				// output 4 quads, around the line segment:
				float4 vert1 = point01;
				float4 vert2 = point00;
				float4 vert3 = point31;
				float4 vert4 = point30;
				OUT_TRI(vert1, vert3, vert2, vert4); // output a quad

				vert1 = point21;
				vert2 = point20;
				/* ( vert3/4 are shared with the previous quad ) */
				OUT_TRI(vert1, vert2, vert3, vert4); // output a quad

				/* ( vert1/2 are shared with the previous quad ) */
				vert3 = point11;
				vert4 = point10;
				OUT_TRI(vert1, vert3, vert2, vert4); // output a quad

				vert1 = point01;
				vert2 = point00;
				/* ( vert3/4 are shared with the previous quad ) */
				OUT_TRI(vert1, vert2, vert3, vert4); // output a quad
			}

			fixed4 frag(g2f i) : SV_Target
			{
				fixed4 col = float4(0,0,1,1);  // apply the line color
				return col;
			}