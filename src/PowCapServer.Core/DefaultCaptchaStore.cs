using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using PowCapServer.Abstractions;
using PowCapServer.Models;

namespace PowCapServer;

public class DefaultCaptchaStore : ICaptchaStore
{
    public const string ChallengeTokenCachePrefix = "Captcha:ChallengeToken:";
    public const string CaptchaTokenCachePrefix = "Captcha:CaptchaToken:";

    private readonly IDistributedCache _distributedCache;
    private readonly ISerializer _serializer;

    public DefaultCaptchaStore(IDistributedCache distributedCache, ISerializer serializer)
    {
        _distributedCache = distributedCache;
        _serializer = serializer;
    }

    public virtual Task CleanExpiredTokensAsync(CancellationToken cancellationToken = default)
    {
        // Expired token cleanup is handled by the distributed cache
        return Task.CompletedTask;
    }

    public virtual async Task SaveChallengeTokenInfoAsync(ChallengeTokenInfo challengeTokenInfo, CancellationToken cancellationToken = default)
    {
#if NETSTANDARD2_0
        if (challengeTokenInfo is null)
        {
            throw new ArgumentNullException(nameof(challengeTokenInfo));
        }
#else
        ArgumentNullException.ThrowIfNull(challengeTokenInfo);
#endif

        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpiration = DateTimeOffset.FromUnixTimeMilliseconds(challengeTokenInfo.Expires)
        };
        var value = await _serializer.SerializeAsync(challengeTokenInfo, cancellationToken).ConfigureAwait(false);
        await _distributedCache.SetAsync($"{ChallengeTokenCachePrefix}{challengeTokenInfo.Token}", value, options, cancellationToken).ConfigureAwait(false);
    }

    public virtual async Task<ChallengeTokenInfo?> GetChallengeTokenInfoAsync(string challengeToken, CancellationToken cancellationToken = default)
    {
        var value = await _distributedCache.GetAsync($"{ChallengeTokenCachePrefix}{challengeToken}", cancellationToken).ConfigureAwait(false);
        if (value == null)
        {
            return null;
        }
        return await _serializer.DeserializeAsync<ChallengeTokenInfo>(value, cancellationToken).ConfigureAwait(false);
    }

    public virtual async Task DeleteChallengeTokenInfoAsync(ChallengeTokenInfo challengeTokenInfo, CancellationToken cancellationToken = default)
    {
        if (challengeTokenInfo is null)
        {
            throw new ArgumentNullException(nameof(challengeTokenInfo));
        }

        await _distributedCache.RemoveAsync($"{ChallengeTokenCachePrefix}{challengeTokenInfo.Token}", cancellationToken).ConfigureAwait(false);
    }

    public virtual async Task SaveCaptchaTokenInfoAsync(CaptchaTokenInfo captchaTokenInfo, CancellationToken cancellationToken = default)
    {
        if (captchaTokenInfo is null)
        {
            throw new ArgumentNullException(nameof(captchaTokenInfo));
        }

        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpiration = DateTimeOffset.FromUnixTimeMilliseconds(captchaTokenInfo.Expires)
        };

        var value = await _serializer.SerializeAsync(captchaTokenInfo, cancellationToken).ConfigureAwait(false);
        await _distributedCache.SetAsync($"{CaptchaTokenCachePrefix}{captchaTokenInfo.Token}", value, options, cancellationToken).ConfigureAwait(false);
    }

    public virtual async Task<CaptchaTokenInfo?> GetCaptchaTokenInfoAsync(string captchaToken, CancellationToken cancellationToken = default)
    {
        var value = await _distributedCache.GetAsync($"{CaptchaTokenCachePrefix}{captchaToken}", cancellationToken).ConfigureAwait(false);
        if (value == null)
        {
            return null;
        }
        return await _serializer.DeserializeAsync<CaptchaTokenInfo>(value, cancellationToken).ConfigureAwait(false);
    }

    public virtual async Task DeleteCaptchaTokenInfoAsync(CaptchaTokenInfo captchaTokenInfo, CancellationToken cancellationToken = default)
    {
        if (captchaTokenInfo is null)
        {
            throw new ArgumentNullException(nameof(captchaTokenInfo));
        }

        await _distributedCache.RemoveAsync($"{CaptchaTokenCachePrefix}{captchaTokenInfo.Token}", cancellationToken).ConfigureAwait(false);
    }
}
