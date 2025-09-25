using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK.Mathematics;
using Pipes.Extensions;
using Pipes.Structures;

namespace Pipes.Pipes3D;

public class PipesConstructor(int width, int height, int depth)
{
    private readonly RasterSet _rasterSet = new(width, height, depth);

    private readonly List<PipePoint> _order = [];
    public IEnumerable<PipePoint> Points => _order;

    private readonly Random _random = new();

    public void Construct()
    {
        while (true)
        {
            var init = FindStart();
            if (init is not (Vector3i position, Direction direction))
                return;

            ConstructSinglePipe(position, direction);
        }
    }

    private void ConstructSinglePipe(Vector3i position, Direction direction)
    {
        var color = GenerateColor();

        while (true)
        {
            // 3/4 probability to step forward, if possible
            if (!_random.NextBool(4) && CanStep(position, direction))
            {
                AddPoint(position, JointType.None, color, direction);
                position = position.Moved(direction);
                continue;
            }

            // Otherwise, try to turn
            var newDirections = NewDirections(position, direction).ToList();
            var turned = false;
            if (newDirections.Count > 0)
            {
                direction = _random.NextFromList(newDirections);
                turned = true;
            }

            // step forward, if possible
            if (!CanStep(position, direction))
                return;

            var jointType = turned
                    ? (_random.NextBool(8) ? JointType.Big : JointType.Small)
                    : JointType.None;
            AddPoint(position, jointType, color, direction);
            position = position.Moved(direction);

            continue;
        }
    }

    private Color GenerateColor()
    {
        var hue = 2 * MathF.PI * _random.NextSingle();
        var colorValue = 0.7f + 0.3f * _random.NextSingle();
        return colorValue * Color.FromHue(hue);
    }

    private (Vector3i Position, Direction Direction)? FindStart()
    {
        for (var _ = 0; _ <= 16; _++)
        {
            var position = _random.NextVector3i(new Vector3i(width, height, depth));
            var direction = _random.NextEnum<Direction>();

            if (CanStartPipeFrom(position, direction))
                return (position, direction);
        }

        return null;
    }

    private bool CanStartPipeFrom(Vector3i position, Direction direction)
        => CanAddPoint(position) && CanStep(position, direction);

    private bool CanStep(Vector3i position, Direction direction)
        => CanAddPoint(position.Moved(direction));

    private bool CanAddPoint(Vector3i position)
        => _rasterSet.IsInBounds(position) && !_rasterSet[position];

    private void AddPoint(Vector3i position, JointType jointType, Color color, Direction direction)
    {
        _rasterSet[position] = true;
        _order.Add(new(position, jointType, color, direction));
    }

    private IEnumerable<Direction> NewDirections(Vector3i position, Direction direction)
    {
        foreach (var newDirection in direction.Neighbors())
        {
            var newPosition = position.Moved(newDirection);
            if (CanAddPoint(newPosition))
                yield return newDirection;
        }
    }
}
