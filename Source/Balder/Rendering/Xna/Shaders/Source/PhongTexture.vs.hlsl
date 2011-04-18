#include "Defaults.hlsl"
#include "RenderVertex.hlsl"
#include "PhongTexture.VertexOutput"

VertexShaderOutput main(RenderVertex vertex)
{
	VertexShaderOutput output;

	float4 position = mul(float4(vertex.Position,1), WorldViewProj);
	float4 normal = mul(vertex.Normal, WorldView);
	output.Position = position;
	output.Normal = normal;
	output.WorldView = position;
	output.UV = vertex.UV.xy;

	return output;
}