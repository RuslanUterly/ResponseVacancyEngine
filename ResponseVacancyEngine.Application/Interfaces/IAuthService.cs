using Microsoft.AspNetCore.Identity.Data;
using ResponseVacancyEngine.Application.Helpers.ResultPattern;

namespace ResponseVacancyEngine.Application.Interfaces;

public interface IAuthService
{
    Task<Result<string>> LoginAsync(LoginRequest request);
    Task<Result<string>> RegisterAsync(RegisterRequest request);
}