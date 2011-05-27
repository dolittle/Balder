#include "Defaults.hlsl"
#include "RenderVertex.hlsl"
#include "Material.hlsl"
#include "TextureEnvironmentMap.VertexOutput"
#include "EnvironmentMapping.hlsl"

VertexShaderOutput main(RenderVertex vertex)
{
	VertexShaderOutput output;
	output.Position = mul(float4(vertex.Position,1), WorldViewProj);
	output.UV = vertex.UV.xy;
	output.EnvMapUV = CalculateEnvironmentMapUV(vertex.Position, vertex.Normal);
	output.TextureFactors = float2(CurrentMaterial.MapOpacities.x,CurrentMaterial.MapOpacities.y);
	return output;
}