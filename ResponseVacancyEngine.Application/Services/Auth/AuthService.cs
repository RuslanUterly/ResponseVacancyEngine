using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using ResponseVacancyEngine.Application.Helpers.ResultPattern;
using ResponseVacancyEngine.Application.Infrastructure.Interfaces.JwtProvider;
using ResponseVacancyEngine.Application.Services.Auth.Intefaces;
using ResponseVacancyEngine.Persistence.Interfaces;
using ResponseVacancyEngine.Persistence.Models;

namespace ResponseVacancyEngine.Application.Services.Auth;

public class AuthService(
    UserManager<Account> userManager, 
    SignInManager<Account> signInManager,
    IJwtProvider jwtProvider) : IAuthService
{
    public async Task<Result<string>> LoginAsync(LoginRequest request)
    {
        var account = await userManager.FindByEmailAsync(request.Email);
        
        if (account == null) 
            return Result<string>.Unauthorized("Ошибка авторизации! Проверьте логин или пароль");
        
        var signInResult = await signInManager.CheckPasswordSignInAsync(account, request.Password, false);
        
        if (!signInResult.Succeeded)
            return Result<string>.Unauthorized("Ошибка авторизации! Проверьте логин или пароль");

        var token = jwtProvider.GenerateToken(account);

        return Result<string>.Ok(token);
    }

    public async Task<Result<string>> RegisterAsync(RegisterRequest request)
    {
        if (await userManager.FindByEmailAsync(request.Email) is not null)
            return Result<string>.BadRequest("Пользователь уже зарегистрирован");

        var account = new Account()
        {
            Email = request.Email,
            UserName = request.Email,
        };
        
        var result = await userManager.CreateAsync(account, request.Password);
        
        if (!result.Succeeded)
            return Result<string>.BadRequest("Произошла ошибка"); 
        
        if (!await userManager.IsInRoleAsync(account,"User"))
            await userManager.AddToRoleAsync(account, "User");
        
        return Result<string>.Ok("Регистрация прошла успешно");
    }
}