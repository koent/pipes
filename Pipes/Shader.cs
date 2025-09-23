using System;
using System.IO;
using System.Reflection;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Pipes;

public class Shader : IDisposable
{
    private readonly Assembly _assembly = typeof(Shader).Assembly;
    public int Handle;

    public Shader(string shaderName)
    {
        var vertexShader = Compile(ShaderType.VertexShader, $"Pipes.Shaders.{shaderName}.vert");
        var geometryShader = Compile(ShaderType.GeometryShader, $"Pipes.Shaders.{shaderName}.geom");
        var fragmentShader = Compile(ShaderType.FragmentShader, $"Pipes.Shaders.{shaderName}.frag");

        Handle = GL.CreateProgram();

        GL.AttachShader(Handle, vertexShader);
        GL.AttachShader(Handle, geometryShader);
        GL.AttachShader(Handle, fragmentShader);

        GL.LinkProgram(Handle);

        GL.GetProgram(Handle, GetProgramParameterName.LinkStatus, out int success);
        if (success == 0)
        {
            Console.WriteLine(GL.GetProgramInfoLog(Handle));
        }

        GL.DetachShader(Handle, vertexShader);
        GL.DetachShader(Handle, geometryShader);
        GL.DetachShader(Handle, fragmentShader);
        GL.DeleteShader(vertexShader);
        GL.DeleteShader(geometryShader);
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

    public void SetVector3(string name, Vector3 vector)
    {
        GL.UseProgram(Handle);
        int location = GL.GetUniformLocation(Handle, name);
        GL.Uniform3(location, ref vector);
    }

    private int Compile(ShaderType type, string resourceName)
    {
        var stream = _assembly.GetManifestResourceStream(resourceName);
        using var reader = new StreamReader(stream);
        var shaderSource = reader.ReadToEnd();
        
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