namespace ResponseVacancyEngine.Infrastructure.Options;

public class JwtOptions
{
    public string SecretKey { get; set; } = "SecretKey121212";
    public int ExpiresHours  { get; set; } = 24;
}