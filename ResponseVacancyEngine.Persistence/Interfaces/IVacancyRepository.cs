using ResponseVacancyEngine.Application.DTOs;
using ResponseVacancyEngine.Persistence.Models;

namespace ResponseVacancyEngine.Persistence.Interfaces;

public interface IVacancyRepository : IBaseRepository<RespondedVacancy>
{
    Task<List<RespondedVacancy>> GetVacanciesByAccountAsync(long accountId);
    Task<List<RespondedVacancy>> GetVacanciesByGroupAsync(long groupId);
    Task<PagedResult<RespondedVacancy>> GetVacanciesByAccountPagedAsync(long accountId, int page, int pageSize);
    Task<PagedResult<RespondedVacancy>> GetVacanciesByGroupPagedAsync(long groupId, int page, int pageSize);
    Task<RespondedVacancy?> GetByIdAsync(long vacancyId);
    Task<bool> ExistsAsync(long groupId, long vacancyId);
}