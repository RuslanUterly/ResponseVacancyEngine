using ResponseVacancyEngine.Persistence.Models.Enums;

namespace ResponseVacancyEngine.Application.DTOs.RespondedVacancy;

public class VacancyBaseDto
{
    public long VacancyId { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? Url { get; set; }
    public DateTime? CreatedDate { get; set; }
    public Status Status { get; set; }
}

public class CreateVacancyDto : VacancyBaseDto;
public class UpdateVacancyDto : VacancyBaseDto;

public class VacancyDto : VacancyBaseDto
{
    public long Id { get; set; }
}