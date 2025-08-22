using System;
using Pipes.Extensions;

namespace Pipes.Pipes3D;

public class Controller : IPipesController
{
    private readonly State _state = new(20000, 0.03f, 0.05f);
    private readonly Random _random = new();
    private int _turnTimer = 0;
    private float _scale = 1.0f;

    public void OnLoad()
    {
        StartRandomPipe();
    }

    public void OnUpdateFrame()
    {
        if (_turnTimer >= 8 && _random.NextBool(16))
            RandomTurn();

        if (_state.OutOfBounds)
            StartRandomPipe();

        _state.Step();
        _turnTimer++;
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
        if (!_state.CanStartPipe())
            _state.Clear();

        var x = 2 * _random.NextSingle() - 1;
        var y = 2 * _random.NextSingle() - 1;
        var z = 2 * _random.NextSingle() - 1;
        var direction = _random.NextEnum<Direction>();
        var hue = 2 * MathF.PI * _random.NextSingle();
        var depth = 0.3f * _random.NextSingle();

        _state.StartPipe(direction, (x * _scale, y, z * _scale), hue, depth);
    }

    private void RandomTurn()
    {
        if (!_state.CanTurn())
        {
            StartRandomPipe();
            return;
        }

        var turnDirection = _random.NextEnum<TurnDirection>();
        var bigSphere = _random.NextBool(4);

        _state.Turn(turnDirection, bigSphere);

        _turnTimer = 0;
    }

    public float[] Vertices => _state.Vertices;
    public int VertexArrayLength => _state.VertexArrayLength;
    public uint[] Indices => _state.Indices;
    public int IndexArrayLength => _state.IndexArrayLength;
    public string ShaderName => "pipes3d";
    

    // bounding box
    // private float S => _scale;

    // public float[] Vertices => [
    //     -S, -1f, -S,   0f, 0f, 0f,    0f, 0f, 0f,
    //     -S, -1f,  S,   0f, 0f, 1f,    0f, 0f, 0f,
    //     -S,  1f, -S,   0f, 1f, 0f,    0f, 0f, 0f,
    //     -S,  1f,  S,   0f, 1f, 1f,    0f, 0f, 0f,
    //      S,  1f, -S,   1f, 0f, 0f,    0f, 0f, 0f,
    //      S,  1f,  S,   1f, 0f, 1f,    0f, 0f, 0f,
    //      S, -1f, -S,   1f, 1f, 0f,    0f, 0f, 0f,
    //      S, -1f,  S,   1f, 1f, 1f,    0f, 0f, 0f,
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