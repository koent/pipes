#version 420 core
layout (triangles) in;

in vertexData
{
    vec3 position;
    vec3 color;
    vec3 normal;
    float time;
} vertices[];

layout (triangle_strip, max_vertices = 3) out;

out fragmentData{
    vec3 position;
    vec3 color;
    vec3 normal;
} fragment;

uniform float time;

void main()
{
    if (vertices[0].time < time)
    {
        for (int i = 0; i < gl_in.length(); i++)
        {
            fragment.position = vertices[i].position;
            fragment.color = vertices[i].color;
            fragment.normal = vertices[i].normal;
            gl_Position = gl_in[i].gl_Position;

            EmitVertex();
        }
    }

    EndPrimitive();
}
