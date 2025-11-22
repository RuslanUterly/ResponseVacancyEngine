using System.Net.Http.Headers;
using System.Net.Http.Json;
using ResponseVacancyEngine.Application.DTOs.HttpResponse;
using ResponseVacancyEngine.Application.DTOs.Profile;
using ResponseVacancyEngine.Application.Interfaces.CryptoHelper;
using ResponseVacancyEngine.Application.Interfaces.Services.HeadHunterApi;
using ResponseVacancyEngine.Infrastructure.Options.HeadHunter;
using ResponseVacancyEngine.Persistence.Models;

namespace ResponseVacancyEngine.Infrastructure.Services.HeadHunterAPI;

public class HhVacancyClient(
    IHttpClientFactory httpClientFactory, 
    ICryptoHelper cryptoHelper) : IHhVacancyClient
{
    public async Task<HttpResponse<HhVacanciesResponse>> GetVacanciesAsync(Account account, Group group, int page = 0)
    {
        var httpClient = httpClientFactory.CreateClient();
        
        if (string.IsNullOrEmpty(account.AccessToken))
        {
            return new HttpResponse<HhVacanciesResponse>()
            {
                IsSuccessStatusCode = false,
                Message = $"Ошибка при получении резюме: access_token не найден",
            };
        }
        
        var accessToken = cryptoHelper.Decrypt(account.AccessToken);
        
        httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", accessToken);
        
        httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("ResponseVacancyEngine/1.0 (+http://localhost:5115)");
        
        var dateFrom = DateTime.UtcNow.AddDays(-1).Date.ToString("yyyy-MM-dd'T'00:00:00");
        var encodedText = Uri.EscapeDataString(group.Settings?.Text ?? "");

        var response = await httpClient
            .GetAsync($"https://api.hh.ru/vacancies?text={encodedText}&experience={group.Settings?.Experience}&schedule={group.Settings?.Schedule}&archived=false&date_from={dateFrom}&page={page}");
        
        if (!response.IsSuccessStatusCode)
        {
            return new HttpResponse<HhVacanciesResponse>()
            {
                IsSuccessStatusCode = false,
                Message = $"Ошибка при получении вакансий: {response.StatusCode}",
            };
        }

        var result = await response.Content.ReadFromJsonAsync<HhVacanciesResponse>();

        return new HttpResponse<HhVacanciesResponse>
        {
            IsSuccessStatusCode = true,
            Data = result ?? new HhVacanciesResponse()
        };
    }
    
    public async Task<HttpResponse<bool>> RespondAsync(string vacancyId, Account account, Group group)
    {
        var httpClient = httpClientFactory.CreateClient();
        
        if (string.IsNullOrEmpty(account.AccessToken))
        {
            return new HttpResponse<bool>()
            {
                IsSuccessStatusCode = false,
                Message = $"Ошибка при получении резюме: access_token не найден",
            };
        }
        
        var accessToken = cryptoHelper.Decrypt(account.AccessToken);
        
        httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", accessToken);
        
        httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("ResponseVacancyEngine/1.0 (+http://localhost:5115)");
        
        var content = new FormUrlEncodedContent(new Dictionary<string, string?>
        {
            ["vacancy_id"] = vacancyId,
            ["resume_id"] = group.ResumeId,
            ["message"] = group.Message
        });
    
        var response = await httpClient.PostAsync("https://api.hh.ru/negotiations", content);
        
        if (!response.IsSuccessStatusCode)
        {
            return new HttpResponse<bool>()
            {
                IsSuccessStatusCode = false,
                Message = $"Ошибка при отклике: {response.StatusCode}",
            };
        }
    
        var result = await response.Content.ReadAsStringAsync();

        return new HttpResponse<bool>()
        {
            IsSuccessStatusCode = true,
            Data = true
        };
    }
}