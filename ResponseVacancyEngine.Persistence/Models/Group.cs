namespace ResponseVacancyEngine.Persistence.Models;

public class Group 
{
    public long Id { get; set; }
    
    public GroupSettings? Settings { get; set; }
    public ICollection<ExcludedWord>? ExcludedWords { get; set; }
    public ICollection<RespondedVacancy>? RespondedVacancies { get; set; }
}