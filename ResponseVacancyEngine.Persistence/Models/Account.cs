using Microsoft.AspNetCore.Identity;

namespace ResponseVacancyEngine.Persistence.Models;

public class Account : IdentityUser<long>
{
    public override long Id { get; set; }
    public string? ClientId { get; set; }
    public string? ClientSecret { get; set; }
    public string? AccessToken { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? AccessTokenExpiresAt { get; set; }
    public bool IsActiveResponse { get; set; }
    
    public ICollection<Group>? Groups { get; set; }
    public ICollection<RespondedVacancy>? RespondedVacancies { get; set; }
}