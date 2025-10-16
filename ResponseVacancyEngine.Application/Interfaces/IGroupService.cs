using System.Security.Claims;
using ResponseVacancyEngine.Application.DTOs.Group;
using ResponseVacancyEngine.Application.Helpers.ResultPattern;

namespace ResponseVacancyEngine.Application.Interfaces;

public interface IGroupService
{
    Task<List<GroupDto>> GetGroupsByAccountIdAsync(long accountId);
    Task<GroupDto> GetByIdAsync(long groupId);
    Task<Result<long>> CreateAsync(ClaimsPrincipal user, GroupDto dto);
    Task<Result<bool>> UpdateAsync(ClaimsPrincipal user, long groupId, GroupDto dto);
    Task<Result<bool>> DeleteAsync(ClaimsPrincipal user, long groupId);
}