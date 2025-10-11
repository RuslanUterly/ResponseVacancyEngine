using ResponseVacancyEngine.Application.DTOs.Group;
using ResponseVacancyEngine.Persistence.Models;

namespace ResponseVacancyEngine.Application.Mapping;

public static class GroupMapping
{
    public static List<GroupDto> ToDto(this IEnumerable<Group> groups)
    {
        return groups.Select(g => new GroupDto()
        {
            Id = g.Id,
            Settings = g.Settings
        }).ToList();
    }

    public static GroupDto ToDto(this Group group)
    {
        return new GroupDto()
        {
            Id = group.Id,
            Settings = group.Settings,
        };
    }
    
    public static Group ToGroup(this GroupDto dto, long accountId)
    {
        return new Group()
        {
            Settings = dto.Settings,
            AccountId = accountId
        };
    }
}