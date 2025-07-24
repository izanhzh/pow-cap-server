using System.Threading;
using System.Threading.Tasks;
using AspNetCore3_1.Models;
using Microsoft.AspNetCore.Mvc;
using PowCapServer.Abstractions;
using PowCapServer.Models;

namespace AspNetCore3_1.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CaptchaController : Controller
{
    private readonly ICaptchaService _captchaService;

    public CaptchaController(ICaptchaService captchaService)
    {
        _captchaService = captchaService;
    }

    [HttpPost("challenge")]
    public async Task<IActionResult> Challenge(CancellationToken cancellationToken = default)
    {
        var challengeTokenInfo = await _captchaService.CreateChallengeAsync(cancellationToken).ConfigureAwait(false);
        return Ok(challengeTokenInfo);
    }

    [HttpPost("redeem")]
    public async Task<IActionResult> Redeem([FromBody] ChallengeSolution challengeSolution, CancellationToken cancellationToken = default)
    {
        if (challengeSolution == null)
        {
            return BadRequest("Invalid request");
        }

        var result = await _captchaService.RedeemChallengeAsync(challengeSolution, cancellationToken).ConfigureAwait(false);
        return Ok(result);
    }

    [HttpPost("test-token")]
    public async Task<IActionResult> TestToken([FromBody] TestTokenInput input, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(input?.Token))
        {
            return BadRequest(new { error = "Token is required" });
        }
        var isValid = await _captchaService.ValidateCaptchaToken(input.Token, cancellationToken).ConfigureAwait(false);
        if (isValid)
        {
            return Ok(new { message = "Token is valid", input.Token });
        }
        else
        {
            return BadRequest(new { error = "Invalid or expired token" });
        }
    }
}
