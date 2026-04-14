using Microsoft.AspNetCore.Mvc;
using TaskKing.Api.Models;
using TaskKing.Api.Services;

namespace TaskKing.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly CategoryService _service;

        public CategoriesController(CategoryService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> GetAll()
        {
            var categories = await _service.GetAll();
            return Ok(categories);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Category>> GetById(int id)
        {
            var category = await _service.GetById(id);

            if (category == null)
                return NotFound();

            return Ok(category);
        }

        [HttpPost]
        public async Task<ActionResult<Category>> Create(Category category)
        {
            if (string.IsNullOrWhiteSpace(category.Name))
                return BadRequest("Name is required");

            var created = await _service.Create(category);

            return CreatedAtAction(
                nameof(GetById),
                new { id = created.Id },
                created
            );
        }
    }
}