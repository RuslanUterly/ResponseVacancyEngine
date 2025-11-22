using System.Security.Claims;
using Mapster;
using Microsoft.AspNetCore.Identity;
using ResponseVacancyEngine.Application.DTOs;
using ResponseVacancyEngine.Application.DTOs.RespondedVacancy;
using ResponseVacancyEngine.Application.Helpers.ResultPattern;
using ResponseVacancyEngine.Application.Interfaces.Services;
using ResponseVacancyEngine.Persistence.Interfaces;
using ResponseVacancyEngine.Persistence.Models;
using ResponseVacancyEngine.Persistence.Models.Enums;

namespace ResponseVacancyEngine.Application.Services;

public class VacancyService(
    UserManager<Account> userManager,
    IGroupRepository groupRepository,
    IVacancyRepository vacancyRepository) : IVacancyService
{
    public async Task<PagedResult<RespondedVacancy>> GetVacanciesByAccountPagedAsync(long accountId, int page, int pageSize)
    {
        var vacancies = await vacancyRepository.GetVacanciesByAccountPagedAsync(accountId, page, pageSize);

        return vacancies;
    }

    public async Task<PagedResult<RespondedVacancy>> GetVacanciesByGroupPagedAsync(long groupId, int page, int pageSize)
    {
        var vacancies = await vacancyRepository.GetVacanciesByGroupPagedAsync(groupId, page, pageSize);

        return vacancies;
    }
    
    public async Task<VacancyDto> GetByIdAsync(long vacancyId)
    {
        var vacancy = await vacancyRepository.GetByIdAsync(vacancyId);

        return vacancy?.Adapt<VacancyDto>()!;
    }

    public async Task<bool> ExistsAsync(long groupId, long vacancyId)
    {
        var exists = await vacancyRepository.ExistsAsync(groupId, vacancyId);
        return exists;
    }

    public async Task<Result<long>> CreateAsync(ClaimsPrincipal user, long groupId, CreateVacancyDto dto)
    {
        var account = await userManager.GetUserAsync(user);
        
        if (account == null) 
            return Result<long>.NotFound("Пользователь не найден");
        
        var group = await groupRepository.GetByIdAsync(groupId);

        if (group == null)
            return Result<long>.NotFound("Группы не существует");

        var vacancy = dto.Adapt<RespondedVacancy>();
        vacancy.ResponseDate = DateTime.Now;
        vacancy.AccountId = account.Id;
        vacancy.GroupId = groupId;
        
        var vacancyId = await vacancyRepository.CreateAsync(vacancy);

        if (vacancyId == 0)
            return Result<long>.BadRequest("Не удалось создать вакансию");
        
        return Result<long>.Created(vacancyId);
    }

    public async Task<Result<long>> CreateInternalAsync(Account account, Group group, CreateVacancyDto dto)
    {
        if (account == null) 
            return Result<long>.NotFound("Пользователь не найден");
        
        if (group == null)
            return Result<long>.NotFound("Группы не существует");
        
        var vacancy = dto.Adapt<RespondedVacancy>();
        vacancy.ResponseDate = DateTime.UtcNow;
        vacancy.AccountId = account.Id;
        vacancy.GroupId = group.Id;
        
        var vacancyId = await vacancyRepository.CreateAsync(vacancy);

        if (vacancyId == 0)
            return Result<long>.BadRequest("Не удалось создать вакансию");
        
        return Result<long>.Created(vacancyId);
    }

    public async Task<Result<bool>> UpdateAsync(ClaimsPrincipal user, long vacancyId, UpdateVacancyDto dto)
    {
        var account = await userManager.GetUserAsync(user);
        
        if (account == null) 
            return Result<bool>.NotFound("Пользователь не найден");
        
        var vacancy = await vacancyRepository.GetByIdAsync(vacancyId);
        
        if (vacancy == null)
            return Result<bool>.NotFound("Вакансия не найдена");
        
        var roles = await userManager.GetRolesAsync(account);
        var isAdmin = roles.Contains(Roles.Admin);

        if (vacancy.AccountId != account.Id && !isAdmin)
            return Result<bool>.Forbidden("Недостаточно прав для редактирования вакансии");
        
        dto.Adapt(vacancy);
        
        var isUpdated = await vacancyRepository.UpdateAsync(vacancy);

        if (!isUpdated)
            return Result<bool>.BadRequest("Ошибка обновления вакансии");
        
        return Result<bool>.NoContent();
    }

    public async Task<Result<bool>> UpdateStatusAsync(ClaimsPrincipal user, long vacancyId, Status status)
    {
        var account = await userManager.GetUserAsync(user);
        
        if (account == null) 
            return Result<bool>.NotFound("Пользователь не найден");
        
        var vacancy = await vacancyRepository.GetByIdAsync(vacancyId);
        
        if (vacancy == null)
            return Result<bool>.NotFound("Вакансия не найдена");
        
        var roles = await userManager.GetRolesAsync(account);
        var isAdmin = roles.Contains(Roles.Admin);

        if (vacancy.AccountId != account.Id && !isAdmin)
            return Result<bool>.Forbidden("Недостаточно прав для редактирования вакансии");
        
        vacancy.Status = status;
        
        var isUpdated = await vacancyRepository.UpdateAsync(vacancy);

        if (!isUpdated)
            return Result<bool>.BadRequest("Ошибка обновления вакансии");
        
        return Result<bool>.NoContent();
    }

    public async Task<Result<bool>> UpdateStatusInternalAsync(Account account, long vacancyId, Status status)
    {
        if (account == null) 
            return Result<bool>.NotFound("Пользователь не найден");
        
        var vacancy = await vacancyRepository.GetByIdAsync(vacancyId);

        if (vacancy == null)
            return Result<bool>.NotFound("Вакансия не найдена");
        
        var roles = await userManager.GetRolesAsync(account);
        var isAdmin = roles.Contains(Roles.Admin);

        if (vacancy.AccountId != account.Id && !isAdmin)
            return Result<bool>.Forbidden("Недостаточно прав для редактирования вакансии");
        
        vacancy.Status = status;
        
        var isUpdated = await vacancyRepository.UpdateAsync(vacancy);

        if (!isUpdated)
            return Result<bool>.BadRequest("Ошибка обновления вакансии");
        
        return Result<bool>.NoContent();
    }

    public async Task<Result<bool>> DeleteAsync(ClaimsPrincipal user, long vacancyId)
    {
        var account = await userManager.GetUserAsync(user);
        
        if (account == null) 
            return Result<bool>.NotFound("Пользователь не найден");
        
        var vacancy = await vacancyRepository.GetByIdAsync(vacancyId);
        
        if (vacancy == null)
            return Result<bool>.NotFound("Вакансия не найдена");
        
        var roles = await userManager.GetRolesAsync(account);
        var isAdmin = roles.Contains(Roles.Admin);

        if (vacancy.AccountId != account.Id && !isAdmin)
            return Result<bool>.Forbidden("Недостаточно прав для удаления вакансии");
        
        var isRemoved = await vacancyRepository.DeleteAsync(vacancy);
        
        if(!isRemoved)
            return Result<bool>.BadRequest("Ошибка удаления вакансии");
        
        return Result<bool>.NoContent();
    }
}