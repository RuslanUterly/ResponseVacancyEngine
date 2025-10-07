namespace ResponseVacancyEngine.Application.DTOs;

public class AccountDto
{
    public long Id { get; set; }
    public string? Email { get; set; }
    public string? ClientId { get; set; }
    public string? ClientSecret { get; set; }
}