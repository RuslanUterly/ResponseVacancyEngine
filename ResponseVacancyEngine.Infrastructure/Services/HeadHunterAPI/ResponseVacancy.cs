using System.Net.Http.Headers;
using System.Net.Http.Json;
using ResponseVacancyEngine.Infrastructure.Options;

namespace ResponseVacancyEngine.Infrastructure.Services.HeadHunterAPI;

public class ResponseVacancy(HeadHunterAccountOptions options)
{
    public async Task Respond(int vacancyId)
    {
        using var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", options.AccessToken);

        var response = await client.PostAsJsonAsync("https://api.hh.ru/negotiations", new {
            vacancy_id = vacancyId,
            resume_id = options.ResumeId,
            message = options.Message
        });

        var result = await response.Content.ReadAsStringAsync();
        Console.WriteLine(result);
    }
}