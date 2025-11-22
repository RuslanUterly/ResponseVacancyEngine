using ResponseVacancyEngine.Persistence.Models;

namespace ResponseVacancyEngine.Persistence.Interfaces;

public interface IVacancyRepository : IBaseRepository<RespondedVacancy>
{
    Task<List<RespondedVacancy>> GetVacanciesAsync(long accountId);
    Task<List<RespondedVacancy>> GetVacanciesByGroupAsync(long groupId);
    Task<RespondedVacancy> GetVacancyAsync(long vacancyId);
}