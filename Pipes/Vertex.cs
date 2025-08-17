namespace Pipes;

public struct Vertex(Vector2 position, Color color)
{
    public readonly Vector2 Position => position;
    public readonly Color Color => color;
    public readonly float X => Position.X;
    public readonly float Y => Position.Y;
    public readonly float Red => Color.Red;
    public readonly float Blue => Color.Blue;
    public readonly float Green => Color.Green;

    public static class Property
    {
        public const int X = 0;
        public const int Y = 1;
        public const int Z = 2;
        public const int Red = 3;
        public const int Green = 4;
        public const int Blue = 5;
    }
}