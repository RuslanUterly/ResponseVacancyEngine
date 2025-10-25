using Quartz;
using ResponseVacancyEngine.Application.Services;

namespace ResponseVacancyEngine.Jobs;

public class ResponseVacancyJob(ResponseVacancyService vacancyService) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        await vacancyService.GetVacancies();
    }
}