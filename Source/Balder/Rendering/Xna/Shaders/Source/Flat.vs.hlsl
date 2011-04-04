#include "RenderVertex.hlsl"
#include "Material.hlsl"
#include "Flat.VertexOutput"
#include "Lighting.vs.hlsl"


float4x4 WorldViewProj : register(c0);

VertexShaderOutput main(RenderVertex vertex)
{
	VertexShaderOutput output;

	output.Position = mul(float4(vertex.Position,1), WorldViewProj);
	output.Color = CalculateLighting(vertex);

	return output;
}