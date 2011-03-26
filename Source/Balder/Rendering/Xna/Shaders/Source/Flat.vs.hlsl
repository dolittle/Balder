#include "Lighting.vs.hlsl"
#include "Flat.VertexOutput"
#include "RenderVertex.hlsl"


float4x4 WorldViewProj : register(c0);

VertexShaderOutput main(RenderVertex vertex)
{
	VertexShaderOutput output;

	output.Position = mul(float4(vertex.Position,1), WorldViewProj);
	output.Color = vertex.Color;

	return output;
}