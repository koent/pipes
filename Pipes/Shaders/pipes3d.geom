#version 420 core
layout (triangles) in;

in vertexData
{
    vec3 color;
    vec3 normal;
    vec3 position;
} vertices[];

layout (triangle_strip, max_vertices = 3) out;

out fragmentData{
    vec3 color;
    vec3 normal;
    vec3 position;
} fragment;

void main()
{
    for (int i = 0; i < gl_in.length(); i++)
    {
        fragment.color = vertices[i].color;
        fragment.normal = vertices[i].normal;
        fragment.position = vertices[i].position;
        gl_Position = gl_in[i].gl_Position;

        EmitVertex();
    }

    EndPrimitive();
}
