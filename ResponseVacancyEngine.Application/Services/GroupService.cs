using System.Security.Claims;
using Mapster;
using Microsoft.AspNetCore.Identity;
using ResponseVacancyEngine.Application.DTOs.Group;
using ResponseVacancyEngine.Application.Helpers.ResultPattern;
using ResponseVacancyEngine.Application.Interfaces;
using ResponseVacancyEngine.Application.Mapping;
using ResponseVacancyEngine.Persistence.Interfaces;
using ResponseVacancyEngine.Persistence.Models;

namespace ResponseVacancyEngine.Application.Services;

public class GroupService(
    UserManager<Account> userManager,
    IGroupRepository groupRepository) : IGroupService
{
    public async Task<List<GroupDto>> GetGroupsByAccountIdAsync(long accountId)
    {
        var groups = await groupRepository.GetGroupsByAccount(accountId);
        
        return groups?.Adapt<List<GroupDto>>()!;
    }

    public async Task<GroupDto> GetByIdAsync(long groupId)
    {
        var group = await groupRepository.GetByIdAsync(groupId);

        return group?.Adapt<GroupDto>()!;
    }

    public async Task<Result<long>> CreateAsync(ClaimsPrincipal user, CreateGroupDto dto)
    {
        var account = await userManager.GetUserAsync(user);
        
        if (account == null) 
            return Result<long>.BadRequest("Пользователь не найден");
        
        var group = dto.Adapt<Group>();
        group.AccountId = account.Id;
        
        var groupId = await groupRepository.CreateAsync(group);

        if (groupId == 0)
            return Result<long>.BadRequest("Не удалось создать группу");
        
        return Result<long>.Created(groupId);
    }

    public async Task<Result<bool>> UpdateAsync(ClaimsPrincipal user, long groupId, UpdateGroupDto dto)
    {
        var account = await userManager.GetUserAsync(user);
        
        if (account == null) 
            return Result<bool>.BadRequest("Пользователь не найден");
        
        var group = await groupRepository.GetByIdAsync(groupId);

        if (group == null)
            return Result<bool>.NotFound("Группа не найдена");
        
        var roles = await userManager.GetRolesAsync(account);
        var isAdmin = roles.Contains(Roles.Admin);

        if (group.AccountId != account.Id && !isAdmin)
            return Result<bool>.Forbidden("Недостаточно прав для редактирования группы");

        dto.Adapt(group);

        var isUpdated = await groupRepository.UpdateAsync(group);

        if (!isUpdated)
            return Result<bool>.BadRequest("Ошибка обновления группы");
        
        return Result<bool>.NoContent();
    }

    public async Task<Result<bool>> DeleteAsync(ClaimsPrincipal user, long groupId)
    {
        var account = await userManager.GetUserAsync(user);
        
        if (account == null) 
            return Result<bool>.BadRequest("Пользователь не найден");
        
        var group = await groupRepository.GetByIdAsync(groupId);

        if (group == null)
            return Result<bool>.NotFound("Группа не найдена");
        
        var roles = await userManager.GetRolesAsync(account);
        var isAdmin = roles.Contains(Roles.Admin);

        if (group.AccountId != account.Id && !isAdmin)
            return Result<bool>.Forbidden("Недостаточно прав для удаления группы");
        
        var isRemoved = await groupRepository.DeleteAsync(group);

        if (!isRemoved)
            return Result<bool>.BadRequest("Ошибка удаления группы");
        
        return Result<bool>.NoContent();
    }
}