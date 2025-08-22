#version 420 core
in vec3 vertexColor;
in vec3 FragPos;
in vec3 Normal;

out vec4 FragColor;

uniform vec3 lightPos;

void main()
{
    float ambientStrength = 0.2;

    vec3 norm = normalize(Normal);
    vec3 lightDir = normalize(lightPos - FragPos);
    float diff = max(dot(norm, lightDir), 0.0);

    FragColor = vec4((ambientStrength + diff) * vertexColor, 1.0);
}