#include "Defaults.hlsl"
#include "RenderVertex.hlsl"
#include "Material.hlsl"
#include "FlatEnvironmentMap.VertexOutput"
#include "EnvironmentMapping.hlsl"
#include "Lighting.vs.hlsl"

VertexShaderOutput main(RenderVertex vertex)
{
	VertexShaderOutput output;
	float4 position = mul(float4(vertex.Position,1), WorldViewProj);
	float4 normal = mul(vertex.FaceNormal, WorldView);
	output.Position = position;
	output.Diffuse = CalculateDiffuse(position, normal);
	output.Specular = CalculateSpecular(position, normal);
	output.UV = CalculateEnvironmentMapUV(vertex.Position, vertex.Normal);
	return output;
}