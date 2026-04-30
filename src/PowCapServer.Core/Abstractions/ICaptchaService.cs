using System.Threading;
using System.Threading.Tasks;
using PowCapServer.Models;

namespace PowCapServer.Abstractions;

public interface ICaptchaService
{
    Task<ChallengeTokenInfo> CreateChallengeAsync(CancellationToken cancellationToken = default);

    Task<ChallengeTokenInfo> CreateChallengeAsync(string? useCase, CancellationToken cancellationToken = default);

    Task<RedeemChallengeResult> RedeemChallengeAsync(ChallengeSolution challengeSolution, CancellationToken cancellationToken = default);

    Task<RedeemChallengeResult> RedeemChallengeAsync(string? useCase, ChallengeSolution challengeSolution, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validate a captcha token without checking which use case it was
    /// redeemed for. Prefer the <see cref="ValidateCaptchaTokenAsync(string?, string, CancellationToken)"/>
    /// overload on any endpoint that opts into a non-default use case;
    /// without the use-case check, a token redeemed under one
    /// configuration is accepted by every other (issue #20).
    /// </summary>
    Task<bool> ValidateCaptchaTokenAsync(string captchaToken, CancellationToken cancellationToken = default);

    /// <summary>
    /// Validate a captcha token and require it to have been redeemed
    /// under the supplied <paramref name="useCase"/>. Pass
    /// <c>null</c> to require the default (no-use-case) configuration.
    /// Tokens whose stored use case does not match are rejected even
    /// when the token itself is well-formed and unexpired.
    /// </summary>
    Task<bool> ValidateCaptchaTokenAsync(string? useCase, string captchaToken, CancellationToken cancellationToken = default);
}
