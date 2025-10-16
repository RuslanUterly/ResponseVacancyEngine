using Mapster;
using ResponseVacancyEngine.Application.DTOs.ExcludedWord;
using ResponseVacancyEngine.Application.DTOs.Group;
using ResponseVacancyEngine.Persistence.Models;

namespace ResponseVacancyEngine.Application.Mapping;

public static class MappingConfig
{
    public static void RegisterMappings()
    {
        TypeAdapterConfig<UpdateGroupDto, Group>
            .NewConfig()
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.AccountId);
        
        TypeAdapterConfig<UpdateExcludedWordDto, ExcludedWord>
            .NewConfig()
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.GroupId);
        
        TypeAdapterConfig.GlobalSettings.Compile();
    }
}