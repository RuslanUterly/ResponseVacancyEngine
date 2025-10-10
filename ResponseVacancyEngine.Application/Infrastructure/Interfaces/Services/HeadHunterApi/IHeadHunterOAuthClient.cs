using ResponseVacancyEngine.Application.DTOs;
using ResponseVacancyEngine.Application.DTOs.HttpResponse;
using ResponseVacancyEngine.Persistence.Models;

namespace ResponseVacancyEngine.Application.Infrastructure.Interfaces.Services.HeadHunterApi;

public interface IHeadHunterOAuthClient
{
    Task<HttpResponse<HeadHunterJwtCredentialsDto>> GetTokensAsync(Account account, string code);
    Task<HttpResponse<HeadHunterJwtCredentialsDto>> RefreshTokensAsync(Account account);
}