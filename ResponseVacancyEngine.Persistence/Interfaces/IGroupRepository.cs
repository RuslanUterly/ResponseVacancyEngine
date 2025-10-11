using ResponseVacancyEngine.Persistence.Models;

namespace ResponseVacancyEngine.Persistence.Interfaces;

public interface IGroupRepository : IBaseRepository<Group>
{
    Task<List<Group>> GetGroupsByAccount(long accountId);
    Task<Group?> GetByIdAsync(long id);
}