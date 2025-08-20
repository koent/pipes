using System;
using System.IO;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Pipes;

public class Shader : IDisposable
{
    public int Handle;

    public Shader(string vertexPath, string fragmentPath)
    {
        var vertexShader = Compile(ShaderType.VertexShader, vertexPath);
        var fragmentShader = Compile(ShaderType.FragmentShader, fragmentPath);

        Handle = GL.CreateProgram();

        GL.AttachShader(Handle, vertexShader);
        GL.AttachShader(Handle, fragmentShader);

        GL.LinkProgram(Handle);

        GL.GetProgram(Handle, GetProgramParameterName.LinkStatus, out int success);
        if (success == 0)
        {
            Console.WriteLine(GL.GetProgramInfoLog(Handle));
        }

        GL.DetachShader(Handle, vertexShader);
        GL.DetachShader(Handle, fragmentShader);
        GL.DeleteShader(vertexShader);
        GL.DeleteShader(fragmentShader);
    }

    public void Use()
    {
        GL.UseProgram(Handle);
    }

    public void SetMatrix4(string name, Matrix4 matrix)
    {
        GL.UseProgram(Handle);
        int location = GL.GetUniformLocation(Handle, name);
        GL.UniformMatrix4(location, true, ref matrix);
    }

    private static int Compile(ShaderType type, string path)
    {
        var shaderSource = File.ReadAllText(path);
        var shader = GL.CreateShader(type);
        GL.ShaderSource(shader, shaderSource);

        GL.CompileShader(shader);
        GL.GetShader(shader, ShaderParameter.CompileStatus, out int success);
        if (success == 0)
        {
            Console.WriteLine(GL.GetShaderInfoLog(shader));
        }

        return shader;
    }

    private bool DisposedValue = false;

    protected virtual void Dispose(bool disposing)
    {
        if (!DisposedValue)
        {
            GL.DeleteProgram(Handle);
            DisposedValue = true;
        }
    }

    ~Shader()
    {
        if (!DisposedValue)
        {
            Console.WriteLine("Dispose first");
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}