using Microsoft.AspNetCore.Mvc;
using TaskKing.Api.DTOs;
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
        public async Task<IEnumerable<TaskDto>> GetTasks()
        {
            var tasks = await _service.GetAllTasks();

            return tasks.Select(t => new TaskDto
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                CreatedAt = t.CreatedAt,
                Status = t.Status
            });
        }

        [HttpPost]
        public async Task<ActionResult<TaskDto>> CreateTask(CreateTaskDto dto)
        {
            var task = new TaskItem
            {
                Title = dto.Title,
                Description = dto.Description,
                Status = dto.Status ?? TaskItem.StatusValues.Todo
            };

            var created = await _service.CreateTask(task);

            var result = new TaskDto
            {
                Id = created.Id,
                Title = created.Title,
                Description = created.Description,
                CreatedAt = created.CreatedAt,
                Status = created.Status
            };

            return CreatedAtAction(nameof(GetTask), new { id = created.Id }, result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TaskDto>> GetTask(int id)
        {
            var task = await _service.GetTaskById(id);

            if (task == null)
                return NotFound();

            var result = new TaskDto
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                CreatedAt = task.CreatedAt,
                Status = task.Status
            };

            return Ok(result);
        }

        [HttpGet("status/{status}")]
        public async Task<IEnumerable<TaskDto>> GetByStatus(string status)
        {
            var tasks = await _service.GetAllTasksByStatus(status);

            return tasks.Select(t => new TaskDto
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                CreatedAt = t.CreatedAt,
                Status = t.Status
            });
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<TaskDto>> UpdateTask(int id, UpdateTaskDto dto)
        {
            var updated = new TaskItem
            {
                Title = dto.Title,
                Description = dto.Description,
                Status = dto.Status ?? TaskItem.StatusValues.Todo
            };

            var result = await _service.UpdateTask(id, updated);

            if (result == null)
                return NotFound();

            var response = new TaskDto
            {
                Id = result.Id,
                Title = result.Title,
                Description = result.Description,
                CreatedAt = result.CreatedAt,
                Status = result.Status
            };

            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTask(int id)
        {
            var deleted = await _service.DeleteTask(id);

            if (!deleted)
                return NotFound();

            return NoContent();
        }
    }
}