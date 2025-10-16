using System.Security.Claims;
using ResponseVacancyEngine.Application.DTOs.ExcludedWord;
using ResponseVacancyEngine.Application.Helpers.ResultPattern;

namespace ResponseVacancyEngine.Application.Interfaces;

public interface IExcludedWordService
{
    Task<List<ExcludedWordDto>> GetExcludedWordsByGroupAsync(long groupId);
    Task<ExcludedWordDto> GetByIdAsync(long wordId);
    Task<Result<long>> CreateAsync(ClaimsPrincipal user, long groupId, CreateExcludedWordDto dto);
    Task<Result<bool>> UpdateAsync(ClaimsPrincipal user, long wordId, UpdateExcludedWordDto dto);
    Task<Result<bool>> DeleteAsync(ClaimsPrincipal user, long wordId);
}