namespace Pipes.Structures;

public struct Vector2(float x, float y)
{
    public readonly float X => x;
    public readonly float Y => y;

    public static Vector2 operator +(Vector2 left, Vector2 right)
    {
        return new Vector2(left.X + right.X, left.Y + right.Y);
    }

    public static Vector2 operator -(Vector2 left, Vector2 right)
    {
        return new Vector2(left.X - right.X, left.Y - right.Y);
    }

    public static implicit operator Vector2((float X, float Y) pair) => new(pair.X, pair.Y);
}
