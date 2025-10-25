using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ResponseVacancyEngine.Application.DTOs.HttpResponse;
using ResponseVacancyEngine.Application.DTOs.Profile;
using ResponseVacancyEngine.Application.Interfaces.Services.HeadHunterApi;
using ResponseVacancyEngine.Persistence.Interfaces;
using ResponseVacancyEngine.Persistence.Models;

namespace ResponseVacancyEngine.Application.Services;

public class ResponseVacancyService(
    IHhVacancyClient hhVacancyClient,
    IGroupRepository groupRepository,
    UserManager<Account> userManager
    )
{
    public async Task GetVacancies()
    {
        var accounts = userManager.Users
            .Include(a => a.Groups)
            .Where(a => a.Groups != null && a.Groups.Count > 0)
            .ToList();
        
        foreach (var account in accounts)
        {
            if (account?.Groups == null)
                continue;
            
            foreach (var accountGroup in account.Groups)
            {
                var result = await hhVacancyClient.GetVacanciesAsync(account, accountGroup);
                
                
            }
        }
        
        await Parallel.ForEachAsync(accounts, new ParallelOptions { MaxDegreeOfParallelism = 5 }, async (account, ct) =>
        {
            var groups = account.Groups;

            foreach (var group in groups!)
            {
                var suitableVacancies = await this.GetAllVacanciesAsync(account, group);

                if (!suitableVacancies.IsSuccessStatusCode)
                    continue;

                foreach (var vacancy in suitableVacancies.Data!.Items)
                {
                    var result = await hhVacancyClient.RespondAsync(vacancy.Id, account, group);

                    if (!result.IsSuccessStatusCode)
                    {
                        
                    }
                }
            }
        });
    }

    private async Task<HttpResponse<HhVacanciesResponse>> GetAllVacanciesAsync(Account account, Group group)
    {
        var suitableVacancies = await hhVacancyClient.GetVacanciesAsync(account, group);

        if (!suitableVacancies.IsSuccessStatusCode)
            return suitableVacancies;

        if (suitableVacancies.Data?.Pages == 1)
            return suitableVacancies;

        for (int page = 1; page < suitableVacancies.Data?.Pages; page++)
        {
            var nextVacancies = await hhVacancyClient.GetVacanciesAsync(account, group, page);

            if (!nextVacancies.IsSuccessStatusCode)
                break;

            if (nextVacancies.Data?.Items != null) 
                suitableVacancies.Data.Items.AddRange(nextVacancies.Data.Items);
        }
        
        return suitableVacancies;
    }
}