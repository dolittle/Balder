// output from the vertex shader
struct VertexShaderOutput
{
  float4 Position : POSITION;
  float4 Color : COLOR;
  float3 LightDirection : TEXCOORD0;
  float3 LightNormal : TEXCOORD1;
};

// main shader function
float4 main(VertexShaderOutput vertex) : COLOR
{
	// Ambient light
	float Ai = 0.8f;
	float4 Ac = vertex.Color;
	//float4(0.075, 0.075, 0.2, 1.0);
	
	// Diffuse light
	float Di = 1.0f;
	float4 Dc = float4(1.0, 1.0, 1.0, 1.0);
	
	// return Ambient light * diffuse light. See tutorial if
	// you dont understand this formula
	float lightDot = dot(vertex.LightDirection, vertex.LightNormal);
	return Ai * Ac + Di * Dc * saturate(lightDot);
}