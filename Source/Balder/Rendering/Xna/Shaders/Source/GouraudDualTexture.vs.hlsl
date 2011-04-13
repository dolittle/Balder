#include "Defaults.hlsl"
#include "RenderVertex.hlsl"
#include "Material.hlsl"
#include "Lighting.vs.hlsl"
#include "GouraudTexture.VertexOutput"




VertexShaderOutput main(RenderVertex vertex)
{
	VertexShaderOutput output;

	float4 position = mul(float4(vertex.Position,1), WorldViewProj);
	float4 normal = mul(vertex.Normal, WorldView);

	output.Position = position;
	output.Diffuse = CalculateDiffuse(position, normal);
	output.Specular = CalculateSpecular(position, normal);
	output.UV = vertex.UV.xy;

	return output;
}