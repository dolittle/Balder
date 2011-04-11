#define MaxLights 5

// Size = 20
struct Light
{
	float4 PositionOrDirection : POSITION;
	float4 Ambient : COLOR;
	float4 Diffuse : COLOR;
	float4 Specular : COLOR;
	float4 Details; // x = strength, y = range, z = light type, w = 0
};



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

float4 CalculateLighting(float4 position, float4 normal)
{
	float4 resultDiffuse = float4(0,0,0,1);
	float4 diffuse = float4(0,0,0,0);

	for( int lightIndex=0; lightIndex<MaxLights; lightIndex++ )
	{
		Light light = Lights[lightIndex];
		if( light.Details.x > 0 ) 
		{
			// Omni light
			if( light.Details.z == 1 ) 
			{
				float3 lightDirection = normalize(light.PositionOrDirection.xyz - position);
				float directionDot = saturate(dot(normal, lightDirection));

				float3 view = normalize(ViewPosition - position);
				float3 reflection = normalize(2 * directionDot * normal - lightDirection);

				float viewDot = saturate(dot(reflection, view));

				float specularPower = 
							CurrentMaterial.Details.y * 
							pow(viewDot, CurrentMaterial.Details.x);
						
				float4 specular = 
							((light.Specular * specularPower) * light.Details.x) *
							CurrentMaterial.Specular;

				float lightDirRanged = lightDirection / light.Details.y;
				float attenuation = 1 - saturate(dot(lightDirRanged, lightDirRanged));

				float shadow = saturate(4*directionDot);

				diffuse = (((light.Diffuse * directionDot) * light.Details.x)*CurrentMaterial.Diffuse)*shadow;
				resultDiffuse = resultDiffuse + ((diffuse + specular) * attenuation);
			}

			// Directional light
			if( light.Details.z == 0 ) 
			{
			
				//diffuse = CalculateDirectional(vertex, normal, Lights[lightIndex]);
			}
		}
	}

	resultDiffuse = saturate(resultDiffuse);
	resultDiffuse.w = 1;
	return resultDiffuse;
}

float4 CalculateDiffuse(RenderVertex vertex)
{
	float4 resultDiffuse = float4(0,0,0,1);
	float4 diffuse = float4(0,0,0,0);

	float4 position = mul(float4(vertex.Position,1), WorldViewProj);
	float4 normal = mul(vertex.Normal, WorldView);
	

	for( int lightIndex=0; lightIndex<MaxLights; lightIndex++ )
	{
		Light light = Lights[lightIndex];
		
		if( Lights[lightIndex].Details.x > 0 ) 
		{
			// Omni light
			if( Lights[lightIndex].Details.z == 1 ) 
			{
				float3 lightDirection = normalize(light.PositionOrDirection.xyz - position);
				float directionDot = dot(lightDirection, normal);
				diffuse = saturate((light.Diffuse * directionDot)*light.Details.x);

				resultDiffuse = resultDiffuse + saturate(diffuse);
			}
		}
	}

	resultDiffuse = saturate(resultDiffuse);
	resultDiffuse.w = 1;
	return resultDiffuse;
}


float4 CalculateSpecular(RenderVertex vertex)
{
	float4 result = float4(0,0,0,1);
	float4 diffuse = float4(0,0,0,0);

	float4 position = mul(float4(vertex.Position,1), WorldViewProj);
	float4 normal = mul(vertex.Normal, WorldView);
	

	for( int lightIndex=0; lightIndex<MaxLights; lightIndex++ )
	{
		Light light = Lights[lightIndex];
		
		if( Lights[lightIndex].Details.x > 0 ) 
		{
			// Omni light
			if( Lights[lightIndex].Details.z == 1 ) 
			{
				float3 lightDirection = normalize(light.PositionOrDirection.xyz - position);

				float3 reflection = normalize(position * (normal - lightDirection));

				float3 view = normalize(ViewPosition- position);

				float viewDot = saturate(dot(reflection, view));

				float specularPower = saturate(CurrentMaterial.Details.y * pow(viewDot, CurrentMaterial.Details.x));
				float4 specular = saturate(((light.Specular * specularPower) * light.Details.x)*CurrentMaterial.Specular);

				result = result + specular;
			}

			// Directional light
			if( light.Details.z == 0 ) 
			{
				//diffuse = CalculateDirectional(vertex, normal, Lights[lightIndex]);
			}

			
		}
	}

	result = saturate(result);
	result.w = 1;
	return result;
}



