#include "Lighting.ps.hlsl"
#include "Bump.VertexOutput"

texture NormalTexture : register(t2);
sampler2D NormalSampler = sampler_state {
    Texture = <NormalTexture>;
    MipFilter = Linear;
    MinFilter = Anisotropic;
    MagFilter = Anisotropic;
    AddressU = Clamp;
    AddressV = Clamp;
};



float4 main(VertexShaderOutput vertex) : COLOR
{
	return tex2D(NormalSampler, vertex.UV).rgba;
}