using ResponseVacancyEngine.Application.DTOs.HttpResponse;
using ResponseVacancyEngine.Application.DTOs.Profile;
using ResponseVacancyEngine.Persistence.Models;

namespace ResponseVacancyEngine.Application.Interfaces.Services.HeadHunterApi;

public interface IHhProfileClient
{
    Task<HttpResponse<List<HhResumeDto>>> GetResumesAsync(Account account);
}