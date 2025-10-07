using System.Security.Claims;
using ResponseVacancyEngine.Application.DTOs;
using ResponseVacancyEngine.Application.Helpers.ResultPattern;

namespace ResponseVacancyEngine.Application.Services.Profile.Interfaces;

public interface IProfileService
{
    Task<Result<AccountDto>> GetCurrentAccountAsync(ClaimsPrincipal user);

    Task<Result> AddHeadHunterClientCredentialsAsync(ClaimsPrincipal user,
        HeadHunterClientCredentialsDto dto);
}