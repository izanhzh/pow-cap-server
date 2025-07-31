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

    Task<bool> ValidateCaptchaTokenAsync(string captchaToken, CancellationToken cancellationToken = default);
}
