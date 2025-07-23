using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace PowCapServer.Utils;

public static class RandomUtil
{
    private static readonly RandomNumberGenerator _rng = RandomNumberGenerator.Create();

    public static byte[] RandomBytes(int size)
    {
        var buffer = new byte[size];
        _rng.GetBytes(buffer);
        return buffer;
    }

    public static string ToHexString(byte[] bytes)
    {
#if NETSTANDARD2_0
        return BitConverter.ToString(bytes).Replace("-", "").ToUpperInvariant();
#else
        return Convert.ToHexString(bytes).ToUpperInvariant();
#endif
    }

    public static string Prng(string seed, int length)
    {
        if (seed is null)
        {
            throw new ArgumentNullException(nameof(seed));
        }
        if (length <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(length), "Length must be greater than zero.");
        }

        var state = Fnv1a(seed);
        var result = new StringBuilder();

        while (result.Length < length)
        {
            var rnd = Next(state);
            state = rnd;
            result.Append(rnd.ToString("x8", CultureInfo.InvariantCulture));
        }

        return result.ToString().Substring(0, length);
    }

    private static uint Fnv1a(string str)
    {
        var hash = 2166136261;
        foreach (var c in str)
        {
            hash ^= c;
            hash += (hash << 1) + (hash << 4) + (hash << 7) + (hash << 8) + (hash << 24);
        }
        return hash >>> 0;
    }

    private static uint Next(uint state)
    {
        state ^= state << 13;
        state ^= state >>> 17;
        state ^= state << 5;
        return state >>> 0;
    }
}

