using Microsoft.AspNetCore.Mvc;
using TaskKing.Api.DTOs;
using TaskKing.Api.Models;
using TaskKing.Api.Services;

namespace TaskKing.Api.Controllers
{
    [ApiController]
    [Route("api/tasks/{taskId}/comments")]
    public class CommentsController : ControllerBase
    {
        private readonly CommentService _service;

        public CommentsController(CommentService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<List<CommentDto>>> Get(int taskId)
        {
            var comments = await _service.GetCommentsForTask(taskId);

            var result = comments.Select(c => new CommentDto
            {
                Id = c.Id,
                Content = c.Content,
                TaskId = c.TaskItemId,
                CreatedAt = c.CreatedAt
            }).ToList();

            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<CommentDto>> Add(AddCommentDto comment)
        {
            var added = await _service.AddComment(comment.TaskId, comment.Content);

            var result = new CommentDto
            {
                Id = added.Id,
                Content = added.Content,
                TaskId = added.TaskItemId,
                CreatedAt = added.CreatedAt
            };

            return Ok(result);
        }
    }
}
