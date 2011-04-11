#include "Lighting.ps.hlsl"
#include "FlatTexture.VertexOutput"

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

	return (vertex.Diffuse*texel) + vertex.Specular;
}