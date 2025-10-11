using Microsoft.EntityFrameworkCore;
using ResponseVacancyEngine.Persistence.Interfaces;
using ResponseVacancyEngine.Persistence.Models;

namespace ResponseVacancyEngine.Infrastructure.Persistence.DataAccess;

public class GroupRepository(VacancyContext context) : IGroupRepository
{
    public async Task<List<Group>> GetGroupsByAccount(long accountId)
    {
        return await context.Groups
            .Where(g => g.AccountId == accountId)
            .ToListAsync();
    }
    
    public async Task<Group?> GetByIdAsync(long id)
    {
        return await context.Groups
            .FirstOrDefaultAsync(g => g.Id == id);
    }
    
    public async Task<long> CreateAsync(Group data)
    {
        await context.AddAsync(data);
        await SaveAsync();
        
        return data.Id;
    }
    
    public async Task<bool> UpdateAsync(Group data)
    {
        Group dbGroup = context.Groups
            .Single(g => g.Id == data.Id);
        
        context.Entry(dbGroup).CurrentValues.SetValues(data);
        
        return await SaveAsync();
    }
    
    public Task<bool> DeleteAsync(Group data)
    {
        context.Remove(data);
        return SaveAsync();
    }
    
    public async Task<bool> SaveAsync() => await context.SaveChangesAsync() > 0;
}