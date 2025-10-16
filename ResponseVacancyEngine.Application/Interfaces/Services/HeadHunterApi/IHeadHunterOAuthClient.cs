using ResponseVacancyEngine.Application.DTOs.HttpResponse;
using ResponseVacancyEngine.Application.DTOs.Profile;
using ResponseVacancyEngine.Persistence.Models;

namespace ResponseVacancyEngine.Application.Interfaces.Services.HeadHunterApi;

public interface IHeadHunterOAuthClient
{
    Task<HttpResponse<HeadHunterJwtCredentialsDto>> GetTokensAsync(Account account, string code);
    Task<HttpResponse<HeadHunterJwtCredentialsDto>> RefreshTokensAsync(Account account);
}