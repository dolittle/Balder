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

float4 CalculateDiffuseForLight(Light light, float4 position, float4 normal)
{
	float3 lightDirection = normalize(light.PositionOrDirection.xyz - position);
	float directionDot = saturate(dot(normal, lightDirection));

	float lightDirRanged = lightDirection / light.Details.y;
	float attenuation = 1 - saturate(dot(lightDirRanged, lightDirRanged));

	float shadow = saturate(4*directionDot);

	float4 diffuse = (((light.Diffuse * directionDot) * light.Details.x)*shadow)*attenuation;
	return diffuse;
}

float4 CalculateSpecularForLight(Light light, float4 position, float4 normal)
{
	float3 lightDirection = normalize(light.PositionOrDirection.xyz - position);
	float directionDot = saturate(dot(normal, lightDirection));
	float3 view = normalize(ViewPosition - position);
	float3 reflection = normalize(2 * directionDot * normal - lightDirection);
	float viewDot = saturate(dot(reflection, view));

	float lightDirRanged = lightDirection / light.Details.y;
	float attenuation = 1 - saturate(dot(lightDirRanged, lightDirRanged));

	float specularPower = 
				CurrentMaterial.Details.y * 
				pow(viewDot, CurrentMaterial.Details.x);
						
	float4 specular = 
				((light.Specular * specularPower) * light.Details.x) *
				attenuation;

	return specular;
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
				float4 diffuse = CalculateDiffuseForLight(light, position, normal) * CurrentMaterial.Diffuse;
				float4 specular = CalculateSpecularForLight(light, position, normal) * CurrentMaterial.Specular;
				resultDiffuse = resultDiffuse + (diffuse + specular); // * attenuation);
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



float4 CalculateDiffuse(float4 position, float4 normal)
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
				diffuse = CalculateDiffuseForLight(light, position, normal);
				resultDiffuse = resultDiffuse + diffuse;
			}
		}
	}

	resultDiffuse = saturate(resultDiffuse);
	resultDiffuse.w = 1;
	return resultDiffuse;
}


float4 CalculateSpecular(float4 position, float4 normal)
{
	float4 result = float4(0,0,0,1);
	float4 diffuse = float4(0,0,0,0);

	for( int lightIndex=0; lightIndex<MaxLights; lightIndex++ )
	{
		Light light = Lights[lightIndex];
		
		if( light.Details.x > 0 ) 
		{
			// Omni light
			if( light.Details.z == 1 ) 
			{
				float4 specular = CalculateSpecularForLight(light, position, normal);

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



