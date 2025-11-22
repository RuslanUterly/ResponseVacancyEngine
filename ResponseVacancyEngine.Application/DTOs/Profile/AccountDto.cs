namespace ResponseVacancyEngine.Application.DTOs.Profile;

public class AccountDto
{
    public long Id { get; set; }
    public string? Email { get; set; }
    public bool IsActiveResponse { get; set; }
}