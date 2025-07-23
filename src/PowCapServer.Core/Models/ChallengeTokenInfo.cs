namespace PowCapServer.Models;

public record ChallengeTokenInfo
{
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
