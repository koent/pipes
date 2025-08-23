using System;
using System.Linq;
using OpenTK.Mathematics;
using Pipes.Extensions;

namespace Pipes.Pipes3D;

public class PipesController
{
    public const string ShaderName = "pipes3d";

    private readonly State _state = new(64000, 0.03f, 0.075f);
    private readonly Random _random = new();
    private int _turnTimer = 0;
    private float _scale = 1.0f;

    public bool OnUpdateFrame()
    {
        if (_turnTimer >= 4 && _random.NextBool(8))
        {
            if (!_state.CanTurn())
                return false;

            RandomTurn();
        }

        if (_state.OutOfBounds)
        {
            if (!_state.CanStartPipe())
                return false;

            StartRandomPipe();
        }

        _state.Step();
        _turnTimer++;

        return true;
    }

    public void Restart(float scale)
    {
        _scale = scale;
        _state.Clear(scale);
        StartRandomPipe();
    }

    public override string ToString() => $"Turn timer: {_turnTimer:D4}";

    private void StartRandomPipe()
    {
        var x = 2 * _random.NextSingle() - 1;
        var y = 2 * _random.NextSingle() - 1;
        var z = 2 * _random.NextSingle() - 1;
        Vector3 position = (x * _scale, y, z * _scale);

        var direction = position.DirectionTowardsOrigin();

        var hue = 2 * MathF.PI * _random.NextSingle();

        _state.StartPipe(direction, position, hue);
    }

    private void RandomTurn()
    {
        var bigSphere = _random.NextBool(4);

        var newDirections = _state.TurnDirections.ToList();

        var directionTowardsOrigin = _state.Postition.DirectionTowardsOrigin();

        var nearEdge = MathF.Max(MathF.Max(_state.Postition.X, _state.Postition.Z) / _scale, _state.Postition.Y) > 0.7f;

        var newDirection = newDirections.Contains(directionTowardsOrigin) && nearEdge
            ? directionTowardsOrigin
            : _random.NextFromList(newDirections);

        _state.Turn(newDirection, bigSphere);

        _turnTimer = 0;
    }

    public float[] Vertices => _state.Vertices;
    public int VertexArrayLength => _state.VertexArrayLength;
    public uint[] Indices => _state.Indices;
    public int IndexArrayLength => _state.IndexArrayLength;
    

    // bounding box
    // private float S => _scale;

    // public float[] Vertices => [
    //     -S, -1f, -S,   0f, 0f, 0f,    0f, 0f, 0f,
    //     -S, -1f,  S,   0f, 0f, 2f,    0f, 0f, 0f,
    //     -S,  1f, -S,   0f, 2f, 0f,    0f, 0f, 0f,
    //     -S,  1f,  S,   0f, 2f, 2f,    0f, 0f, 0f,
    //      S,  1f, -S,   2f, 0f, 0f,    0f, 0f, 0f,
    //      S,  1f,  S,   2f, 0f, 2f,    0f, 0f, 0f,
    //      S, -1f, -S,   2f, 2f, 0f,    0f, 0f, 0f,
    //      S, -1f,  S,   2f, 2f, 2f,    0f, 0f, 0f,
    // ];
    // public int VertexArrayLength => 8 * 9;
    // public uint[] Indices => [
    //     0, 1, 2,   1, 2, 3,
    //     2, 3, 4,   3, 4, 5,
    //     4, 5, 6,   5, 6, 7,
    //     6, 7, 0,   7, 0, 1,
    // ];
    // public int IndexArrayLength => 8 * 3;
}