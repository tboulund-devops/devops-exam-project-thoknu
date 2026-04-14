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
            {
                task.Status = TaskItem.StatusValues.Todo;
            }
            
            var created = await _service.CreateTask(task);
            return CreatedAtAction(nameof(GetTasks), new { id = created.Id }, created);
        }
        
        [HttpGet("{id}")]
        public async Task<ActionResult<TaskItem>> GetTask(int id)
        {
            var task = await _service.GetTaskById(id);

            if (task == null)
                return NotFound();

            return task;
        }
        
        [HttpGet("status/{status}")]
        public async Task<IEnumerable<TaskItem>> GetByStatus(string status)
        {
            return await _service.GetAllTasksByStatus(status);
        }
        
        [HttpPut("{id}")]
        public async Task<ActionResult<TaskItem>> UpdateTask(int id, TaskItem updated)
        {
            var result = await _service.UpdateTask(id, updated);

            if (result == null)
                return NotFound();

            return Ok(result);
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