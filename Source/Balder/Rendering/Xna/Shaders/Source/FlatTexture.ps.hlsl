#include "Lighting.ps.hlsl"
#include "Flat.VertexOutput"


float4 main(VertexShaderOutput vertex) : COLOR
{
	return vertex.Color;
}