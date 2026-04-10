using PowCapServer.Abstractions;
using PowCapServer.Models;

namespace PowCapServer.Test.Fakes;

internal sealed class FakeCaptchaStore : ICaptchaStore
{
    private readonly Dictionary<string, ChallengeTokenInfo> _challengeTokens = new();
    private readonly Dictionary<string, CaptchaTokenInfo> _captchaTokens = new();

    public Task CleanExpiredTokensAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

    public Task SaveChallengeTokenInfoAsync(ChallengeTokenInfo info, CancellationToken cancellationToken = default)
    {
        _challengeTokens[info.Token] = info;
        return Task.CompletedTask;
    }

    public Task<ChallengeTokenInfo?> GetChallengeTokenInfoAsync(string token, CancellationToken cancellationToken = default)
    {
        _challengeTokens.TryGetValue(token, out var info);
        return Task.FromResult(info);
    }

    public Task DeleteChallengeTokenInfoAsync(ChallengeTokenInfo info, CancellationToken cancellationToken = default)
    {
        _challengeTokens.Remove(info.Token);
        return Task.CompletedTask;
    }

    public Task SaveCaptchaTokenInfoAsync(CaptchaTokenInfo info, CancellationToken cancellationToken = default)
    {
        _captchaTokens[info.Token] = info;
        return Task.CompletedTask;
    }

    public Task<CaptchaTokenInfo?> GetCaptchaTokenInfoAsync(string token, CancellationToken cancellationToken = default)
    {
        _captchaTokens.TryGetValue(token, out var info);
        return Task.FromResult(info);
    }

    public Task DeleteCaptchaTokenInfoAsync(CaptchaTokenInfo info, CancellationToken cancellationToken = default)
    {
        _captchaTokens.Remove(info.Token);
        return Task.CompletedTask;
    }
}
