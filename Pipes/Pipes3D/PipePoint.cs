using OpenTK.Mathematics;
using Pipes.Structures;

namespace Pipes.Pipes3D;

public enum JointType { None, Small, Big }

public class PipePoint(Vector3i position, JointType jointType, Color color, Direction direction)
{
    public Vector3i Position => position;
    public JointType JointType => jointType;
    public Color Color => color;
    public Direction Direction => direction;
}
