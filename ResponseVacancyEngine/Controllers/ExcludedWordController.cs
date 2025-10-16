using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResponseVacancyEngine.Application.DTOs.ExcludedWord;
using ResponseVacancyEngine.Application.Interfaces;

namespace ResponseVacancyEngine.Controllers;

[ApiController]
[Authorize]
[Route("[controller]")]
public class ExcludedWordController(
    IExcludedWordService wordService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetExcludedWordsByGroupAsync(long groupId)
    {
        var result = await wordService.GetExcludedWordsByGroupAsync(groupId);
        return Ok(result);
    }

    [HttpGet("{wordId}")]
    public async Task<IActionResult> GetWordsByIdAsync(long wordId)
    {
        var result = await wordService.GetByIdAsync(wordId);
        return Ok(result);
    }

    [HttpPost("create/{groupId}")]
    public async Task<IActionResult> CreateAsync(long groupId, [FromBody] CreateExcludedWordDto dto)
    {
        var result = await wordService.CreateAsync(this.User, groupId, dto);

        if (!result)
            return StatusCode(result.StatusCode, result.Error);
        
        return CreatedAtAction("GetWordsById", "ExcludedWord", new { wordId = result.Data }, result.Data);
    }

    [HttpPut("update/{wordId}")]
    public async Task<IActionResult> UpdateAsync(long wordId, [FromBody] UpdateExcludedWordDto dto)
    {
        var result = await wordService.UpdateAsync(this.User, wordId, dto);
        
        if (!result)
            return StatusCode(result.StatusCode, result.Error);
        
        return NoContent();
    }

    [HttpDelete("delete/{wordId}")]
    public async Task<IActionResult> DeleteAsync(long wordId)
    {
        var result = await wordService.DeleteAsync(this.User, wordId);
        
        if (!result)
            return StatusCode(result.StatusCode, result.Error);
        
        return NoContent();
    }
}