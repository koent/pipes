using System;

namespace Pipes.Extensions;

public static class RandomExtensions
{
    public static bool NextBool(this Random random, int odds = 2)
        => random.NextInt64() % odds == 0;

    public static TEnum NextEnum<TEnum>(this Random random) where TEnum : struct, Enum
    {
        var values = Enum.GetValues<TEnum>();
        return values[random.NextInt64() % values.Length];
    }
}
