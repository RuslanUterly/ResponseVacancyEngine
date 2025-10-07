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
    

    [HttpPost("add-hh-credentials")]
    [Authorize]
    public async Task<IActionResult> AddHeadHunterClientCredentials([FromBody] HeadHunterClientCredentialsDto dto)
    {
        var result = await profileService.AddHeadHunterClientCredentialsAsync(this.User, dto);
        
        if (!result)
            return StatusCode(result.StatusCode, result.Error);
        
        return Ok();
    }
}