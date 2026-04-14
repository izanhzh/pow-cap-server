using System.Collections.Generic;
using Microsoft.Extensions.Options;

namespace PowCapServer;

public sealed class PowCapServerOptionsValidator : IValidateOptions<PowCapServerOptions>
{
    public ValidateOptionsResult Validate(string? name, PowCapServerOptions options)
    {
        var errors = new List<string>();

        ValidateConfig("Default", options.Default, errors);

        if (options.UseCaseConfigs != null)
        {
            foreach (var kvp in options.UseCaseConfigs)
            {
                ValidateConfig(kvp.Key, kvp.Value, errors);
            }
        }

        return errors.Count > 0
            ? ValidateOptionsResult.Fail(errors)
            : ValidateOptionsResult.Success;
    }

    private static void ValidateConfig(string name, PowCapConfig config, List<string> errors)
    {
        if (config.ChallengeCount <= 0)
        {
            errors.Add($"{name}.{nameof(PowCapConfig.ChallengeCount)} must be greater than 0");
        }

        if (config.ChallengeSize <= 0)
        {
            errors.Add($"{name}.{nameof(PowCapConfig.ChallengeSize)} must be greater than 0");
        }

        if (config.ChallengeDifficulty < 1)
        {
            errors.Add($"{name}.{nameof(PowCapConfig.ChallengeDifficulty)} must be at least 1");
        }

        if (config.ChallengeTokenExpiresMs <= 0)
        {
            errors.Add($"{name}.{nameof(PowCapConfig.ChallengeTokenExpiresMs)} must be greater than 0");
        }

        if (config.CaptchaTokenExpiresMs <= 0)
        {
            errors.Add($"{name}.{nameof(PowCapConfig.CaptchaTokenExpiresMs)} must be greater than 0");
        }
    }
}
