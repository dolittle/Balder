#include "Lighting.ps.hlsl"
#include "TextureEnvironmentMap.VertexOutput"

sampler2D TextureSampler = sampler_state
{
    Texture = <Texture>;
    MipFilter = Linear;
    MinFilter = Anisotropic;
    MagFilter = Anisotropic;
    AddressU = Clamp;
    AddressV = Clamp;
};

sampler2D EnvironmentMapSampler = sampler_state
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
	float4 textureTexel = tex2D(TextureSampler, vertex.UV).rgba;
	float4 envMapTexel = tex2D(EnvironmentMapSampler, vertex.EnvMapUV).rgba;
	return float4
		(
			(textureTexel.xyz * vertex.TextureFactors.x) * 
			(envMapTexel.xyz * vertex.TextureFactors.y), textureTexel.w * envMapTexel.w
		);
}