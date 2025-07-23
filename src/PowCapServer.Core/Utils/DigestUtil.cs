using System;
using System.Security.Cryptography;
using System.Text;

namespace PowCapServer.Utils;

public static class DigestUtil
{
    public static string Sha256Hex(string input)
    {
#if NETSTANDARD2_0
        using var sha256 = SHA256.Create();
        var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
        return BitConverter.ToString(hash).Replace("-", "").ToUpperInvariant();
#else
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(input));
        return Convert.ToHexString(hash).ToUpperInvariant();
#endif
    }
}
