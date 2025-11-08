namespace PowCapServer.Models;

public class CaptchaTokenInfo
{
#if NETSTANDARD2_0
    public CaptchaTokenInfo()
    {
        Token = string.Empty;
        Expires = 0;
    }
#endif

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
