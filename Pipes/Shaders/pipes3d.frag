#version 420 core
in vec3 vertexColor;
in vec3 FragPos;
in vec3 Normal;

out vec4 FragColor;

uniform vec3 lightPos;
uniform vec3 viewPos;

void main()
{
    float ambientStrength = 0.2;
    float specularStrength = 0.2;

    vec3 norm = normalize(Normal);
    vec3 lightDir = normalize(lightPos - FragPos);
    float diff = max(dot(norm, lightDir), 0.0);

    vec3 viewDir = normalize(viewPos - FragPos);
    vec3 reflectDir = reflect(-lightDir, norm);
    float spec = pow(max(dot(viewDir, reflectDir), 0.0), 16);

    FragColor = vec4((ambientStrength + diff + spec) * vertexColor, 1.0);
}