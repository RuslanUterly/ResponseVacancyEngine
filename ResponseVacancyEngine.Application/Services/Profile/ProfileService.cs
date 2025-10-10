using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using ResponseVacancyEngine.Application.DTOs;
using ResponseVacancyEngine.Application.Helpers.ResultPattern;
using ResponseVacancyEngine.Application.Infrastructure.Interfaces.CryptoHelper;
using ResponseVacancyEngine.Application.Infrastructure.Interfaces.Services.HeadHunterApi;
using ResponseVacancyEngine.Application.Services.Profile.Interfaces;
using ResponseVacancyEngine.Persistence.Models;

namespace ResponseVacancyEngine.Application.Services.Profile;

public class ProfileService(
    UserManager<Account> userManager, 
    ICryptoHelper cryptoHelper,
    IHeadHunterOAuthClient oAuthClient) : IProfileService
{
    public async Task<Result<AccountDto>> GetCurrentAccountAsync(ClaimsPrincipal user)
    {
        var account = await userManager.GetUserAsync(user);
        
        if (account is null)
            return Result<AccountDto>.NotFound("Пользователь не найден");
        
        return Result<AccountDto>.Ok(new AccountDto
        {
            Id = account.Id,
            Email = account.Email,
            ClientId = cryptoHelper.Decrypt(account.ClientId),
            ClientSecret = cryptoHelper.Decrypt(account.ClientSecret)
        });
    }

    public async Task<Result> UpdateHeadHunterClientCredentialsAsync(ClaimsPrincipal user,
        HeadHunterClientCredentialsDto dto)
    {
        var account = await userManager.GetUserAsync(user);
        
        if (account is null)
            return Result.NotFound("Пользователь не найден");

        account.ClientId = cryptoHelper.Encrypt(dto.ClientId);
        account.ClientSecret = cryptoHelper.Encrypt(dto.ClientSecret);
        
        var result = await userManager.UpdateAsync(account);
        
        if (!result.Succeeded)
            return Result.BadRequest("Ошибка! ID и Secret не были добавлены");
        
        return Result.Ok();
    }

    public async Task<Result> UpdateHeadHunterJwtCredentialsAsync(ClaimsPrincipal user,
        HeadHunterJwtCredentialsDto dto)
    {
        var account = await userManager.GetUserAsync(user);

        if (account is null)
            return Result.NotFound("Пользователь не найден");
        
        account.AccessToken = cryptoHelper.Encrypt(dto.AccessToken);
        account.RefreshToken = cryptoHelper.Encrypt(dto.RefreshToken);
        account.AccessTokenExpiresAt = DateTime.UtcNow.AddSeconds(dto.ExpiresIn);
        
        var result = await userManager.UpdateAsync(account);
        
        if (!result.Succeeded)
            return Result.BadRequest("Ошибка! токены не были добавлены");
        
        return Result.Ok();
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
        
        return Result.Ok();
    }
    
    public async Task<Result<bool>> ExchangeHeadHunterCodeAsync(ClaimsPrincipal user, string code)
    {
        var account = await userManager.GetUserAsync(user);
        
        if (account is null)
            return Result<bool>.NotFound("Пользователь не найден");
        
        var response = await oAuthClient.GetTokensAsync(account, code);

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
        
        var response = await oAuthClient.RefreshTokensAsync(account);

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