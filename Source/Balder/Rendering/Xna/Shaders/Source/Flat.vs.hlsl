#include "Defaults.hlsl"
#include "RenderVertex.hlsl"
#include "Material.hlsl"
#include "Flat.VertexOutput"
#include "Lighting.vs.hlsl"




VertexShaderOutput main(RenderVertex vertex)
{
	VertexShaderOutput output;

	float4 position = mul(float4(vertex.Position,1), WorldViewProj);
	float4 normal = mul(vertex.Normal, World);
	float4 worldPosition = mul(float4(vertex.Position,1), World);
	worldPosition = worldPosition / worldPosition.w;
	output.Color = CalculateLighting(worldPosition, normal);
	output.Position = position;

	return output;
}