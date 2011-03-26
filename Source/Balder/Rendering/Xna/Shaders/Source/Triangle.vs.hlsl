// transformation matrix provided by the application
float4x4 WorldViewProj : register(c0);
float4x4 InverseWorld : register(c4);
float4 LightDirection : register(c8);


// vertex input to the shader
struct VertexData
{
  float3 Position : POSITION;
  float3 Normal : NORMAL;
  float4 Color : COLOR;
  float2 UV : TEXCOORD0;
};

// vertex shader output passed through to geometry 
// processing and a pixel shader
struct VertexShaderOutput
{
  float4 Position : POSITION;
  float4 Color : COLOR;
  float2 UV : TEXCOORD0;
  float3 LightDirection : TEXCOORD1;
  float3 LightNormal : TEXCOORD2;
};

// main shader function
VertexShaderOutput main(VertexData vertex)
{
  VertexShaderOutput output;
  output.UV = vertex.UV.xy;
  output.Color = vertex.Color;

  // apply standard transformation for rendering
  output.Position = mul(float4(vertex.Position,1), WorldViewProj);
  output.LightDirection = normalize(LightDirection);
  output.LightNormal = normalize(mul(InverseWorld, vertex.Normal));
  
  return output;
}