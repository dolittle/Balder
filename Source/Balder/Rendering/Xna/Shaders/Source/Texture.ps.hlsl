#include "Lighting.ps.hlsl"
#include "Texture.VertexOutput"

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
	return tex2D(TextureSampler, vertex.UV).rgba;
}