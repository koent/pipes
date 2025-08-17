using System;
using Pipes.Extensions;

namespace Pipes.Pipes2D;

public class Controller
{
    private readonly State _state = new(1000, 0.02f, 0.05f);
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
        var direction = _random.NextEnum<Direction>();
        var hue = 2 * MathF.PI * _random.NextSingle();

        _state.StartPipe(direction, (x / _scale, y), hue);
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
}