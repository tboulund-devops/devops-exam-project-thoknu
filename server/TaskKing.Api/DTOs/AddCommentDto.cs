namespace TaskKing.Api.DTOs;

public class AddCommentDto
{
    public string Content { get; set; } = string.Empty;
    public int TaskId { get; set; }
}