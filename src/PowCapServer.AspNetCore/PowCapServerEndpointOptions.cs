using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace PowCapServer;

public class PowCapServerEndpointOptions
{
    /// <summary>
    /// URL prefix for all PoW endpoints. Defaults to "/api/captcha".
    /// </summary>
    public string EndpointPrefix { get; set; } = "/api/captcha";

    /// <summary>
    /// Name of a rate limiter policy (registered via <c>AddRateLimiter</c>) to apply to all endpoints.
    /// When <c>null</c> (default), no rate limiting is enforced.
    /// </summary>
    public string? RateLimiterPolicy { get; set; }

    /// <summary>
    /// Factory that builds the logging scope attached to redeem requests.
    /// When <c>null</c> (default), no scope is created.
    /// Requires the logging provider to have scopes enabled to be effective.
    /// </summary>
    public Func<HttpContext, Dictionary<string, object?>>? RequestScopeFactory { get; set; }

    /// <summary>
    /// Maximum allowed size in bytes for the body of redeem requests.
    /// Defaults to 65 536 bytes (64 KB). Set to <c>null</c> to use the server default.
    /// </summary>
    public long? MaxRedeemBodySize { get; set; } = 65_536;
}
