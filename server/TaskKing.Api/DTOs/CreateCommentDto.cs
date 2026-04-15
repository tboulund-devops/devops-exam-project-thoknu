namespace TaskKing.Api.DTOs;

public class CreateCommentDto
{
    public string Text { get; set; } = string.Empty;
    public int TaskId { get; set; }
}