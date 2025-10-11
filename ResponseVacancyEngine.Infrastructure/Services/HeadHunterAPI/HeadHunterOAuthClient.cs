using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using ResponseVacancyEngine.Application.DTOs;
using ResponseVacancyEngine.Application.DTOs.HttpResponse;
using ResponseVacancyEngine.Application.DTOs.Profile;
using ResponseVacancyEngine.Application.Infrastructure.Interfaces.CryptoHelper;
using ResponseVacancyEngine.Application.Infrastructure.Interfaces.Services.HeadHunterApi;
using ResponseVacancyEngine.Infrastructure.Options;
using ResponseVacancyEngine.Persistence.Models;

namespace ResponseVacancyEngine.Infrastructure.Services.HeadHunterAPI;

public class HeadHunterOAuthClient(
    IHttpClientFactory httpClientFactory,
    ICryptoHelper cryptoHelper,
    IOptions<HeadHunterUriOptions> options) : IHeadHunterOAuthClient
{
    public async Task<HttpResponse<HeadHunterJwtCredentialsDto>> GetTokensAsync(Account account, string code)
    {
        var httpClient = httpClientFactory.CreateClient();
        
        var content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            {"grant_type", "authorization_code"},
            {"client_id", cryptoHelper.Decrypt(account.ClientId)},
            {"client_secret", cryptoHelper.Decrypt(account.ClientSecret)},
            {"code", code},
            {"redirect_uri", options.Value.RedirectUri}
        });

        var response = await httpClient.PostAsync(options.Value.TokenUri, content);
        response.EnsureSuccessStatusCode();

        if (!response.IsSuccessStatusCode)
        {
            return new HttpResponse<HeadHunterJwtCredentialsDto>()
            {
                IsSuccessStatusCode = false,
                Message = "Ошибка получения токенов",
            };
        }

        var tokens = await response.Content.ReadFromJsonAsync<HeadHunterJwtCredentialsDto>();
        
        return new HttpResponse<HeadHunterJwtCredentialsDto>()
        {
            Data = tokens,
        };
    }

    public async Task<HttpResponse<HeadHunterJwtCredentialsDto>> RefreshTokensAsync(Account account)
    {
        var httpClient = httpClientFactory.CreateClient();

        var content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            {"grant_type", "refresh_token"},
            {"refresh_token", cryptoHelper.Decrypt(account.RefreshToken)},
            {"client_id", cryptoHelper.Decrypt(account.ClientId)},
            {"client_secret", cryptoHelper.Decrypt(account.ClientSecret)}
        });

        var response = await httpClient.PostAsync(options.Value.TokenUri, content);
        
        if (!response.IsSuccessStatusCode)
        {
            return new HttpResponse<HeadHunterJwtCredentialsDto>()
            {
                IsSuccessStatusCode = false,
                Message = "Ошибка обновления токенов",
            };
        }

        var tokens = await response.Content.ReadFromJsonAsync<HeadHunterJwtCredentialsDto>();

        return new HttpResponse<HeadHunterJwtCredentialsDto>()
        {
            Data = tokens,
        };
    }
}