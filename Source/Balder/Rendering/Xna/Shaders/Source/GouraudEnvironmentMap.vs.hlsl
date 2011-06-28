#include "Defaults.hlsl"
#include "RenderVertex.hlsl"
#include "Material.hlsl"
#include "GouraudEnvironmentMap.VertexOutput"
#include "EnvironmentMapping.hlsl"
#include "Lighting.vs.hlsl"

VertexShaderOutput main(RenderVertex vertex)
{
	VertexShaderOutput output;
	float4 position = mul(float4(vertex.Position,1), WorldViewProj);
	float4 normal = mul(vertex.Normal, World);
	float4 worldPosition = mul(float4(vertex.Position,1), World);
	worldPosition = worldPosition / worldPosition.w;
	output.Position = position;
	output.Diffuse = CalculateDiffuse(worldPosition, normal);
	output.Specular = CalculateSpecular(worldPosition, normal);
	output.UV = CalculateEnvironmentMapUV(vertex.Position, vertex.Normal);
	return output;
}