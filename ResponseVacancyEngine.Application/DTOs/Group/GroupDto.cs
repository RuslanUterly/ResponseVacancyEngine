using ResponseVacancyEngine.Persistence.Models;

namespace ResponseVacancyEngine.Application.DTOs.Group;

public class GroupBaseDto
{
    public string ResumeId { get; set; }
    public string Message { get; set; }
    public GroupSettings? Settings { get; set; }
}

public class CreateGroupDto : GroupBaseDto;
public class UpdateGroupDto : GroupBaseDto;

public class GroupDto : GroupBaseDto
{
    public long Id { get; set; }
}
