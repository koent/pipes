#version 420 core
in vec3 vertexColor;
in vec3 FragPos;
in vec3 Normal;

out vec4 FragColor;

uniform vec3 lightDir;
uniform vec3 viewPos;

void main()
{
    float ambientStrength = 0.2;
    float diffuseStrength = 0.7;
    float specularStrength = 0.8;

    float ambient = ambientStrength;

    vec3 norm = normalize(Normal);
    float diffuse = diffuseStrength * max(dot(norm, lightDir), 0.0);

    vec3 viewDir = normalize(viewPos - FragPos);
    vec3 reflectDir = reflect(-lightDir, norm);
    float specular = specularStrength * pow(max(dot(viewDir, reflectDir), 0.0), 16);

    FragColor = vec4((ambient + diffuse + specular) * vertexColor, 1.0);
}