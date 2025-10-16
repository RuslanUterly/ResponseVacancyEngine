using ResponseVacancyEngine.Persistence.Models;

namespace ResponseVacancyEngine.Application.Interfaces.JwtProvider;

public interface IJwtProvider
{
    string GenerateToken(Account account);
}