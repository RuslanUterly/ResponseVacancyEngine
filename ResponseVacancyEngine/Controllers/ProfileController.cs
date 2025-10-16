using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResponseVacancyEngine.Application.Interfaces;

namespace ResponseVacancyEngine.Controllers;

[ApiController]
[Route("[controller]")]
public class ProfileController(IProfileService profileService) : ControllerBase
{
    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetMe()
    {
        var result = await profileService.GetCurrentAccountAsync(this.User);
        
        if (!result)
            return StatusCode(result.StatusCode, result.Error);

        return Ok(result.Data);
    }
    
    [HttpGet("resumes")]
    [Authorize]
    public async Task<IActionResult> GetResumes()
    {
        var result = await profileService.GetResumesAsync(this.User);
        return Ok(result);
    }
    
    [HttpPost("update-hh-response-mode")]
    [Authorize]
    public async Task<IActionResult> UpdateHeadHunterResponseMode([FromBody] bool isActive)
    {
        var result = await profileService.UpdateHeadHunterActiveResponse(this.User, isActive);
        
        if (!result)
            return StatusCode(result.StatusCode, result.Error);
        
        return NoContent();
    }
    
    [HttpGet("login")]
    [Authorize] 
    public async Task<IActionResult> Login()
    {
        return Ok(await profileService.Login());
    }

    [HttpPost("exchange-hh-code")]
    [Authorize]
    public async Task<IActionResult> ExchangeHeadHunterCode([FromBody] string code)
    {
        var result = await profileService.ExchangeHeadHunterCodeAsync(this.User, code);

        if (!result)
            return StatusCode(result.StatusCode, result.Error);
        
        return Ok(result.Data);
    }

    [HttpPost("refresh-hh-tokens")]
    [Authorize]
    public async Task<IActionResult> ResfreshHeadHunterTokens()
    {
        var result = await profileService.RefreshTokenAsync(this.User);

        if (!result)
            return StatusCode(result.StatusCode, result.Error);
        
        return Ok(result.Data);
    }
}