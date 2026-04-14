using System.Text.Json.Serialization;

namespace TaskKing.Api.Models;

public class Category
{
    [JsonIgnore]
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public List<TaskItem> Tasks { get; set; } = new();
}