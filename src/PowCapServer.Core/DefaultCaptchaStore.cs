using System;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using PowCapServer.Abstractions;
using PowCapServer.Models;

namespace PowCapServer;

public class DefaultCaptchaStore : ICaptchaStore
{
    private readonly IDistributedCache _distributedCache;

    public DefaultCaptchaStore(IDistributedCache distributedCache)
    {
        _distributedCache = distributedCache;
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
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMilliseconds(challengeTokenInfo.Expires)
        };
        using var stream = new MemoryStream();
        await JsonSerializer.SerializeAsync(stream, challengeTokenInfo, cancellationToken: cancellationToken).ConfigureAwait(false);
        await _distributedCache.SetAsync(challengeTokenInfo.Token, stream.ToArray(), options, cancellationToken).ConfigureAwait(false);
    }

    public virtual async Task<ChallengeTokenInfo?> GetChallengeTokenInfoAsync(string challengeToken, CancellationToken cancellationToken = default)
    {
        var value = await _distributedCache.GetAsync(challengeToken, cancellationToken).ConfigureAwait(false);
        if (value == null)
        {
            return null;
        }
        using var stream = new MemoryStream(value);
        return await JsonSerializer.DeserializeAsync<ChallengeTokenInfo>(stream, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    public virtual async Task DeleteChallengeTokenInfoAsync(ChallengeTokenInfo challengeTokenInfo, CancellationToken cancellationToken = default)
    {
        if (challengeTokenInfo is null)
        {
            throw new ArgumentNullException(nameof(challengeTokenInfo));
        }

        await _distributedCache.RemoveAsync(challengeTokenInfo.Token, cancellationToken).ConfigureAwait(false);
    }

    public virtual async Task SaveCaptchaTokenInfoAsync(CaptchaTokenInfo captchaTokenInfo, CancellationToken cancellationToken = default)
    {
        if (captchaTokenInfo is null)
        {
            throw new ArgumentNullException(nameof(captchaTokenInfo));
        }

        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMilliseconds(captchaTokenInfo.Expires)
        };

        using var stream = new MemoryStream();
        await JsonSerializer.SerializeAsync(stream, captchaTokenInfo, cancellationToken: cancellationToken).ConfigureAwait(false);
        await _distributedCache.SetAsync(captchaTokenInfo.Token, stream.ToArray(), options, cancellationToken).ConfigureAwait(false);
    }

    public virtual async Task<CaptchaTokenInfo?> GetCaptchaTokenInfoAsync(string captchaToken, CancellationToken cancellationToken = default)
    {
        var value = await _distributedCache.GetAsync(captchaToken, cancellationToken).ConfigureAwait(false);
        if (value == null)
        {
            return null;
        }
        using var stream = new MemoryStream(value);
        return await JsonSerializer.DeserializeAsync<CaptchaTokenInfo>(stream, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    public virtual async Task DeleteCaptchaTokenInfoAsync(CaptchaTokenInfo captchaTokenInfo, CancellationToken cancellationToken = default)
    {
        if (captchaTokenInfo is null)
        {
            throw new ArgumentNullException(nameof(captchaTokenInfo));
        }

        await _distributedCache.RemoveAsync(captchaTokenInfo.Token, cancellationToken).ConfigureAwait(false);
    }
}
