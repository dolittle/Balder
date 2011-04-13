float2 CalculateEnvironmentMapUV(float3 position, float3 normal)
{
	float4 normalizedPosition = normalize(mul(float4(position,1), WorldView));
	float4 transformedNormal = mul(normal, WorldView);
	float4 reflection = reflect(transformedNormal, normalizedPosition);

	float m = 1 / length(reflection);

	float s = reflection.x * m;
	float t = reflection.y * m;

	return float2(-(s*0.5)+0.5, (t*0.5)+0.5);
}