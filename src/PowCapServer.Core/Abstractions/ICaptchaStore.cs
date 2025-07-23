using System.Threading;
using System.Threading.Tasks;
using PowCapServer.Models;

namespace PowCapServer.Abstractions;

public interface ICaptchaStore
{
    Task CleanExpiredTokensAsync(CancellationToken cancellationToken = default);

    Task SaveChallengeTokenInfoAsync(ChallengeTokenInfo challengeTokenInfo, CancellationToken cancellationToken = default);

    Task<ChallengeTokenInfo?> GetChallengeTokenInfoAsync(string challengeToken, CancellationToken cancellationToken = default);

    Task DeleteChallengeTokenInfoAsync(ChallengeTokenInfo challengeTokenInfo, CancellationToken cancellationToken = default);

    Task SaveCaptchaTokenInfoAsync(CaptchaTokenInfo captchaTokenInfo, CancellationToken cancellationToken = default);

    Task<CaptchaTokenInfo?> GetCaptchaTokenInfoAsync(string captchaToken, CancellationToken cancellationToken = default);

    Task DeleteCaptchaTokenInfoAsync(CaptchaTokenInfo captchaTokenInfo, CancellationToken cancellationToken = default);
}
