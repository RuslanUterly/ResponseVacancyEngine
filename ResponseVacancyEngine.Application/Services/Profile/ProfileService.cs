using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using ResponseVacancyEngine.Application.DTOs;
using ResponseVacancyEngine.Application.Helpers.ResultPattern;
using ResponseVacancyEngine.Application.Infrastructure.Interfaces.CryptoHelper;
using ResponseVacancyEngine.Application.Services.Profile.Interfaces;
using ResponseVacancyEngine.Persistence.Models;

namespace ResponseVacancyEngine.Application.Services.Profile;

public class ProfileService(
    UserManager<Account> userManager, 
    ICryptoHelper cryptoHelper) : IProfileService
{
    public async Task<Result<AccountDto>> GetCurrentAccountAsync(ClaimsPrincipal user)
    {
        var account = await userManager.GetUserAsync(user);
        
        if (account is null)
            return Result<AccountDto>.NotFound("User not found");
        
        return Result<AccountDto>.Ok(new AccountDto
        {
            Id = account.Id,
            Email = account.Email,
            ClientId = cryptoHelper.Decrypt(account.ClientId),
            ClientSecret = cryptoHelper.Decrypt(account.ClientSecret)
        });
    }

    public async Task<Result> AddHeadHunterClientCredentialsAsync(ClaimsPrincipal user,
        HeadHunterClientCredentialsDto dto)
    {
        var account = await userManager.GetUserAsync(user);
        
        if (account is null)
            return Result.NotFound("User not found");

        account.ClientId = cryptoHelper.Encrypt(dto.ClientId);
        account.ClientSecret = cryptoHelper.Encrypt(dto.ClientSecret);
        
        var result = await userManager.UpdateAsync(account);
        
        if (!result.Succeeded)
            return Result.BadRequest("Ошибка! ID и Secret не были добавлены");
        
        return Result.Ok();
    }
}