#define MaxLights 7

// Size = 20
struct Light
{
	float4 PositionOrDirection : POSITION;
	float4 Ambient : COLOR;
	float4 Diffuse : COLOR;
	float4 Specular : COLOR;
	float4 Details; // x = strength, y = range, z = light type
};

float4x4 InverseWorld : register(c4);
float4 ViewPosition : register(c8);
Material CurrentMaterial : register(c9);
Light Lights[MaxLights] : register(c13);	// Size = 20 * 7 = 140

float4 CalculateDirectional(RenderVertex vertex, float4 normal, Light light)
{
	return vertex.Color;
}

float4 CalculateOmni(RenderVertex vertex, float4 normal, Light light) : COLOR
{
	return vertex.Color;
}

float4 CalculateLighting(RenderVertex vertex)
{
	float3 normal = normalize(vertex.Normal).xyz;
	float4 resultDiffuse = float4(1,0,0,1);
	float4 diffuse = float4(0,0,0,0);

	for( int lightIndex=0; lightIndex<MaxLights; lightIndex++ )
	{
		Light light = Lights[lightIndex];

		if( Lights[lightIndex].Details.x > 0 ) 
		{
			// Omni light
			if( Lights[lightIndex].Details.z == 0 ) 
			{
				//diffuse = CalculateOmni(vertex, normal, Lights[lightIndex]);
				float3 direction = light.PositionOrDirection.xyz - vertex.Position;
				float3 normalizedDirection = normalize(direction); 

				float normalDot = dot(direction, normal);
				diffuse = (light.Diffuse * normalDot)*light.Details.x;

				float3 reflection = normalize(2 * normalDot * (normal - direction));

				float3 view = normalize(ViewPosition - vertex.Position);

				float3 viewDot = dot(reflection,view);

			}

			// Directional light
			if( Lights[lightIndex].Details.z == 1 ) 
			{
				//diffuse = CalculateDirectional(vertex, normal, Lights[lightIndex]);
			}

			resultDiffuse = resultDiffuse + diffuse;
		}
	}

	resultDiffuse.w = 1;
	return resultDiffuse;
}


