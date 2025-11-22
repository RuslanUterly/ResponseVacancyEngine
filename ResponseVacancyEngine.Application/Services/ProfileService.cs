using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using ResponseVacancyEngine.Application.DTOs.Profile;
using ResponseVacancyEngine.Application.Helpers.ResultPattern;
using ResponseVacancyEngine.Application.Interfaces;
using ResponseVacancyEngine.Application.Interfaces.CryptoHelper;
using ResponseVacancyEngine.Application.Interfaces.Services.HeadHunterApi;
using ResponseVacancyEngine.Persistence.Models;

namespace ResponseVacancyEngine.Application.Services;

public class ProfileService(
    UserManager<Account> userManager,
    ICryptoHelper cryptoHelper,
    IHhProfileClient hhProfileClient,
    IHhOAuthClient hhOAuthClient) : IProfileService
{
    public async Task<Result<AccountDto>> GetCurrentAccountAsync(ClaimsPrincipal user)
    {
        var account = await userManager.GetUserAsync(user);
        
        if (account is null)
            return Result<AccountDto>.NotFound("Пользователь не найден");
        
        //TODO: Перевести на mapster
        return Result<AccountDto>.Ok(new AccountDto
        {
            Id = account.Id,
            Email = account.Email,
            IsActiveResponse = account.IsActiveResponse
        });
    }

    public async Task<List<HhResumeDto>> GetResumesAsync(ClaimsPrincipal user)
    {
        var account = await userManager.GetUserAsync(user);

        if (account is null)
            return [];
        
        var response = await hhProfileClient.GetResumesAsync(account);
        
        if (!response.IsSuccessStatusCode)
            return [];

        return response?.Data!;
    }

    public Task<string> Login()
    {
        return Task.FromResult(hhOAuthClient.Login());
    }

    public async Task<Result> UpdateHeadHunterActiveResponse(ClaimsPrincipal user, bool isActive)
    {
        var account = await userManager.GetUserAsync(user);

        if (account is null)
            return Result.NotFound("Пользователь не найден");
        
        account.IsActiveResponse = isActive;
        
        var result = await userManager.UpdateAsync(account);
        
        if (!result.Succeeded)
            return Result.BadRequest("Ошибка!");
        
        return Result.NoContent();
    }
    
    public async Task<Result<bool>> ExchangeHeadHunterCodeAsync(ClaimsPrincipal user, string code)
    {
        var account = await userManager.GetUserAsync(user);
        
        if (account is null)
            return Result<bool>.NotFound("Пользователь не найден");
        
        var response = await hhOAuthClient.GetTokensAsync(account, code);

        if (!response.IsSuccessStatusCode)
            return Result<bool>.BadRequest(response.Message);
        
        account.AccessToken = cryptoHelper.Encrypt(response.Data!.AccessToken);
        account.RefreshToken = cryptoHelper.Encrypt(response.Data!.RefreshToken);
        account.AccessTokenExpiresAt = DateTime.UtcNow.AddSeconds(response.Data!.ExpiresIn);
        
        var result = await userManager.UpdateAsync(account);
        
        if (!result.Succeeded)
            return Result<bool>.BadRequest("Ошибка! токены не были добавлены");
        
        return Result<bool>.Ok(result.Succeeded);
    }

    public async Task<Result<bool>> RefreshTokenAsync(ClaimsPrincipal user)
    {
        var account = await userManager.GetUserAsync(user);
        
        if (account is null)
            return Result<bool>.NotFound("Пользователь не найден");
        
        if (account.AccessTokenExpiresAt > DateTime.UtcNow)
            return Result<bool>.Ok(true); 
        
        var response = await hhOAuthClient.RefreshTokensAsync(account);

        if (!response.IsSuccessStatusCode)
            return Result<bool>.BadRequest(response.Message);
        
        account.AccessToken = cryptoHelper.Encrypt(response.Data!.AccessToken);
        account.RefreshToken = cryptoHelper.Encrypt(response.Data!.RefreshToken);
        account.AccessTokenExpiresAt = DateTime.UtcNow.AddSeconds(response.Data!.ExpiresIn);
        
        var result = await userManager.UpdateAsync(account);
        
        if (!result.Succeeded)
            return Result<bool>.BadRequest("Ошибка! токены не были добавлены");
        
        return Result<bool>.Ok(result.Succeeded);
    }
}