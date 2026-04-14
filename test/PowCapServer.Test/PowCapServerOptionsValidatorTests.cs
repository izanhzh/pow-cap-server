namespace PowCapServer.Test;

public class PowCapServerOptionsValidatorTests
{
    private readonly PowCapServerOptionsValidator _validator = new();

    [Fact]
    public void Validate_WithDefaultConfig_Succeeds()
    {
        var result = _validator.Validate(null, new PowCapServerOptions());

        Assert.True(result.Succeeded);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_WhenChallengeCountIsInvalid_Fails(int count)
    {
        var options = new PowCapServerOptions();
        options.Default.ChallengeCount = count;

        var result = _validator.Validate(null, options);

        Assert.True(result.Failed);
        Assert.Contains(result.Failures, f => f.Contains(nameof(PowCapConfig.ChallengeCount)));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_WhenChallengeSizeIsInvalid_Fails(int size)
    {
        var options = new PowCapServerOptions();
        options.Default.ChallengeSize = size;

        var result = _validator.Validate(null, options);

        Assert.True(result.Failed);
        Assert.Contains(result.Failures, f => f.Contains(nameof(PowCapConfig.ChallengeSize)));
    }

    [Fact]
    public void Validate_WhenChallengeDifficultyIsZero_Fails()
    {
        var options = new PowCapServerOptions();
        options.Default.ChallengeDifficulty = 0;

        var result = _validator.Validate(null, options);

        Assert.True(result.Failed);
        Assert.Contains(result.Failures, f => f.Contains(nameof(PowCapConfig.ChallengeDifficulty)));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_WhenChallengeTokenExpiresMsIsInvalid_Fails(long ms)
    {
        var options = new PowCapServerOptions();
        options.Default.ChallengeTokenExpiresMs = ms;

        var result = _validator.Validate(null, options);

        Assert.True(result.Failed);
        Assert.Contains(result.Failures, f => f.Contains(nameof(PowCapConfig.ChallengeTokenExpiresMs)));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_WhenCaptchaTokenExpiresMsIsInvalid_Fails(long ms)
    {
        var options = new PowCapServerOptions();
        options.Default.CaptchaTokenExpiresMs = ms;

        var result = _validator.Validate(null, options);

        Assert.True(result.Failed);
        Assert.Contains(result.Failures, f => f.Contains(nameof(PowCapConfig.CaptchaTokenExpiresMs)));
    }

    [Fact]
    public void Validate_WhenUseCaseConfigIsInvalid_ReportsUseCaseName()
    {
        var options = new PowCapServerOptions
        {
            UseCaseConfigs = new Dictionary<string, PowCapConfig>
            {
                ["login"] = new PowCapConfig { ChallengeCount = 0 }
            }
        };

        var result = _validator.Validate(null, options);

        Assert.True(result.Failed);
        Assert.Contains(result.Failures, f => f.Contains("login"));
    }

    [Fact]
    public void Validate_ReportsAllErrors_WhenMultipleFieldsInvalid()
    {
        var options = new PowCapServerOptions();
        options.Default.ChallengeCount = 0;
        options.Default.ChallengeSize = 0;
        options.Default.ChallengeDifficulty = 0;

        var result = _validator.Validate(null, options);

        Assert.True(result.Failed);
        Assert.True(result.Failures.Count() >= 3);
    }
}
