#include "Defaults.hlsl"
#include "RenderVertex.hlsl"
#include "Material.hlsl"
#include "Lighting.vs.hlsl"
#include "GouraudTexture.VertexOutput"




VertexShaderOutput main(RenderVertex vertex)
{
	VertexShaderOutput output;

	output.Position = mul(float4(vertex.Position,1), WorldViewProj);
	//output.Color = CalculateLighting(vertex);
	output.Specular = CalculateSpecular(vertex);
	output.UV = vertex.UV.xy;

	return output;
}