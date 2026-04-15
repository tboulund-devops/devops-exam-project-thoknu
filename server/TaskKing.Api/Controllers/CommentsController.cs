using Microsoft.AspNetCore.Mvc;
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
        public async Task<IActionResult> Get(int taskId)
        {
            var comments = await _service.GetCommentsForTask(taskId);
            return Ok(comments);
        }

        [HttpPost]
        public async Task<IActionResult> Create(int taskId, Comment comment)
        {
            var created = await _service.AddComment(taskId, comment.Content);
            return Ok(created);
        }
    }
}
