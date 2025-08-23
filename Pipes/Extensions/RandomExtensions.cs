using System;
using System.Collections.Generic;
using OpenTK.Mathematics;
namespace Pipes.Extensions;

public static class RandomExtensions
{
    public static bool NextBool(this Random random, int odds = 2)
        => random.NextInt64() % odds == 0;

    public static TEnum NextEnum<TEnum>(this Random random) where TEnum : struct, Enum
    {
        var values = Enum.GetValues<TEnum>();
        return random.NextFromList(values);
    }

    public static T NextFromList<T>(this Random random, IReadOnlyList<T> list)
        => list[random.Next() % list.Count];

    public static Vector3i NextVector3i(this Random random, Vector3i max)
    {
        if (max.X <= 0 || max.Y <= 0 || max.Z <= 0)
            throw new ArgumentOutOfRangeException(nameof(max));

        return new Vector3i(random.Next() % max.X, random.Next() % max.Y, random.Next() % max.Z);
    }
}
