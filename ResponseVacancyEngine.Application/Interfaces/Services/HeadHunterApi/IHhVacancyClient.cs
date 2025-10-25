using ResponseVacancyEngine.Application.DTOs.HttpResponse;
using ResponseVacancyEngine.Application.DTOs.Profile;
using ResponseVacancyEngine.Persistence.Models;

namespace ResponseVacancyEngine.Application.Interfaces.Services.HeadHunterApi;

public interface IHhVacancyClient
{
    Task<HttpResponse<HhVacanciesResponse>> GetVacanciesAsync(Account account, Group group, int page = 0);
    Task<HttpResponse<bool>> RespondAsync(string vacancyId, Account account, Group group);
}