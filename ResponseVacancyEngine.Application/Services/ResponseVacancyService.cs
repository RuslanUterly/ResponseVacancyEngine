using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ResponseVacancyEngine.Application.DTOs.ExcludedWord;
using ResponseVacancyEngine.Application.DTOs.HttpResponse;
using ResponseVacancyEngine.Application.DTOs.Profile;
using ResponseVacancyEngine.Application.DTOs.RespondedVacancy;
using ResponseVacancyEngine.Application.Interfaces;
using ResponseVacancyEngine.Application.Interfaces.Services;
using ResponseVacancyEngine.Application.Interfaces.Services.HeadHunterApi;
using ResponseVacancyEngine.Persistence.Models;
using ResponseVacancyEngine.Persistence.Models.Enums;

namespace ResponseVacancyEngine.Application.Services;

public class ResponseVacancyService(
    IHhVacancyClient hhVacancyClient,
    IVacancyService vacancyService,
    IExcludedWordService excludedWordService,
    UserManager<Account> userManager
    )
{
    public async Task GetVacancies()
    {
        var accounts = userManager.Users
            .Include(a => a.Groups)
            .Where(a => a.Groups != null && a.Groups.Count > 0 && a.IsActiveResponse)
            .ToList();

        var excludedWordsLookup = await this.GetExcludedWordsByAllAccountsAsync(accounts);
        
        await Parallel.ForEachAsync(accounts, new ParallelOptions { MaxDegreeOfParallelism = 5 }, async (account, ct) =>
        {
            var groups = account.Groups;

            foreach (var group in groups!)
            {
                var suitableVacancies = await this.GetAllVacanciesAsync(account, group);

                if (!suitableVacancies.IsSuccessStatusCode)
                    continue;

                var filteredVacancies = await this.GetFilteredVacanciesAsync(excludedWordsLookup, group.Id, suitableVacancies.Data!.Items);

                foreach (var vacancy in filteredVacancies)
                {
                    long vacancyId = long.Parse(vacancy.Id);
                    
                    if (await vacancyService.ExistsAsync(group.Id, vacancyId))
                        continue;
                    
                    var created = await vacancyService.CreateInternalAsync(account, group, new CreateVacancyDto()
                    {
                        VacancyId = vacancyId,
                        Name = vacancy.Name,
                        Description = vacancy.Snippet.Requirement,
                        Url = vacancy.AlternateUrl,
                        CreatedDate = DateTime.SpecifyKind(vacancy.CreatedAt.Date, DateTimeKind.Utc),
                        Status = Status.Pending,
                    });
                    
                    if (!created)
                        return;
                    
                    var result = await hhVacancyClient.RespondAsync(vacancy.Id, account, group);

                    if (!result.IsSuccessStatusCode)
                        continue;
                    
                    await vacancyService.UpdateStatusInternalAsync(account, created.Data, Status.Responded);
                }
            }
        });
    }

    private Task<List<Item>> GetFilteredVacanciesAsync(ILookup<long, ExcludedWordDto> excludedWordsLookup, long groupId, IEnumerable<Item> items)
    {
        var excludedWordsForHeader = excludedWordsLookup[groupId]
            .Where(w => w.Category == "header")
            .SelectMany(w => w.Words)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
                
        var excludedWordsForBody = excludedWordsLookup[groupId]
            .Where(w => w.Category == "description")
            .SelectMany(w => w.Words)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var filteredVacancies = items
            .Where(v => !excludedWordsForHeader.Any(word => v.Name.Contains(word, StringComparison.OrdinalIgnoreCase)))
            .Where(v => !excludedWordsForBody.Any(word => v.Snippet.Requirement.Contains(word, StringComparison.OrdinalIgnoreCase)))
            .Where(v => !excludedWordsForBody.Any(word => v.Snippet.Responsibility.Contains(word, StringComparison.OrdinalIgnoreCase)))
            .ToList();
        
        return Task.FromResult(filteredVacancies);
    }

    private async Task<ILookup<long,ExcludedWordDto>> GetExcludedWordsByAllAccountsAsync(IEnumerable<Account> accounts)
    {
        var semaphore = new SemaphoreSlim(5);
        var tasks = new List<Task<List<ExcludedWordDto>>>();

        foreach (var account in accounts)
        {
            foreach (var group in account.Groups!)
            {
                tasks.Add(Task.Run(async () =>
                {
                    await semaphore.WaitAsync();
                    try
                    {
                        return await excludedWordService.GetExcludedWordsByGroupAsync(group.Id);
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                }));
            }
        }

        var results = await Task.WhenAll(tasks);

        var allExcludedWords = results.SelectMany(x => x).ToList();

        var excludedWordsLookup = allExcludedWords.ToLookup(w => w.GroupId);

        return excludedWordsLookup;
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