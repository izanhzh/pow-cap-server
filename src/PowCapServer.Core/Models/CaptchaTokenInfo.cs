namespace PowCapServer.Models;

public record CaptchaTokenInfo
{
    public CaptchaTokenInfo(string token, long expires)
    {
        Token = token;
        Expires = expires;
    }

    /// <summary>
    /// Captcha token
    /// </summary>
    public string Token { get; set; }

    /// <summary>
    /// Expiration timestamp
    /// </summary>
    public long Expires { get; set; }
}
