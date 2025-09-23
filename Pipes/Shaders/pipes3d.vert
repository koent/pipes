#version 420 core
layout (location = 0) in vec3 aPosition;
layout (location = 1) in vec3 aColor;
layout (location = 2) in vec3 aNormal;
layout (location = 3) in float aTime;

out vertexData
{
    vec3 position;
    vec3 color;
    vec3 normal;
    float time;
} vertex;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;
uniform mat4 normalModel;

void main()
{
    gl_Position = vec4(aPosition, 1.0) * model * view * projection;

    vertex.position = vec3(model * vec4(aPosition, 1.0));
    vertex.color = aColor;
    vertex.normal = aNormal * mat3(normalModel);
    vertex.time = aTime;
}
