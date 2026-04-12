using Microsoft.AspNetCore.Mvc;
using TaskKing.Api.Models;
using TaskKing.Api.Services;

namespace TaskKing.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TasksController : ControllerBase
    {
        private readonly TaskService _service;

        public TasksController(TaskService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IEnumerable<TaskItem>> GetTasks()
        {
            return await _service.GetAllTasks();
        }

        [HttpPost]
        public async Task<ActionResult<TaskItem>> CreateTask(TaskItem task)
        {
            if (string.IsNullOrWhiteSpace(task.Title))
                return BadRequest("Title is required.");

            var created = await _service.CreateTask(task);
            return CreatedAtAction(nameof(GetTasks), new { id = created.Id }, created);
        }
    }
}