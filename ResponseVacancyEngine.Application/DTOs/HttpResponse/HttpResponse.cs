namespace ResponseVacancyEngine.Application.DTOs.HttpResponse;

public class HttpResponse<T>
{
    public T? Data { get; set; }
    public bool IsSuccessStatusCode { get; set; } =  true;
    public string Message { get; set; } = string.Empty;
}