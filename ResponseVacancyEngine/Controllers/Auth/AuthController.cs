using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using ResponseVacancyEngine.Application.Services.Auth.Intefaces;

namespace ResponseVacancyEngine.Controllers.Auth;

[ApiController]
[Route("[controller]")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync([FromBody] LoginRequest loginRequest)
    {
        var result = await authService.LoginAsync(loginRequest);

        if (!result)
            return StatusCode(result.StatusCode, result.Error);
        
        return Ok(result.Data);
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterAsync([FromBody] RegisterRequest registerRequest)
    {
        var result = await authService.RegisterAsync(registerRequest);

        if (!result)
            return StatusCode(result.StatusCode, result.Error);

        return CreatedAtAction(nameof(RegisterAsync), new { id = result.Data }, result.Data);
    }
}