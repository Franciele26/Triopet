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

            var categoryList = new List<CategoryDto>();

            //var result = categories.Select(c => new CategoryDto
            //{
            //    Id = c.Id,
            //    Category = c.CategoryName
            //}).ToList();

            //ou assim
            foreach (var item in categories)
            {
                var categoryDto = new CategoryDto
                {
                    Id = item.Id,
                    Category = item.CategoryName,
                };
                categoryList.Add(categoryDto);
            }

            return Ok(categoryList);
        }

        [HttpGet("/categories/{id}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {

            var category = await _businessContext.Categories
                .Where(c => c.Id == id)
                .Select(cat => new CategoryDto
                {
                    Id = cat.Id,
                    Category = cat.CategoryName,
                }).FirstOrDefaultAsync();

            if (category == null)
            {
                return BadRequest("Error trying to find a certain category");
            }

            return Ok(category);
        }
    }
}
