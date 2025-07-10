using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Triopet.BusinessContext;
using Triopet.Shared.Models;

namespace Triopet.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly IBusinessContext _businessContext;

        public CategoryController(IBusinessContext context)
        {
            _businessContext = context;
        }

        [HttpGet("/categories")]
        public async Task<IActionResult> GetAllCategories()
        {
            var categories = await _businessContext.Categories.ToListAsync();

            if (categories == null || categories.Count == 0)
            {
                return NotFound("No categories found");
            }

            var result = categories.Select(c => new CategoryDto
            {
                Id = c.Id,
                Category = c.CategoryName
            }).ToList();

            return Ok(result);
        }

        [HttpGet("/categories/{id}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid id");
            }

            var category = await _businessContext.Categories.FindAsync(id);

            if (category == null)
            {
                return NotFound("Category not found");
            }

            var dto = new CategoryDto
            {
                Id = category.Id,
                Category = category.CategoryName
            };

            return Ok(dto);
        }
    }
}
