#include "Defaults.hlsl"
#include "RenderVertex.hlsl"
#include "Material.hlsl"
#include "Flat.VertexOutput"
#include "Lighting.vs.hlsl"




VertexShaderOutput main(RenderVertex vertex)
{
	VertexShaderOutput output;

	float4 position = mul(float4(vertex.Position,1), WorldViewProj);
	float4 normal = mul(vertex.FaceNormal, WorldView);
	output.Position = position;
	output.Color = CalculateLighting(position, normal);

	return output;
}