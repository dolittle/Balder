#include "Defaults.hlsl"
#include "Material.hlsl"
#include "Lighting.vs.hlsl"
#include "Phong.VertexOutput"


float4 main(VertexShaderOutput vertex) : COLOR
{
	float4 diffuse = CalculateDiffuseForLight(Lights[0], float4(vertex.WorldView,1), float4(vertex.Normal,1)) * CurrentMaterial.Diffuse;
	float4 specular = CalculateSpecularForLight(Lights[0], float4(vertex.WorldView,1), float4(vertex.Normal,1)) * CurrentMaterial.Specular;
	return CurrentMaterial.Ambient + float4(diffuse.xyz + specular.xyz,1);
}