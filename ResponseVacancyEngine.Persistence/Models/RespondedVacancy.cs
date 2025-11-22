using ResponseVacancyEngine.Persistence.Models.Enums;

namespace ResponseVacancyEngine.Persistence.Models;

public class RespondedVacancy
{
    public long Id { get; set; }
    public long VacancyId { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Url { get; set; }
    public DateTime? CreatedDate { get; set; }
    public DateTime? ResponseDate { get; set; }
    public Status Status { get; set; }
    
    public long GroupId { get; set; }
    public Group Group { get; set; }
    
    public long AccountId { get; set; }
    public Account Account { get; set; }
}