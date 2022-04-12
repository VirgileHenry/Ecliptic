#version 330

struct Material
{
	vec3 ambiant;
	vec3 diffuse;
	vec3 specular;
	float shininess;
	bool emissive;
};

struct Light
{
	vec3 position;
	vec4 color;
	vec4 ambiant;
};

uniform vec3 cameraPosition;
uniform Light light;
uniform Material material;

in vec3 FragPos;  
flat in vec3 Normal; //flat key word mean the value is not interpolated

out vec4 FragColor;

void main()
{
	if(material.emissive)
	{
		FragColor = vec4(material.ambiant, 1.0f);
	}
	else
	{
		vec3 norm = normalize(Normal);
		vec3 lightDir = normalize(light.position - FragPos);
		vec3 viewDir = normalize(cameraPosition - FragPos);
	
		//ambiant lighting
		vec3 ambiant = light.ambiant.xyz * material.ambiant;
		
		//diffuse
		float diff = pow(min(max(dot(norm, lightDir), 0.0), 1.0), 2);
		vec3 diffuse = diff * light.color.xyz;
		
		//specular
		vec3 specular = vec3(0);
		vec3 reflectDir = reflect(-lightDir, norm);
		float spec = pow(max(dot(viewDir, reflectDir), 0), material.shininess);
		specular = light.color.xyz * spec * material.specular;
	
		
	
		//add all the lightings
		vec3 result = (ambiant + diffuse + specular);
		FragColor = vec4(result, 1.0);
	}
}