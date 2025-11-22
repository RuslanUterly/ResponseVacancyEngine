namespace ResponseVacancyEngine.Application.DTOs.ExcludedWord;

public class ExcludedWordBaseDto
{
    public string Category { get; set; }
    public string[] Words { get; set; }
}

public class CreateExcludedWordDto : ExcludedWordBaseDto;

public class UpdateExcludedWordDto : ExcludedWordBaseDto;

public class ExcludedWordDto :  ExcludedWordBaseDto
{
    public long Id { get; set; }
    public long GroupId { get; set; }
}

