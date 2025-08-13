namespace PowCapServer.Models;

public record ChallengeTokenInfo
{
#if NETSTANDARD2_0
    public ChallengeTokenInfo()
    {
        Challenge = new Challenge(0, 0, 0);
        Token = string.Empty;
        Expires = 0;
    }
#endif

    public ChallengeTokenInfo(Challenge challenge, string token, long expires)
    {
        Challenge = challenge;
        Token = token;
        Expires = expires;
    }

    /// <summary>
    /// Challenge configuration object
    /// </summary>
    public Challenge Challenge { get; set; }

    /// <summary>
    /// Challenge token
    /// </summary>
    public string Token { get; set; }

    /// <summary>
    /// Expiration timestamp
    /// </summary>
    public long Expires { get; set; }
}
