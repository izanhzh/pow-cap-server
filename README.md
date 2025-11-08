# What is PowCapServer

This is a  **.NET Core server implementation** of the open-source project [tiagozip/cap](https://github.com/tiagozip/cap). based on the Proof-of-Work (PoW) mechanism. It provides a lightweight and non-intrusive CAPTCHA solution that can be used to prevent bot abuse and spam submissions.


## 📦 NuGet Packages
The project is split into two main libraries, which will be published to NuGet:

- PowCapServer.Core – Core logic and services for CAPTCHA generation and validation.
- PowCapServer.AspNetCore – ASP.NET Core integration for exposing CAPTCHA endpoints as HTTP APIs.

You can install them via:

```bash
dotnet add package PowCapServer.Core
dotnet add package PowCapServer.AspNetCore
```


## 🧩 Features
- ✅ Challenge generation (`/api/captcha/challenge` or `/api/captcha/{useCase}/challenge`)
- ✅ Challenge redemption (`/api/captcha/redeem` or `/api/captcha/{useCase}/redeem`)
- ✅ Token-based CAPTCHA validation
- ✅ Configurable difficulty, expiration times, and endpoint paths
- ✅ Built-in token cleanup for expired challenges
- ✅ ASP.NET Core middleware and endpoint integration


## 🛠️ Usage
1. Install the NuGet packages
```bash
dotnet add package PowCapServer.AspNetCore
```

2. Register services
```csharp
builder.Services.AddPowCapServer(options =>
{
    // Default configuration for CAPTCHAs without specific use case
    options.Default.ChallengeCount = 1000;
    options.Default.ChallengeSize = 32;
    options.Default.ChallengeDifficulty = 4;
    options.Default.ChallengeTokenExpiresMs = 60000;
    options.Default.CaptchaTokenExpiresMs = 120000;

    // Configuration for specific use case CAPTCHA
    options.UseCaseConfigs = new Dictionary<string, PowCapConfig>()
    {
        ["login"] = new PowCapConfig
        {
            ChallengeCount = 1000,
            ChallengeSize = 32,
            ChallengeDifficulty = 5,
            ChallengeTokenExpiresMs = 60000,
            CaptchaTokenExpiresMs = 120000
        },
        ["form"] = new PowCapConfig
        {
            ChallengeCount = 100,
            ChallengeSize = 16,
            ChallengeDifficulty = 3,
            ChallengeTokenExpiresMs = 120000,
            CaptchaTokenExpiresMs = 600000
        }
    };
});
```


3. Map CAPTCHA endpoints
```csharp
app.MapPowCapServer();
```

This will expose the following endpoints:

- POST /api/captcha/challenge – Generate a new CAPTCHA challenge with default configuration.
- POST /api/captcha/{useCase}/challenge – Generate a new CAPTCHA challenge with configuration specific to the use case.
- POST /api/captcha/redeem – Redeem a solved CAPTCHA challenge with default configuration.
- POST /api/captcha/{useCase}/redeem – Redeem a solved CAPTCHA challenge with configuration specific to the use case.


## 🗃️ Storage and Caching
By default, PowCapServer uses the `Microsoft.Extensions.Caching.Memory` implementation of `IDistributedCache` to store CAPTCHA-related data in memory. This provides a lightweight, in-memory storage solution that's perfect for single-instance deployments.

For more robust scenarios such as multi-instance deployments or when persistence is required, you can replace the default in-memory cache with other `IDistributedCache` implementations. Popular alternatives include:

- Redis Distributed Cache
- SQL Server Distributed Cache

To use Redis as an example, first install the required package:

```bash
dotnet add package Microsoft.Extensions.Caching.StackExchangeRedis
```

Then configure it in your service registration:

```csharp
builder.Services.AddPowCapServer();
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = "localhost:6379"; // Redis server configuration
    options.InstanceName = "PowCapServer:";
});
```

For more information on available `IDistributedCache` implementations, please refer to the [Microsoft Documentation](https://learn.microsoft.com/en-us/aspnet/core/performance/caching/distributed).


## 📐 Integration with Frontend
please refer to the official documentation of [@cap.js/widget](https://capjs.js.org/guide/widget.html) for instructions on how to embed and configure the CAPTCHA widget in your web application.

Example for default CAPTCHA:

```html
<script src="https://cdn.jsdelivr.net/npm/@cap.js/widget"></script>

<cap-widget id="cap" data-cap-api-endpoint="/api/captcha/"></cap-widget>
```

Example for specific use case CAPTCHA (e.g. login):
```html
<script src="https://cdn.jsdelivr.net/npm/@cap.js/widget"></script>

<cap-widget id="cap" data-cap-api-endpoint="/api/captcha/login/"></cap-widget>
```

You can listen to the solve event to obtain the generated token and proceed with your form submission or API calls.


## 🧪 Validate the CAPTCHA Token in a Controller
To use the CAPTCHA token validation in a real-world scenario, you can inject `ICaptchaService` into any controller (e.g., a LoginController) and verify the token submitted by the client.

Example: Validate Token in LoginController

```csharp
[ApiController]
[Route("[controller]")]
public class LoginController : ControllerBase
{
    private readonly ICaptchaService _captchaService;

    public LoginController(ICaptchaService captchaService)
    {
        _captchaService = captchaService;
    }

    [HttpPost]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken ct = default)
    {
        if (string.IsNullOrEmpty(request.CaptchaToken))
        {
            return BadRequest("CAPTCHA token is required.");
        }

        var isValid = await _captchaService.ValidateCaptchaTokenAsync(request.CaptchaToken, ct);

        if (!isValid)
        {
            return BadRequest("Invalid or expired CAPTCHA token.");
        }

        // Proceed with login logic
        return Ok(new { message = "Login successful" });
    }
}

public class LoginRequest
{
    public string Username { get; set; }
    public string Password { get; set; }
    public string CaptchaToken { get; set; }
}
```

- The `ICaptchaService` is injected via constructor injection.
- The `ValidateCaptchaTokenAsync` method is used to verify the token submitted by the client.
- This helps prevent bot abuse on critical endpoints such as login, registration, or form submission.


## 📚 View Sample Project
Please check the `samples/WebApplication1` folder in the source code. It includes:

- ✅ A full ASP.NET Core web application integrated with PowCapServer
- ✅ Frontend usage with the [@cap.js/widget](https://capjs.js.org/guide/widget.html)
- ✅ Example controller usage for token validation

## 📚 License
This project is licensed under the Apache-2.0 license – see the [LICENSE](https://github.com/izanhzh/pow-cap-server?tab=Apache-2.0-1-ov-file) file for details.
