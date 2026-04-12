using System.ComponentModel.DataAnnotations;

namespace TaskKing.Api.Models;

public class TaskItem
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public string Status { get; set; } = "Todo"; // Todo, InProgress, Done
}