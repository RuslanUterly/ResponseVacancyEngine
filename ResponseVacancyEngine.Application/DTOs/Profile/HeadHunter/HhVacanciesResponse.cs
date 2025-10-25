using System.Text.Json.Serialization;
using ResponseVacancyEngine.Application.Helpers;

namespace ResponseVacancyEngine.Application.DTOs.Profile;

public class Area
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Url { get; set; }
}

public class Experience
{
    public string Id { get; set; }
    public string Name { get; set; }
}

public class Item
{
    public string Id { get; set; }
    public string Name { get; set; }
    public object Department { get; set; }
    [JsonPropertyName("has_test")]
    public bool HasTest { get; set; }
    public Area Area { get; set; }
    public Salary? Salary { get; set; }
    public Type Type { get; set; }
    [JsonPropertyName("published_at")]
    [JsonConverter(typeof(HhDateTimeConverter))]
    public DateTime PublishedAt { get; set; }
    [JsonPropertyName("created_at")]
    [JsonConverter(typeof(HhDateTimeConverter))]
    public DateTime CreatedAt { get; set; }
    public bool Archived { get; set; }
    public string Url { get; set; }
    [JsonPropertyName("alternate_url")]
    public string AlternateUrl { get; set; }
    public Snippet Snippet { get; set; }
    public Schedule Schedule { get; set; }
    [JsonPropertyName("accept_incomplete_resumes")]
    public bool AcceptIncompleteResumes { get; set; }
    public Experience Experience { get; set; }
}

public class HhVacanciesResponse
{
    public List<Item> Items { get; set; }
    public int Found { get; set; }
    public int Pages { get; set; }
    public int Page { get; set; }
}

public class Salary
{
    public int? From { get; set; }
    public int? To { get; set; }
    public string? Currency { get; set; }
    public bool? Gross { get; set; }
}

public class Schedule
{
    public string Id { get; set; }
    public string Name { get; set; }
}

public class Snippet
{
    public string Requirement { get; set; }
    public string Responsibility { get; set; }
}

public class Type
{
    public string Id { get; set; }
    public string Name { get; set; }
}

