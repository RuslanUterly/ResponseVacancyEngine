using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using ResponseVacancyEngine.Application.DTOs.HttpResponse;
using ResponseVacancyEngine.Application.DTOs.Profile;
using ResponseVacancyEngine.Application.Interfaces.CryptoHelper;
using ResponseVacancyEngine.Application.Interfaces.Services.HeadHunterApi;
using ResponseVacancyEngine.Infrastructure.Options.HeadHunter;
using ResponseVacancyEngine.Persistence.Models;

namespace ResponseVacancyEngine.Infrastructure.Services.HeadHunterAPI;

public class HhOAuthClient(
    IHttpClientFactory httpClientFactory,
    ICryptoHelper cryptoHelper,
    IOptions<HhUriOptions> uriOptions,
    IOptions<HhAuthOptions> authOptions) : IHhOAuthClient
{
    public string Login()
    {
        return uriOptions.Value.AuthUri
            .Replace("{redirectUri}", uriOptions.Value.RedirectUri) 
            .Replace("{clientId}", authOptions.Value.ClientId);
    }
    
    public async Task<HttpResponse<HhJwtCredentialsDto>> GetTokensAsync(Account account, string code)
    {
        var httpClient = httpClientFactory.CreateClient();
        
        var content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            {"grant_type", "authorization_code"},
            {"client_id", authOptions.Value.ClientId},
            {"client_secret", authOptions.Value.ClientSecret},
            {"code", code},
            {"redirect_uri", uriOptions.Value.RedirectUri}
        });

        var response = await httpClient.PostAsync(uriOptions.Value.TokenUri, content);

        if (!response.IsSuccessStatusCode)
        {
            return new HttpResponse<HhJwtCredentialsDto>()
            {
                IsSuccessStatusCode = false,
                Message = "Ошибка получения токенов",
            };
        }

        var tokens = await response.Content.ReadFromJsonAsync<HhJwtCredentialsDto>();
        
        return new HttpResponse<HhJwtCredentialsDto>()
        {
            Data = tokens,
        };
    }

    public async Task<HttpResponse<HhJwtCredentialsDto>> RefreshTokensAsync(Account account)
    {
        var httpClient = httpClientFactory.CreateClient();
        
        if (string.IsNullOrEmpty(account.RefreshToken))
        {
            return new HttpResponse<HhJwtCredentialsDto>()
            {
                IsSuccessStatusCode = false,
                Message = $"Ошибка при получении резюме: refresh_token не найден",
            };
        }

        var content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            {"grant_type", "refresh_token"},
            {"refresh_token", cryptoHelper.Decrypt(account.RefreshToken)},
            {"client_id", authOptions.Value.ClientId},
            {"client_secret", authOptions.Value.ClientSecret}
        });

        var response = await httpClient.PostAsync(uriOptions.Value.TokenUri, content);
        
        if (!response.IsSuccessStatusCode)
        {
            return new HttpResponse<HhJwtCredentialsDto>()
            {
                IsSuccessStatusCode = false,
                Message = "Ошибка обновления токенов",
            };
        }

        var tokens = await response.Content.ReadFromJsonAsync<HhJwtCredentialsDto>();

        return new HttpResponse<HhJwtCredentialsDto>()
        {
            Data = tokens,
        };
    }
}