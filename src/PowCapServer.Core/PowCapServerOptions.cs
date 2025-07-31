using System.Collections.Generic;

namespace PowCapServer;

public class PowCapServerOptions
{
    /// <summary>
    /// Default configuration for not specified use cases of PoW Captcha.
    /// </summary>
    public PowCapConfig Default { get; set; } = new PowCapConfig();

    /// <summary>
    /// Dictionary of configurations for different use case of PoW Captcha
    /// </summary>
    public Dictionary<string, PowCapConfig>? UseCaseConfigs { get; set; }

    /// <summary>
    /// Gets the configuration for a specific use case of PoW Captcha.
    /// </summary>
    /// <param name="useCase">The use case of PoW Captcha for which the configuration is requested. If null, no specific use case is queried.</param>
    /// <returns>
    /// Returns the configuration for the specified use case if it exists in <see cref="UseCaseConfigs"/>. 
    /// Returns null if <see cref="UseCaseConfigs"/> is null, the specified use case is null, or the use case is not found in the dictionary.
    /// </returns>
    public PowCapConfig GetPowCapConfig(string? useCase)
    {
        if (UseCaseConfigs == null || useCase == null || !UseCaseConfigs.TryGetValue(useCase, out var config))
        {
            return Default;
        }
        return config;
    }
}
