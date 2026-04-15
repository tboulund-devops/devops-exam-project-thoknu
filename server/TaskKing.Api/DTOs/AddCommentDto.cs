using System.ComponentModel.DataAnnotations;

namespace TaskKing.Api.DTOs;

public class AddCommentDto
{
    [Required]
    public string Content { get; set; } = string.Empty;

    [Required]
    public int TaskId { get; set; }
}