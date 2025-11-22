using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using ResponseVacancyEngine.Application.DTOs.HttpResponse;
using ResponseVacancyEngine.Application.DTOs.Profile;
using ResponseVacancyEngine.Application.Interfaces.CryptoHelper;
using ResponseVacancyEngine.Application.Interfaces.Services.HeadHunterApi;
using ResponseVacancyEngine.Infrastructure.Options.HeadHunter;
using ResponseVacancyEngine.Persistence.Models;

namespace ResponseVacancyEngine.Infrastructure.Services.HeadHunterAPI;

public class HhProfileClient(
    IHttpClientFactory httpClientFactory, 
    ICryptoHelper cryptoHelper,
    IOptions<HhUriOptions> options) : IHhProfileClient
{
    public async Task<HttpResponse<List<HhResumeDto>>> GetResumesAsync(Account account)
    {
        var httpClient = httpClientFactory.CreateClient();

        if (string.IsNullOrEmpty(account.AccessToken))
        {
            return new HttpResponse<List<HhResumeDto>>()
            {
                IsSuccessStatusCode = false,
                Message = $"Ошибка при получении резюме: access_token не найден",
            };
        }
        
        var accessToken = cryptoHelper.Decrypt(account.AccessToken);
        
        httpClient.DefaultRequestHeaders.Authorization =
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
        
        httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("ResponseVacancyEngine/1.0 (+http://localhost:5115)");
        
        var response = await httpClient.GetAsync(options.Value.ResumeUri);

        if (!response.IsSuccessStatusCode)
        {
            return new HttpResponse<List<HhResumeDto>>()
            {
                IsSuccessStatusCode = false,
                Message = $"Ошибка при получении резюме: {response.StatusCode}",
            };
        }

        var result = await response.Content.ReadFromJsonAsync<HhResumeListResponse>();

        return new HttpResponse<List<HhResumeDto>>
        {
            IsSuccessStatusCode = true,
            Data = result?.Items ?? new List<HhResumeDto>()
        };
    }
}