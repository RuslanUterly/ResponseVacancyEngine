using System.Text.Json.Serialization;

namespace ResponseVacancyEngine.Application.DTOs.Profile;

public class HhJwtCredentialsDto
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; }
    
    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }
    
    [JsonPropertyName("refresh_token")]
    public string RefreshToken { get; set; }
}