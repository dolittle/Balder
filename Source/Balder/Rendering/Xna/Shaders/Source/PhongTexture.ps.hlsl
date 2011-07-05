#include "Defaults.hlsl"
#include "Material.hlsl"
#include "Lighting.vs.hlsl"
#include "PhongTexture.VertexOutput"

sampler2D TextureSampler = sampler_state
{
    Texture = <Texture>;
    MipFilter = Linear;
    MinFilter = Anisotropic;
    MagFilter = Anisotropic;
    AddressU = Clamp;
    AddressV = Clamp;
};


float4 main(VertexShaderOutput vertex) : COLOR
{
	float4 texel = tex2D(TextureSampler, vertex.UV).rgba;

	float4 diffuse = CalculateDiffuseForLight(Lights[0], float4(vertex.WorldView,1), float4(vertex.Normal,1));
	float4 specular = CalculateSpecularForLight(Lights[0], float4(vertex.WorldView,1), float4(vertex.Normal,1)) * CurrentMaterial.Specular;
	return CurrentMaterial.Ambient + float4((diffuse * texel).xyz + specular.xyz,1);
	//return float4(texel.xyz + specular.xyz,1);
}