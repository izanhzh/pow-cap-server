namespace PowCapServer;

public class PowCapServerOptions
{
    /// <summary>
    /// Number of challenges to generate
    /// </summary>
    public int ChallengeCount { get; set; }

    /// <summary>
    /// Size of each challenge in bytes
    /// </summary>
    public int ChallengeSize { get; set; }

    /// <summary>
    /// Difficulty level of the challenge
    /// </summary>
    public int ChallengeDifficulty { get; set; }

    /// <summary>
    /// Time in milliseconds until the challenge token expires
    /// </summary>
    public long ChallengeTokenExpiresMs { get; set; }

    /// <summary>
    /// Time in milliseconds until the captcha token expires
    /// </summary>
    public long CaptchaTokenExpiresMs { get; set; }

    /// <summary>
    /// Endpoint prefix for request to create a challenge or redeem a challenge
    /// </summary>
    public string EndpointPrefix { get; set; } = "/api/captcha";
}
