namespace TaskKing.Api.DTOs;

public class CommentDto
{
    public int Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public int TaskId { get; set; }
    public DateTime CreatedAt { get; set; }
}