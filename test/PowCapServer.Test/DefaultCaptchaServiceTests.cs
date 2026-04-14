using Microsoft.Extensions.Options;
using PowCapServer.Models;
using PowCapServer.Test.Fakes;

namespace PowCapServer.Test;

public class DefaultCaptchaServiceTests
{
    private readonly FakeCaptchaStore _store = new();
    private readonly DefaultCaptchaService _service;

    public DefaultCaptchaServiceTests()
    {
        var options = Options.Create(new PowCapServerOptions());
        _service = new DefaultCaptchaService(options, _store);
    }

    // --- RedeemChallengeAsync: error paths ---

    [Fact]
    public async Task RedeemChallengeAsync_WhenSolutionsIsNull_ReturnsError()
    {
        var token = await StoreValidChallengeAsync(challengeCount: 2);

        var result = await _service.RedeemChallengeAsync(new ChallengeSolution(token, null!));

        Assert.False(result.Success);
        Assert.NotNull(result.Message);
    }

    [Fact]
    public async Task RedeemChallengeAsync_WhenSolutionsCountDoesNotMatchChallenge_ReturnsError()
    {
        var token = await StoreValidChallengeAsync(challengeCount: 3);
        var wrongSolutions = new List<int> { 1, 2 }; // 2 instead of 3

        var result = await _service.RedeemChallengeAsync(new ChallengeSolution(token, wrongSolutions));

        Assert.False(result.Success);
        Assert.NotNull(result.Message);
    }

    [Fact]
    public async Task RedeemChallengeAsync_WhenChallengeTokenNotFound_ReturnsError()
    {
        var result = await _service.RedeemChallengeAsync(new ChallengeSolution("unknown_token", new List<int>()));

        Assert.False(result.Success);
        Assert.NotNull(result.Message);
    }

    [Fact]
    public async Task RedeemChallengeAsync_WhenChallengeIsExpired_ReturnsError()
    {
        const string token = "expired_token";
        var expiredInfo = new ChallengeTokenInfo(
            new Challenge(1, 32, 1),
            token,
            DateTimeOffset.Now.AddMinutes(-5).ToUnixTimeMilliseconds());
        await _store.SaveChallengeTokenInfoAsync(expiredInfo);

        var result = await _service.RedeemChallengeAsync(new ChallengeSolution(token, new List<int> { 0 }));

        Assert.False(result.Success);
        Assert.NotNull(result.Message);
    }

    // --- RedeemChallengeAsync: success path ---

    [Fact]
    public async Task RedeemChallengeAsync_WhenSolutionsAreCorrect_ReturnsSuccessWithToken()
    {
        // C=0: no challenges to solve, vacuously valid
        var token = await StoreValidChallengeAsync(challengeCount: 0);

        var result = await _service.RedeemChallengeAsync(new ChallengeSolution(token, new List<int>()));

        Assert.True(result.Success);
        Assert.NotNull(result.Token);
        Assert.Contains('_', result.Token);
    }

    // --- ValidateCaptchaTokenAsync ---

    [Fact]
    public async Task ValidateCaptchaTokenAsync_WhenTokenFormatIsInvalid_ReturnsFalse()
    {
        Assert.False(await _service.ValidateCaptchaTokenAsync("no-underscore"));
        Assert.False(await _service.ValidateCaptchaTokenAsync(""));
        Assert.False(await _service.ValidateCaptchaTokenAsync(null!));
    }

    [Fact]
    public async Task ValidateCaptchaTokenAsync_WhenTokenNotInStore_ReturnsFalse()
    {
        var result = await _service.ValidateCaptchaTokenAsync("someId_someVertoken");

        Assert.False(result);
    }

    [Fact]
    public async Task ValidateCaptchaTokenAsync_WhenTokenIsValid_ReturnsTrue()
    {
        // Full flow: create challenge (C=0) → solve it → validate the resulting captcha token
        var challengeToken = await StoreValidChallengeAsync(challengeCount: 0);
        var redeemResult = await _service.RedeemChallengeAsync(new ChallengeSolution(challengeToken, new List<int>()));
        Assert.True(redeemResult.Success);

        var valid = await _service.ValidateCaptchaTokenAsync(redeemResult.Token!);

        Assert.True(valid);
    }

    [Fact]
    public async Task ValidateCaptchaTokenAsync_WhenTokenUsedTwice_ReturnsFalseOnSecondUse()
    {
        var challengeToken = await StoreValidChallengeAsync(challengeCount: 0);
        var redeemResult = await _service.RedeemChallengeAsync(new ChallengeSolution(challengeToken, new List<int>()));

        await _service.ValidateCaptchaTokenAsync(redeemResult.Token!); // first use
        var secondUse = await _service.ValidateCaptchaTokenAsync(redeemResult.Token!);

        Assert.False(secondUse); // token was consumed
    }

    // --- Helpers ---

    private async Task<string> StoreValidChallengeAsync(int challengeCount)
    {
        const string token = "test_challenge_token";
        var info = new ChallengeTokenInfo(
            new Challenge(challengeCount, 32, 1),
            token,
            DateTimeOffset.Now.AddMinutes(10).ToUnixTimeMilliseconds());
        await _store.SaveChallengeTokenInfoAsync(info);
        return token;
    }
}
