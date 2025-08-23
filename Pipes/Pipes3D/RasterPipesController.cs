using System;
using System.Linq;
using OpenTK.Mathematics;
using Pipes.Extensions;
using Pipes.Structures;

namespace Pipes.Pipes3D;

public class RasterPipesController
{
    private const int XZ = 19;
    private const int Y = 11;

    private const int SlowdownFactor = 2;
    private const float Speed = 1.0f / SlowdownFactor;

    private readonly RasterState _state = new(XZ, Y);
    private readonly Random _random = new();

    private int _time = 0;

    public bool OnUpdateFrame()
    {
        // Continue pipe that is in progress
        if (_time % SlowdownFactor != 0)
        {
            _state.PerformPartialStep(Speed);
            _time++;
            return true;
        }

        _time++;

        // If pipe has length >= 256, 1/128 Probability to start a new pipe
        if (_time >= SlowdownFactor * 256 && _random.NextBool(128))
        {
            if (!_state.CanStartPipe())
                return false;

            return TryStartNewPipe();
        }

        // 3/4 Probability to step forward, if possible
        if (!_random.NextBool(4) && _state.CanStep())
        {
            _state.Step(Speed);
            return true;
        }

        // Otherwise, try to turn
        if (!_state.CanTurn())
            return false;

        var newDirections = _state.NewDirections().ToList();
        if (_state.Started && newDirections.Count > 0)
        {
            var newDirection = _random.NextFromList(newDirections);
            var bigSphere = _random.NextBool(8);

            _state.Turn(newDirection, bigSphere);
        }

        // Then step forwards
        if (_state.CanStep())
        {
            _state.Step(Speed);
            return true;
        }

        // Otherwise, try to start a new pipe
        if (!_state.CanStartPipe())
            return false;

        if (TryStartNewPipe())
            return true;

        // Could not find a place to start a new pipe
        return false;
    }

    public void Restart()
    {
        _state.Clear();

        TryStartNewPipe();
    }

    public bool TryStartNewPipe()
    {
        var hue = 2 * MathF.PI * _random.NextSingle();
        for (int _ = 0; _ <= 16; _++)
        {
            var position = _random.NextVector3i(new Vector3i(XZ, Y, XZ));
            var direction = _random.NextEnum<Direction>();

            if (_state.CanStartNewPipeFrom(position, direction))
            {
                _state.StartPipe(direction, position, hue);
                if (_state.CanStep())
                {
                    _state.Step(Speed);
                    _time = 1;
                    return true;
                }
                return false;
            }
        }

        return false;
    }

    public float[] Vertices => _state.Vertices;
    public int VertexArrayLength => _state.VertexArrayLength;
    public uint[] Indices => _state.Indices;
    public int IndexArrayLength => _state.IndexArrayLength;


    // bounding box
    // public float[] Vertices => [
    //      0,   0,   0,   0, 0, 0,    0, 0, 0,
    //      0,   0,  XZ,   0, 0, 2,    0, 0, 0,
    //      0,   Y,   0,   0, 2, 0,    0, 0, 0,
    //      0,   Y,  XZ,   0, 2, 2,    0, 0, 0,
    //     XZ,   Y,   0,   2, 0, 0,    0, 0, 0,
    //     XZ,   Y,  XZ,   2, 0, 2,    0, 0, 0,
    //     XZ,   0,   0,   2, 2, 0,    0, 0, 0,
    //     XZ,   0,  XZ,   2, 2, 2,    0, 0, 0,
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