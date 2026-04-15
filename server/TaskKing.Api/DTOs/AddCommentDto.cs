using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace TaskKing.Api.DTOs;

public class AddCommentDto
{
    [Required]
    public string Content { get; set; } = string.Empty;

    [JsonRequired]
    public int TaskId { get; set; }
}