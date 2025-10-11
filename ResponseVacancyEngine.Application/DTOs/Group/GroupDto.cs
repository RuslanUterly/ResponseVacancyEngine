using ResponseVacancyEngine.Persistence.Models;

namespace ResponseVacancyEngine.Application.DTOs.Group;

public class GroupDto
{
    public long Id { get; set; }
    public GroupSettings? Settings { get; set; }
}