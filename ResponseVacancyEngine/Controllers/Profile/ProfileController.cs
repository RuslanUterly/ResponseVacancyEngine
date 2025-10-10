using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResponseVacancyEngine.Application.DTOs;
using ResponseVacancyEngine.Application.Services.Profile.Interfaces;

namespace ResponseVacancyEngine.Controllers.Profile;

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
    

    [HttpPost("update-hh-credentials")]
    [Authorize]
    public async Task<IActionResult> UpdateHeadHunterClientCredentials([FromBody] HeadHunterClientCredentialsDto dto)
    {
        var result = await profileService.UpdateHeadHunterClientCredentialsAsync(this.User, dto);
        
        if (!result)
            return StatusCode(result.StatusCode, result.Error);
        
        return Ok();
    }
    
    [HttpPost("update-hh-tokens")]
    [Authorize]
    public async Task<IActionResult> UpdateHeadHunterJwtTokens([FromBody] HeadHunterJwtCredentialsDto dto)
    {
        var result = await profileService.UpdateHeadHunterJwtCredentialsAsync(this.User, dto);
        
        if (!result)
            return StatusCode(result.StatusCode, result.Error);
        
        return Ok();
    }
    
    [HttpPost("update-hh-response-mode")]
    [Authorize]
    public async Task<IActionResult> UpdateHeadHunterResponseMode([FromBody] bool isActive)
    {
        var result = await profileService.UpdateHeadHunterActiveResponse(this.User, isActive);
        
        if (!result)
            return StatusCode(result.StatusCode, result.Error);
        
        return Ok();
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