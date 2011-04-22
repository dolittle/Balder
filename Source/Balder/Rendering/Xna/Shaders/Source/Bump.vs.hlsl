#include "Defaults.hlsl"
#include "RenderVertex.hlsl"
#include "Bump.VertexOutput"


VertexShaderOutput main(RenderVertex vertex)
{
	VertexShaderOutput output;

	output.Position = mul(float4(vertex.Position,1), WorldViewProj);
	output.UV = vertex.UV.xy;

	float3 normal = vertex.Normal;
	float3 tangent = cross(float3(0.0,1.0,0.0), normal);
	float3 binormal = cross(tangent, normal);

    output.WorldNormal = mul(float4(normal,0), WorldView).xyz;
    output.WorldTangent = mul(float4(tangent,0), WorldView).xyz;
    output.WorldBinormal = mul(float4(binormal,0), WorldView).xyz;

	return output;
}