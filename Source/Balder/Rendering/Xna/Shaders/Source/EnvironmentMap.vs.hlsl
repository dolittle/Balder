#include "Defaults.hlsl"
#include "RenderVertex.hlsl"
#include "EnvironmentMap.VertexOutput"


VertexShaderOutput main(RenderVertex vertex)
{
	VertexShaderOutput output;

	/*
			var u = vertex.TransformedVectorNormalized;
			var n = vertex.TransformedNormal;
			var r = Vector.Reflect(n, u);
			var m = MathHelper.Sqrt((r.X * r.X) + (r.Y * r.Y) +
									((r.Z + 0f) * (r.Z + 0f)));

			var m1 = 1f / m;
			var s = (r.X * m1);
			var t = (r.Y * m1);

			if (null == Texture2)
			{
				vertex.U1 = -(s * 0.5f) + 0.5f;
				vertex.V1 = (t * 0.5f) + 0.5f;

			}
			else
			{
				vertex.U2 = -(s * 0.5f) + 0.5f;
				vertex.V2 = (t * 0.5f) + 0.5f;

			}
	*/

	float4 position = mul(float4(vertex.Position,1), WorldViewProj);
	float4 normalizedPosition = normalize(position);
	float4 transformedNormal = mul(float4(vertex.Normal,1), WorldViewProj);
	float4 reflection = reflect(transformedNormal, normalizedPosition);

	float m = 1 / sqrt(
					(reflection.x * reflection.x) +
					(reflection.y * reflection.y) +
					(reflection.z * reflection.z)
				  );

	output.Position = position;
	output.UV = float2(reflection.x * m, reflection.y * m);
	return output;
}