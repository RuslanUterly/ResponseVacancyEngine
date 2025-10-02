using Microsoft.AspNetCore.Identity;

namespace ResponseVacancyEngine.Persistence.Models;

public class Account : IdentityUser<long>
{
    public override long Id { get; set; }
    public string? ClientIdHash { get; set; }
    public string? ClientSecretHash { get; set; }
    public string? AccessTokenHash { get; set; }
    public string? RefreshTokenHash { get; set; }
    public DateTime? AccessTokenExpiresAt { get; set; }
    public bool IsActiveResponse { get; set; }
    
    public ICollection<Group>? Groups { get; set; }
    public ICollection<RespondedVacancy>? RespondedVacancies { get; set; }
}