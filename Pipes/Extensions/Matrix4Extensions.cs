using OpenTK.Mathematics;

namespace Pipes.Extensions;

public static class Matrix4Extensions
{
    public static Matrix4 Transposed(this Matrix4 matrix)
    {
        Matrix4 result = matrix;
        result.Transpose();
        return result;
    }
}
