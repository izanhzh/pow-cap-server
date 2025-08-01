using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using PowCapServer.Abstractions;
using PowCapServer.Models;
using PowCapServer.Utils;

namespace PowCapServer;

public class DefaultCaptchaService : ICaptchaService
{
    private readonly IOptions<PowCapServerOptions> _powCapServerOptions;
    private readonly ICaptchaStore _captchaStore;

    public DefaultCaptchaService(
        IOptions<PowCapServerOptions> powCapServerOptions,
        ICaptchaStore captchaStore)
    {
        _powCapServerOptions = powCapServerOptions;
        _captchaStore = captchaStore;
    }

    public virtual Task<ChallengeTokenInfo> CreateChallengeAsync(CancellationToken cancellationToken = default)
    {
        return InternalCreateChallengeAsync(_powCapServerOptions.Value.Default, cancellationToken);
    }

    public virtual Task<ChallengeTokenInfo> CreateChallengeAsync(string? useCase, CancellationToken cancellationToken = default)
    {
        return InternalCreateChallengeAsync(_powCapServerOptions.Value.GetPowCapConfig(useCase), cancellationToken);
    }

    protected virtual async Task<ChallengeTokenInfo> InternalCreateChallengeAsync(PowCapConfig? powCapConfig, CancellationToken cancellationToken = default)
    {
        powCapConfig ??= _powCapServerOptions.Value.Default;

        await _captchaStore.CleanExpiredTokensAsync(cancellationToken).ConfigureAwait(false);

        var challenge = new Challenge(powCapConfig.ChallengeCount, powCapConfig.ChallengeSize, powCapConfig.ChallengeDifficulty);
        var token = RandomUtil.ToHexString(RandomUtil.RandomBytes(25));
        var expires = DateTimeOffset.Now.ToUnixTimeMilliseconds() + powCapConfig.ChallengeTokenExpiresMs;

        var challengeTokenInfo = new ChallengeTokenInfo(challenge, token, expires);

        await _captchaStore.SaveChallengeTokenInfoAsync(challengeTokenInfo, cancellationToken).ConfigureAwait(false);

        return challengeTokenInfo;
    }

    public virtual Task<RedeemChallengeResult> RedeemChallengeAsync(ChallengeSolution challengeSolution, CancellationToken cancellationToken = default)
    {
        return InternalRedeemChallengeAsync(_powCapServerOptions.Value.Default, challengeSolution, cancellationToken);
    }

    public virtual Task<RedeemChallengeResult> RedeemChallengeAsync(string? useCase, ChallengeSolution challengeSolution, CancellationToken cancellationToken = default)
    {
        return InternalRedeemChallengeAsync(_powCapServerOptions.Value.GetPowCapConfig(useCase), challengeSolution, cancellationToken);
    }

    protected virtual async Task<RedeemChallengeResult> InternalRedeemChallengeAsync(PowCapConfig? powCapConfig, ChallengeSolution challengeSolution, CancellationToken cancellationToken = default)
    {
        powCapConfig ??= _powCapServerOptions.Value.Default;

        if (challengeSolution == null)
        {
            return RedeemChallengeResult.Error("Invalid redeem challenge request");
        }
        await _captchaStore.CleanExpiredTokensAsync(cancellationToken).ConfigureAwait(false);

        var challengeTokenInfo = await _captchaStore.GetChallengeTokenInfoAsync(challengeSolution.Token, cancellationToken).ConfigureAwait(false);
        if (challengeTokenInfo == null)
        {
            return RedeemChallengeResult.Error("Challenge expired");
        }
        if (challengeTokenInfo.Expires < DateTimeOffset.Now.ToUnixTimeMilliseconds())
        {
            await _captchaStore.DeleteChallengeTokenInfoAsync(challengeTokenInfo, cancellationToken).ConfigureAwait(false);
            return RedeemChallengeResult.Error("Challenge expired");
        }

        await _captchaStore.DeleteChallengeTokenInfoAsync(challengeTokenInfo, cancellationToken).ConfigureAwait(false);

        var challenge = challengeTokenInfo.Challenge;

        var isValid = Enumerable.Range(0, challenge.C).All(i =>
        {
            var salt = RandomUtil.Prng($"{challengeSolution.Token}{i + 1}", challenge.S);
            var target = RandomUtil.Prng($"{challengeSolution.Token}{i + 1}d", challenge.D);
            var solution = challengeSolution.Solutions[i];

            var hash = DigestUtil.Sha256Hex(salt + solution);
            return hash.StartsWith(target, StringComparison.InvariantCultureIgnoreCase);
        });

        if (!isValid)
        {
            return RedeemChallengeResult.Error("Invalid challenge solution");
        }

        var vertoken = RandomUtil.ToHexString(RandomUtil.RandomBytes(15));
        var expires = DateTimeOffset.Now.ToUnixTimeMilliseconds() + powCapConfig.CaptchaTokenExpiresMs;
        var hash = DigestUtil.Sha256Hex(vertoken);
        var id = RandomUtil.ToHexString(RandomUtil.RandomBytes(8));

        await _captchaStore.SaveCaptchaTokenInfoAsync(new CaptchaTokenInfo($"{id}_{hash}", expires), cancellationToken).ConfigureAwait(false);

        return RedeemChallengeResult.Ok($"{id}_{vertoken}", expires);
    }

    public virtual async Task<bool> ValidateCaptchaTokenAsync(string captchaToken, CancellationToken cancellationToken = default)
    {
        await _captchaStore.CleanExpiredTokensAsync(cancellationToken).ConfigureAwait(false);

        var idAndVertoken = captchaToken?.Split('_');
        if (idAndVertoken == null || idAndVertoken.Length != 2)
        {
            return false;
        }

        var id = idAndVertoken[0];
        var hash = DigestUtil.Sha256Hex(idAndVertoken[1]);
        var actualToken = $"{id}_{hash}";

        var captchaTokenInfo = await _captchaStore.GetCaptchaTokenInfoAsync(actualToken, cancellationToken).ConfigureAwait(false);
        if (captchaTokenInfo == null)
        {
            return false;
        }
        if (captchaTokenInfo.Expires < DateTimeOffset.Now.ToUnixTimeMilliseconds())
        {
            await _captchaStore.DeleteCaptchaTokenInfoAsync(captchaTokenInfo, cancellationToken).ConfigureAwait(false);
            return false;
        }

        await _captchaStore.DeleteCaptchaTokenInfoAsync(captchaTokenInfo, cancellationToken).ConfigureAwait(false);

        return true;
    }
}
