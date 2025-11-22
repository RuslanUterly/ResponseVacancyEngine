using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResponseVacancyEngine.Application.DTOs.RespondedVacancy;
using ResponseVacancyEngine.Application.Interfaces.Services;

namespace ResponseVacancyEngine.Controllers;

[ApiController]
[Route("[controller]")]
public class VacancyController(IVacancyService vacancyService) : ControllerBase
{
    // [Authorize]
    // [HttpGet("account/{accountId}")]
    // public async Task<List<VacancyDto>> GetVacanciesByAccountIdAsync(long accountId)
    // {
    //     var result = await vacancyService.GetVacanciesByAccountAsync(accountId);
    //     return result;
    // }
    //
    // [Authorize]
    // [HttpGet("group/{groupId}")]
    // public async Task<List<VacancyDto>> GetVacanciesByGroupIdAsync(long groupId)
    // {
    //     var result = await vacancyService.GetVacanciesByGroupAsync(groupId);
    //     return result;
    // }
    
    [Authorize]
    [HttpGet("account/{accountId}")]
    public async Task<IActionResult> GetVacanciesByAccountIdPagedAsync(long accountId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var result = await vacancyService.GetVacanciesByAccountPagedAsync(accountId, page, pageSize);
        return Ok(result);
    }
    
    [Authorize]
    [HttpGet("group/{groupId}")]
    public async Task<IActionResult> GetVacanciesByGroupIdAsync(long groupId, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
    {
        var result = await vacancyService.GetVacanciesByGroupPagedAsync(groupId, page, pageSize);
        return Ok(result);
    }

    [Authorize]
    [HttpGet("{vacancyId}")]
    public async Task<IActionResult> GetVacancyByIdAsync(long vacancyId)
    {
        var result = await vacancyService.GetByIdAsync(vacancyId);
        return Ok(result);
    }

    [Authorize]
    [HttpPost("create/{groupId}")]
    public async Task<IActionResult> CreateAsync(long groupId, [FromBody] CreateVacancyDto dto)
    {
        var result = await vacancyService.CreateAsync(this.User, groupId, dto);

        if (!result)
            return StatusCode(result.StatusCode, result.Error);
        
        return CreatedAtAction("GetVacancyById", "Vacancy", new { vacancyId = result.Data }, result.Data);
    }

    [Authorize]
    [HttpPut("update/{vacancyId}")]
    public async Task<IActionResult> UpdateAsync(long vacancyId, [FromBody] UpdateVacancyDto dto)
    {
        var result = await vacancyService.UpdateAsync(this.User, vacancyId, dto);
        
        if (!result)
            return StatusCode(result.StatusCode, result.Error);
        
        return NoContent();
    }

    [Authorize]
    [HttpDelete("delete/{vacancyId}")]
    public async Task<IActionResult> DeleteAsync(long vacancyId)
    {
        var result = await vacancyService.DeleteAsync(this.User, vacancyId);
        
        if (!result)
            return StatusCode(result.StatusCode, result.Error);
        
        return NoContent();
    }
}