namespace PowCapServer.Models;

public class RedeemChallengeResult
{
    /// <summary>
    /// Whether the challenge is successful or not
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Failure message
    /// </summary>
    public string? Message { get; set; }

    /// <summary>
    /// The token generated after the challenge is successfully completed
    /// </summary>
    public string? Token { get; set; }

    /// <summary>
    /// Token expiration time
    /// </summary>
    public long Expires { get; set; }

    public static RedeemChallengeResult Error(string message)
    {
        return new RedeemChallengeResult { Success = false, Message = message };
    }

    public static RedeemChallengeResult Ok(string token, long expires)
    {
        return new RedeemChallengeResult
        {
            Success = true,
            Token = token,
            Expires = expires
        };
    }
}
