namespace PowCapServer;

public class PowCapConfig
{
    /// <summary>
    /// Number of challenges to generate
    /// </summary>
    public int ChallengeCount { get; set; } = 50;

    /// <summary>
    /// Size of each challenge in bytes
    /// </summary>
    public int ChallengeSize { get; set; } = 32;

    /// <summary>
    /// Difficulty level of the challenge
    /// </summary>
    public int ChallengeDifficulty { get; set; } = 4;

    /// <summary>
    /// Time in milliseconds until the challenge token expires
    /// </summary>
    public long ChallengeTokenExpiresMs { get; set; } = 600000;

    /// <summary>
    /// Time in milliseconds until the captcha token expires
    /// </summary>
    public long CaptchaTokenExpiresMs { get; set; } = 1200000;
}
