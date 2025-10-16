using Microsoft.EntityFrameworkCore;
using ResponseVacancyEngine.Persistence.Interfaces;
using ResponseVacancyEngine.Persistence.Models;

namespace ResponseVacancyEngine.Infrastructure.Persistence.DataAccess;

public class ExcludedWordRepository(VacancyContext context) : IExcludedWordRepository
{
    public async Task<List<ExcludedWord>> GetWordsByGroup(long groupId)
    {
        return await context.ExcludedWords
            .Where(x => x.GroupId == groupId)
            .ToListAsync();
    }

    public async Task<ExcludedWord?> GetByIdAsync(long id)
    {
        return await context.ExcludedWords
            .FirstOrDefaultAsync(x => x.Id == id);
    }
    
    public async Task<ExcludedWord?> GetByIdWithGroupAsync(long id)
    {
        return await context.ExcludedWords
            .Include(x => x.Group)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<bool> ExistsByCategoryAndGroup(long groupId, string category)
    {
        return await context.ExcludedWords
            .Where(x => x.GroupId == groupId)
            .AnyAsync(x => x.Category == category);
    }
    
    public async Task<long> CreateAsync(ExcludedWord data)
    {
        await context.AddAsync(data);
        await SaveAsync();
        
        return data.Id;
    }

    public async Task<bool> UpdateAsync(ExcludedWord data)
    {
        ExcludedWord dbWord = context.ExcludedWords
            .Single(g => g.Id == data.Id);
        
        context.Entry(dbWord).CurrentValues.SetValues(data);
        
        return await SaveAsync();
    }

    public Task<bool> DeleteAsync(ExcludedWord data)
    {
        context.Remove(data);
        return SaveAsync();
    }
    
    public async Task<bool> SaveAsync() => await context.SaveChangesAsync() > 0;
}