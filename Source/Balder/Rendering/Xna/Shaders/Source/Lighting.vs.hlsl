#define MaxLights 2

// Size = 20
struct Light
{
	float4 PositionOrDirection : POSITION;
	float4 Ambient : COLOR;
	float4 Diffuse : COLOR;
	float4 Specular : COLOR;
	float4 Details; // x = strength, y = range, z = light type, w = 0
};



float4 ViewPosition : register(c17);

Light Lights[MaxLights] : register(c18);	// Size = 20 * 7 = 140


float4 CalculateDiffuseForLight(Light light, float4 position, float4 normal)
{
	float3 lightDirection = normalize(light.PositionOrDirection.xyz - position);
	float directionDot = saturate(dot(lightDirection, normal));
	
	float lightDirRanged = lightDirection / light.Details.y;
	float attenuation = 1 - saturate(dot(lightDirRanged, lightDirRanged));

	float shadow = saturate(4*directionDot);

	float4 diffuse = (((light.Diffuse * directionDot) * light.Details.x)*shadow)*attenuation;
	return diffuse;
}

/*
float4 DiffuseAndPhongPS(PixelShaderInputPerPixelDiffuse input) : COLOR
{
     //calculate per-pixel diffuse
     float3 directionToLight = normalize(lightPosition - input.WorldPosition);
     float diffuseIntensity = saturate( dot(directionToLight, input.WorldNormal));
     float4 diffuse = diffuseLightColor * diffuseIntensity;

     //calculate Phong components per-pixel
     float3 reflectionVector = normalize(reflect(-directionToLight, input.WorldNormal));
     float3 directionToCamera = normalize(cameraPosition - input.WorldPosition);
     
     //calculate specular component
     float4 specular = specularLightColor * specularIntensity * 
                       pow(saturate(dot(reflectionVector, directionToCamera)), 
                           specularPower);
      
     //all color components are summed in the pixel shader
     float4 color = specular  + diffuse + ambientLightColor;
     color.a = 1.0;
     return color;
}*/


float4 CalculateSpecularForLight(Light light, float4 position, float4 normal)
{
	float3 lightDirection = normalize(light.PositionOrDirection.xyz - position);
	float directionDot = saturate(dot(lightDirection, normal));
	
	float3 viewDirection = normalize(ViewPosition - position);

	
	float3 reflection = normalize(reflect(-lightDirection,normal));

	float viewDot = directionDot;
	//saturate(dot(reflection, viewDirection));
	float specularPower = CurrentMaterial.Details.y * pow(viewDot, CurrentMaterial.Details.x);
					
	float4 specular = light.Specular * specularPower;
	
	return specular;
}


float4 CalculateLighting(float4 position, float4 normal)
{
	float4 resultDiffuse = float4(0,0,0,1);

	for( int lightIndex=0; lightIndex<MaxLights; lightIndex++ )
	{
		Light light = Lights[lightIndex];
		if( light.Details.x > 0 ) 
		{
			// Omni light
			if( light.Details.z == 1 ) 
			{
				//resultDiffuse = light.Ambient + CalculateDiffuseForLight(light, position, normal) * CurrentMaterial.Diffuse;
				//resultDiffuse = CalculateSpecularForLight(light, position, normal) * CurrentMaterial.Specular;
				float4 diffuse = CalculateDiffuseForLight(light, position, normal) * CurrentMaterial.Diffuse;
				float4 specular = CalculateSpecularForLight(light, position, normal) * CurrentMaterial.Specular;
				resultDiffuse += light.Ambient + diffuse + specular;
			}

			// Directional light
			//if( light.Details.z == 0 ) 
			//{
				//diffuse = CalculateDirectional(vertex, normal, Lights[lightIndex]);
			//}
		}
	}

	resultDiffuse += CurrentMaterial.Ambient;
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
			//if( light.Details.z == 0 ) 
			//{
				//diffuse = CalculateDirectional(vertex, normal, Lights[lightIndex]);
			//}
		}
	}

	result = saturate(result);
	result.w = 1;
	return result;
}



