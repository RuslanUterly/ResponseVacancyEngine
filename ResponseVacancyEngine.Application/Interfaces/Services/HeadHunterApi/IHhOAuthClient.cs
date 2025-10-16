using ResponseVacancyEngine.Application.DTOs.HttpResponse;
using ResponseVacancyEngine.Application.DTOs.Profile;
using ResponseVacancyEngine.Persistence.Models;

namespace ResponseVacancyEngine.Application.Interfaces.Services.HeadHunterApi;

public interface IHhOAuthClient
{
    string Login();
    Task<HttpResponse<HhJwtCredentialsDto>> GetTokensAsync(Account account, string code);
    Task<HttpResponse<HhJwtCredentialsDto>> RefreshTokensAsync(Account account);
}