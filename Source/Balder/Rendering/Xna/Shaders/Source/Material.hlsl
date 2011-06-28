
// Size = 4 registers

struct Material
{
	float4 Ambient : COLOR;
	float4 Diffuse : COLOR;
	float4 Specular : COLOR;
	float4 Details;	// x = Glossiness, y = Specular level, z = Opacity, w = nothing
	float4 MapOpacities; // x = DiffuseMap, y = ReflectionMap, z = nothing, w = nothing
};

Material CurrentMaterial : register(c12);