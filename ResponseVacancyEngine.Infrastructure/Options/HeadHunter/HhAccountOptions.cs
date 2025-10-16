namespace ResponseVacancyEngine.Infrastructure.Options.HeadHunter;

public class HhAccountOptions
{
    public string AccessToken { get; set; } = string.Empty;
    public int ResumeId { get; set; }
    public string? Message { get; set; }
}