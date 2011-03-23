texture Texture : register(t0);

sampler2D TextureSampler = sampler_state
{
    Texture = <Texture>;
    MipFilter = Linear;
    MinFilter = Anisotropic;
    MagFilter = Anisotropic;
    AddressU = Clamp;
    AddressV = Clamp;
};

// output from the vertex shader
struct VertexShaderOutput
{
  float4 Position : POSITION;
  float4 Color : COLOR;
  float2 UV : TEXCOORD0;
  float3 LightDirection : TEXCOORD1;
  float3 LightNormal : TEXCOORD2;
};

// main shader function
float4 main(VertexShaderOutput vertex) : COLOR
{
	float4 texel = tex2D(TextureSampler, vertex.UV).rgba;
	float lightDot = dot(vertex.LightDirection, vertex.LightNormal);

	// Ambient light
	float Ai = 0.5f;
	float4 Ac = vertex.Color;
	//float4(0.075, 0.075, 0.2, 1.0);
	
	// Diffuse light
	float Di = 1.0f;
	float4 Dc = float4(1.0, 1.0, 1.0, 1.0);
	
	

	// return Ambient light * diffuse light. See tutorial if
	// you dont understand this formula
	
	return Ai * Ac + Di * Dc * saturate(lightDot) + float4(0,0,0,1.0);
}