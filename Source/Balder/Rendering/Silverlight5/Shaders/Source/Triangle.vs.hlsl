// transformation matrix provided by the application
float4x4 WorldViewProj : register(c0);

// vertex input to the shader
struct VertexData
{
  float3 Position : POSITION;
  float4 Color : COLOR;
};

// vertex shader output passed through to geometry 
// processing and a pixel shader
struct VertexShaderOutput
{
  float4 Position : POSITION;
  float4 Color : COLOR;
};

// main shader function
VertexShaderOutput main(VertexData vertex)
{
  VertexShaderOutput output;

  // apply standard transformation for rendering
  output.Position = mul(float4(vertex.Position,1), WorldViewProj);

  // pass the color through to the next stage
  output.Color = vertex.Color;
  
  return output;
}