using System.Security.Claims;
using ResponseVacancyEngine.Application.DTOs.RespondedVacancy;
using ResponseVacancyEngine.Application.Helpers.ResultPattern;

namespace ResponseVacancyEngine.Application.Services;

public interface IVacancyService
{
    Task<List<VacancyDto>> GetVacanciesByAccountAsync(long accountId);
    Task<List<VacancyDto>> GetVacanciesByGroupAsync(long groupId);
    Task<VacancyDto> GetByIdAsync(long vacancyId);
    Task<Result<long>> CreateAsync(ClaimsPrincipal user, long groupId, CreateVacancyDto dto);
    Task<Result<bool>> UpdateAsync(ClaimsPrincipal user, long vacancyId, UpdateVacancyDto dto);
    Task<Result<bool>> DeleteAsync(ClaimsPrincipal user, long vacancyId);
}