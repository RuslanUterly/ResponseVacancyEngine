namespace ResponseVacancyEngine.Infrastructure.Options;

public class HeadHunterAccountOptions
{
    public string AccessToken { get; set; } = string.Empty;
    public int ResumeId { get; set; }
    public string? Message { get; set; }
}