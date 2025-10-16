using System.Security.Claims;
using ResponseVacancyEngine.Application.DTOs.Profile;
using ResponseVacancyEngine.Application.Helpers.ResultPattern;

namespace ResponseVacancyEngine.Application.Interfaces;

public interface IProfileService
{
    Task<Result<AccountDto>> GetCurrentAccountAsync(ClaimsPrincipal user);
    Task<List<HhResumeDto>> GetResumesAsync(ClaimsPrincipal user);
    Task<Result> UpdateHeadHunterActiveResponse(ClaimsPrincipal user, bool isActive);
    Task<string> Login();
    Task<Result<bool>> ExchangeHeadHunterCodeAsync(ClaimsPrincipal user, string code);
    Task<Result<bool>> RefreshTokenAsync(ClaimsPrincipal user);
}