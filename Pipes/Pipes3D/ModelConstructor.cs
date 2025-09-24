using System;
using System.Collections.Generic;
using OpenTK.Mathematics;
using Pipes.Extensions;
using Pipes.Structures;

namespace Pipes.Pipes3D;

public class ModelConstructor
{
    private const int SlowdownFactor = 2;
    private const int SpherePrecision = 20;
    private const int PipePrecision = 20;
    private const float Radius = 0.15f;

    private readonly List<Vertex> _vertices;
    private readonly List<(uint First, uint Second, uint Third)> _triangles;

    public void Construct(IReadOnlyCollection<PipePoint> points)
    {
        if (points.Count == 0) return;

        var time = 0;
        foreach (var point in points)
        {
            for (int i = 0; i < SlowdownFactor; i++)
                CreatePipeSegment(point, (float)i / SlowdownFactor, time++);

            if (point.JointType is not JointType.None)
                CreateSphere(point, time);
        }
    }

    public IEnumerable<float> GetVertexAttributes()
    {
        foreach (var vertex in _vertices)
            foreach (var attribute in vertex.GetAttributes())
                yield return attribute;
    }

    public IEnumerable<uint> GetIndices()
    {
        foreach (var (first, second, third) in _triangles)
        {
            yield return first;
            yield return second;
            yield return third;
        }
    }

    private void CreatePipeSegment(PipePoint point, float offset, float time)
    {
        var firstVertex = (uint)_vertices.Count;

        var startpoint = point.Position + offset * point.Direction.GetVector();
        for (int i = 0; i < PipePrecision; i++)
        {
            var normal = Matrix3.CreateFromAxisAngle(point.Direction.GetVector(), i * MathF.Tau / PipePrecision)
                * point.Direction.GetPerpendicularVector();

            var vertex = new Vertex(startpoint + Radius * normal, point.Color, normal, time);
            _vertices.Add(vertex);
        }

        var endpoint = point.Position + (offset + 1) * point.Direction.GetVector();
        for (int i = 0; i < PipePrecision; i++)
        {
            var normal = Matrix3.CreateFromAxisAngle(point.Direction.GetVector(), i * MathF.Tau / PipePrecision)
                * point.Direction.GetPerpendicularVector();

            var vertex = new Vertex(endpoint + Radius * normal, point.Color, normal, time);
            _vertices.Add(vertex);
        }

        for (uint i = 0; i < PipePrecision; i++)
        {
            var next = (i + 1) % PipePrecision;
            _triangles.Add((firstVertex + i, firstVertex + next, firstVertex + PipePrecision + i));
            _triangles.Add((firstVertex + next, firstVertex + PipePrecision + i, firstVertex + PipePrecision + next));
        }
    }

    private void CreateSphere(PipePoint point, float time)
    {
        var radius = point.JointType switch
        {
            JointType.Small => Radius,
            JointType.Big => 1.5f * Radius,
            _ => throw new ArgumentException($"{nameof(PipePoint.JointType)} not of correct type")
        };

        // https://www.songho.ca/opengl/gl_sphere.html#webgl_sphere
        var firstVertex = (uint)_vertices.Count;

        const int sectorCount = SpherePrecision;
        const int stackCount = SpherePrecision / 2;

        const float sectorStep = MathF.Tau / sectorCount;
        const float stackStep = MathF.PI / stackCount;

        for (int i = 0; i <= stackCount; i++)
        {
            var xy = MathF.Cos(MathF.PI / 2 - i * stackStep);
            var z = MathF.Sin(MathF.PI / 2 - i * stackStep);

            for (int j = 0; j <= sectorCount; j++)
            {
                var x = xy * MathF.Cos(j * sectorStep);
                var y = xy * MathF.Sin(j * sectorStep);
                var normal = new Vector3(x, y, z);
                var vertex = new Vertex(point.Position + radius * normal, point.Color, normal, time);
                _vertices.Add(vertex);
            }
        }

        for (uint i = 0; i < stackCount; i++)
        {
            uint k1 = i * (sectorCount + 1); // Beginning of current stack
            uint k2 = k1 + sectorCount + 1;  // Beginning of next stack

            for (uint j = 0; j < sectorCount; j++, k1++, k2++)
            {
                if (i != 0)
                    _triangles.Add((firstVertex + k1, firstVertex + k2, firstVertex + k1 + 1));

                if (i != (stackCount - 1))
                    _triangles.Add((firstVertex + k1 + 1, firstVertex + k2, firstVertex + k2 + 1));
            }
        }
    }
}
