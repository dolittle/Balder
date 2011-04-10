#include "Defaults.hlsl"
#include "RenderVertex.hlsl"
#include "Texture.VertexOutput"


VertexShaderOutput main(RenderVertex vertex)
{
	VertexShaderOutput output;

	output.Position = mul(float4(vertex.Position,1), WorldViewProj);
	output.UV = vertex.UV.xy;

	return output;
}