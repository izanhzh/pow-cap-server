namespace PowCapServer.Models;

public class CaptchaTokenInfo
{
#if NETSTANDARD2_0
    public CaptchaTokenInfo()
    {
        Token = string.Empty;
        Expires = 0;
        UseCase = null;
    }
#endif

    public CaptchaTokenInfo(string token, long expires)
        : this(token, expires, null)
    {
    }

    public CaptchaTokenInfo(string token, long expires, string? useCase)
    {
        Token = token;
        Expires = expires;
        UseCase = useCase;
    }

    /// <summary>
    /// Captcha token
    /// </summary>
    public string Token { get; set; }

    /// <summary>
    /// Expiration timestamp
    /// </summary>
    public long Expires { get; set; }

    /// <summary>
    /// The use case the token was redeemed for. <c>null</c> means the
    /// token was redeemed under the default (no use case specified)
    /// configuration. Validation must compare this against the use case
    /// the consumer is gating on; without that check, a token redeemed
    /// for a low-friction use case (e.g. <c>contact-us</c>) would pass
    /// a high-friction one (e.g. <c>register-new-account</c>). See
    /// issue #20.
    /// </summary>
    public string? UseCase { get; set; }
}
