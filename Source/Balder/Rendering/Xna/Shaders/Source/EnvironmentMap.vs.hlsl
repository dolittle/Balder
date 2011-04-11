#include "Defaults.hlsl"
#include "RenderVertex.hlsl"
#include "EnvironmentMap.VertexOutput"
#include "EnvironmentMapping.hlsl"

VertexShaderOutput main(RenderVertex vertex)
{
	VertexShaderOutput output;
	output.Position = mul(float4(vertex.Position,1), WorldViewProj);
	output.UV = CalculateEnvironmentMapUV(vertex.Position, vertex.Normal);
	return output;
}