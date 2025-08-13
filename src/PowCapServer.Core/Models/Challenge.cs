namespace PowCapServer.Models;

public record Challenge
{
#if NETSTANDARD2_0
    public Challenge()
    {
        C = 0;
        S = 0;
        D = 0;
    }
#endif

    public Challenge(int c, int s, int d)
    {
        C = c;
        S = s;
        D = d;
    }

    /// <summary>
    /// Number of challenges
    /// </summary>
    public int C { get; set; }

    /// <summary>
    /// Size of each challenge
    /// </summary>
    public int S { get; set; }

    /// <summary>
    /// Difficulty level
    /// </summary>
    public int D { get; set; }
}
