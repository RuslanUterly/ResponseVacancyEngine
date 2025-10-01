namespace ResponseVacancyEngine.Persistence.Models;

public class ExcludedWord
{
    public long Id { get; set; }
    public long GroupId { get; set; }
    public string Category { get; set; }
    public string[] Words { get; set; }

    public Group Group { get; set; }
}