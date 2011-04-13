struct RenderVertex
{
  float3 Position : POSITION;
  float3 Normal : NORMAL0;
  float3 FaceNormal : NORMAL1;
  float4 Color : COLOR;
  float2 UV : TEXCOORD0;
};