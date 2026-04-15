using System.Text.Json.Serialization;
namespace TaskKing.Api.Models;

public class Comment
{
    [JsonIgnore]
    public int Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public int TaskItemId { get; set; }
    public TaskItem TaskItem { get; set; } = null!;
}