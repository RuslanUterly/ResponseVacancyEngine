using System.Security.Claims;
using ResponseVacancyEngine.Application.DTOs;
using ResponseVacancyEngine.Application.Helpers.ResultPattern;

namespace ResponseVacancyEngine.Application.Services.Profile.Interfaces;

public interface IProfileService
{
    Task<Result<AccountDto>> GetCurrentAccountAsync(ClaimsPrincipal user);
    Task<Result> UpdateHeadHunterClientCredentialsAsync(ClaimsPrincipal user,
        HeadHunterClientCredentialsDto dto);
    Task<Result> UpdateHeadHunterJwtCredentialsAsync(ClaimsPrincipal user,
        HeadHunterJwtCredentialsDto dto);
    Task<Result> UpdateHeadHunterActiveResponse(ClaimsPrincipal user, bool isActive);
    Task<Result<bool>> ExchangeHeadHunterCodeAsync(ClaimsPrincipal user, string code);
    Task<Result<bool>> RefreshTokenAsync(ClaimsPrincipal user);
}