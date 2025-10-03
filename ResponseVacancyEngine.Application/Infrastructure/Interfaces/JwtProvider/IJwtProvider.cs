using ResponseVacancyEngine.Persistence.Models;

namespace ResponseVacancyEngine.Application.Infrastructure.Interfaces.JwtProvider;

public interface IJwtProvider
{
    string GenerateToken(Account account);
}