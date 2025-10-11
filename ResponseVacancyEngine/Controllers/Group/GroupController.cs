using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResponseVacancyEngine.Application.DTOs.Group;
using ResponseVacancyEngine.Application.Services.Group.Interfaces;

namespace ResponseVacancyEngine.Controllers.Group;

[ApiController]
[Route("[controller]")]
public class GroupController(IGroupService groupService) : ControllerBase
{
    [Authorize]
    [HttpGet("my-groups")]
    public async Task<List<GroupDto>> GetGroupsByAccountIdAsync(long accountId)
    {
        var result = await groupService.GetGroupsByAccountIdAsync(accountId);
        return result;
    }

    [Authorize]
    [HttpGet("my-groups/{groupId}")]
    public async Task<GroupDto> GetGroupByIdAsync(long groupId)
    {
        var result = await groupService.GetByIdAsync(groupId);
        return result;
    }

    [Authorize]
    [HttpPost("create-group")]
    public async Task<IActionResult> CreateAsync([FromBody] GroupDto dto)
    {
        var result = await groupService.CreateAsync(this.User, dto);

        if (!result)
            return StatusCode(result.StatusCode, result.Error);
        
        return CreatedAtAction("GetGroupById", "Group", new { groupId = result.Data }, result.Data);
    }

    [Authorize]
    [HttpPut("update-group/{groupId}")]
    public async Task<IActionResult> UpdateAsync(long groupId, [FromBody] GroupDto dto)
    {
        var result = await groupService.UpdateAsync(this.User, groupId, dto);
        
        if (!result)
            return StatusCode(result.StatusCode, result.Error);
        
        return NoContent();
    }

    [Authorize]
    [HttpDelete("delete-group/{groupId}")]
    public async Task<IActionResult> DeleteAsync(long groupId)
    {
        var result = await groupService.DeleteAsync(this.User, groupId);
        
        if (!result)
            return StatusCode(result.StatusCode, result.Error);
        
        return NoContent();
    }
}