namespace ResponseVacancyEngine.Application.DTOs;

public class HeadHunterJwtCredentialsDto
{
    public string AccessToken { get; set; }
    public int ExpiresIn { get; set; }
    public string RefreshToken { get; set; }
}