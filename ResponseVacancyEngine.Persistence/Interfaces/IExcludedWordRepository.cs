using ResponseVacancyEngine.Persistence.Models;

namespace ResponseVacancyEngine.Persistence.Interfaces;

public interface IExcludedWordRepository : IBaseRepository<ExcludedWord>
{
    Task<List<ExcludedWord>> GetWordsByGroup(long groupId);
    Task<ExcludedWord?> GetByIdAsync(long id);
    Task<ExcludedWord?> GetByIdWithGroupAsync(long id);
    Task<bool> ExistsByCategoryAndGroup(long groupId, string category);
}