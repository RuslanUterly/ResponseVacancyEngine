using System.Security.Claims;
using Mapster;
using Microsoft.AspNetCore.Identity;
using ResponseVacancyEngine.Application.DTOs.ExcludedWord;
using ResponseVacancyEngine.Application.Helpers.ResultPattern;
using ResponseVacancyEngine.Application.Interfaces;
using ResponseVacancyEngine.Persistence.Interfaces;
using ResponseVacancyEngine.Persistence.Models;

namespace ResponseVacancyEngine.Application.Services;

public class ExcludedWordService(
    UserManager<Account> userManager,
    IExcludedWordRepository wordRepository) : IExcludedWordService
{
    public async Task<List<ExcludedWordDto>> GetExcludedWordsByGroupAsync(long groupId)
    {
        var words = await wordRepository.GetWordsByGroup(groupId);
        
        return words?.Adapt<List<ExcludedWordDto>>()!;
    }

    public async Task<ExcludedWordDto> GetByIdAsync(long wordId)
    {
        var word = await wordRepository.GetByIdAsync(wordId);

        return word?.Adapt<ExcludedWordDto>()!;
    }

    public async Task<Result<long>> CreateAsync(ClaimsPrincipal user, long groupId, CreateExcludedWordDto dto)
    {
        var account = await userManager.GetUserAsync(user);
        
        if (account == null) 
            return Result<long>.NotFound("Пользователь не найден");

        var existsByCategory = await wordRepository.ExistsByCategoryAndGroup(groupId, dto.Category);

        if (existsByCategory)
            return Result<long>.Forbidden("Нельзя создавать две группы иссключений для одной группы");
        
        var word = dto.Adapt<ExcludedWord>();
        word.GroupId = groupId;

        var wordId = await wordRepository.CreateAsync(word);

        if (wordId == 0)
            return Result<long>.BadRequest("Не удалось добавить иссключения");
        
        return Result<long>.Created(wordId);
    }

    public async Task<Result<bool>> UpdateAsync(ClaimsPrincipal user, long wordId, UpdateExcludedWordDto dto)
    {
        var account = await userManager.GetUserAsync(user);
        
        if (account == null) 
            return Result<bool>.NotFound("Пользователь не найден");
        
        var word = await wordRepository.GetByIdWithGroupAsync(wordId);
        
        if (word == null)
            return Result<bool>.NotFound("Иссключения не найдены");
        
        var roles = await userManager.GetRolesAsync(account);
        var isAdmin = roles.Contains(Roles.Admin);

        if (word.Group.AccountId != account.Id && !isAdmin)
            return Result<bool>.Forbidden("Недостаточно прав для редактирования иссключений");
        
        word.Category = dto.Category;
        word.Words = dto.Words;
        
        var isUpdated = await wordRepository.UpdateAsync(word);
        
        if (!isUpdated)
            return Result<bool>.BadRequest("Ошибка обновления иссключений");
        
        return Result<bool>.NoContent();
    }

    public async Task<Result<bool>> DeleteAsync(ClaimsPrincipal user, long wordId)
    {
        var account = await userManager.GetUserAsync(user);
        
        if (account == null) 
            return Result<bool>.NotFound("Пользователь не найден");
        
        var word = await wordRepository.GetByIdWithGroupAsync(wordId);
        
        if (word == null)
            return Result<bool>.NotFound("Иссключения не найдены");

        var roles = await userManager.GetRolesAsync(account);
        var isAdmin = roles.Contains(Roles.Admin);
        
        if (word.Group.AccountId != account.Id && !isAdmin)
            return Result<bool>.Forbidden("Недостаточно прав для удаления группы");
        
        var isRemoved = await wordRepository.DeleteAsync(word);

        if (!isRemoved)
            return Result<bool>.BadRequest("Ошибка удаления иссключений");
        
        return Result<bool>.NoContent();
    }
}