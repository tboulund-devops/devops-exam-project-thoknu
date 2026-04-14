using System.ComponentModel.DataAnnotations;

namespace TaskKing.Api.Models;

public class TaskItem
{
    public static class StatusValues
    {
        public const string Todo = "Todo";
        public const string InProgress = "InProgress";
        public const string Done = "Done";
    }
    
    public static class PriorityValues
    {
        public const string Low = "Low";
        public const string Medium = "Medium";
        public const string High = "High";
    }
    
    public int Id { get; set; }
    
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public string Status { get; set; } = StatusValues.Todo;
    
    public string Priority { get; set; } = PriorityValues.Medium;
}