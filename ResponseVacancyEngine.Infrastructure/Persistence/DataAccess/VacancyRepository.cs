using Microsoft.EntityFrameworkCore;
using ResponseVacancyEngine.Application.DTOs;
using ResponseVacancyEngine.Persistence.Interfaces;
using ResponseVacancyEngine.Persistence.Models;

namespace ResponseVacancyEngine.Infrastructure.Persistence.DataAccess;

public class VacancyRepository(VacancyContext context) : IVacancyRepository
{
    public async Task<List<RespondedVacancy>> GetVacanciesByAccountAsync(long accountId)
    {
        return await context.RespondedVacancies
            .Where(g => g.AccountId == accountId)
            .ToListAsync();
    }

    public async Task<List<RespondedVacancy>> GetVacanciesByGroupAsync(long groupId)
    {
        return await context.RespondedVacancies
            .Where(g => g.GroupId == groupId)
            .ToListAsync();
    }
    
    public async Task<PagedResult<RespondedVacancy>> GetVacanciesByAccountPagedAsync(long accountId, int page, int pageSize)
    {
        var query = context.RespondedVacancies
            .Where(g => g.AccountId == accountId)
            .OrderByDescending(v => v.ResponseDate);

        var totalCount = await query.CountAsync();

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<RespondedVacancy>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

    public async Task<PagedResult<RespondedVacancy>> GetVacanciesByGroupPagedAsync(long groupId, int page, int pageSize)
    {
        var query = context.RespondedVacancies
            .Where(g => g.GroupId == groupId)
            .OrderByDescending(v => v.ResponseDate);

        var totalCount = await query.CountAsync();

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<RespondedVacancy>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }


    public async Task<RespondedVacancy?> GetByIdAsync(long vacancyId)
    {
        return await context.RespondedVacancies
            .FirstOrDefaultAsync(v => v.Id == vacancyId);
    }

    public async Task<bool> ExistsAsync(long groupId, long vacancyId)
    {
        return await context.RespondedVacancies
            .AnyAsync(g => g.GroupId == groupId && g.VacancyId == vacancyId);
    }

    public async Task<long> CreateAsync(RespondedVacancy data)
    {
        await context.AddAsync(data);
        await SaveAsync();
        
        return data.Id;
    }

    public async Task<bool> UpdateAsync(RespondedVacancy data)
    {
        RespondedVacancy dbVacancy = context.RespondedVacancies
            .Single(g => g.Id == data.Id);
        
        context.Entry(dbVacancy).CurrentValues.SetValues(data);
        
        return await SaveAsync();
    }

    public async Task<bool> DeleteAsync(RespondedVacancy data)
    {
        context.Remove(data);
        return await SaveAsync();
    }

    public async Task<bool> SaveAsync() => await context.SaveChangesAsync() > 0;
}