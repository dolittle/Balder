#include "Lighting.ps.hlsl"
#include "Gouraud.VertexOutput"


float4 main(VertexShaderOutput vertex) : COLOR
{
	return vertex.Color;
}