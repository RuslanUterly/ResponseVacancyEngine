using System.Security.Claims;
using ResponseVacancyEngine.Application.DTOs;
using ResponseVacancyEngine.Application.DTOs.RespondedVacancy;
using ResponseVacancyEngine.Application.Helpers.ResultPattern;
using ResponseVacancyEngine.Persistence.Models;
using ResponseVacancyEngine.Persistence.Models.Enums;

namespace ResponseVacancyEngine.Application.Interfaces.Services;

public interface IVacancyService
{
    Task<PagedResult<RespondedVacancy>> GetVacanciesByAccountPagedAsync(long accountId, int page, int pageSize);
    Task<PagedResult<RespondedVacancy>> GetVacanciesByGroupPagedAsync(long groupId, int page, int pageSize);
    Task<VacancyDto> GetByIdAsync(long vacancyId);
    Task<bool> ExistsAsync(long groupId, long vacancyId);
    Task<Result<long>> CreateAsync(ClaimsPrincipal user, long groupId, CreateVacancyDto dto);
    Task<Result<long>> CreateInternalAsync(Account account, Group group, CreateVacancyDto dto);
    Task<Result<bool>> UpdateAsync(ClaimsPrincipal user, long vacancyId, UpdateVacancyDto dto);
    Task<Result<bool>> UpdateStatusAsync(ClaimsPrincipal user, long vacancyId, Status status);
    Task<Result<bool>> UpdateStatusInternalAsync(Account account, long vacancyId, Status status);
    Task<Result<bool>> DeleteAsync(ClaimsPrincipal user, long vacancyId);
}