namespace ResponseVacancyEngine.Application.DTOs.Profile;

public class HhResumeDto
{
    public string Id { get; set; }
    public string Title { get; set; }
}

public class HhResumeListResponse
{
    public List<HhResumeDto> Items { get; set; } = [];
} 