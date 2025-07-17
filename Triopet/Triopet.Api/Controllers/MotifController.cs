using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Triopet.BusinessContext;
using Triopet.Shared.Models;

namespace Triopet.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MotifController : ControllerBase
    {
        private readonly IBusinessContext _businessContext;

        public MotifController(IBusinessContext context)
        {
            _businessContext = context;
        }

        [HttpGet("/motives")]
        public async Task<IActionResult> GetAllMotifs()
        {
            var motives = await _businessContext.Motifs.ToListAsync();

            if (motives == null || motives.Count == 0)
            {
                return NotFound("No motives found");
            }

            var motifList = new List<CategoryDto>();

            //var result = categories.Select(c => new CategoryDto
            //{
            //    Id = c.Id,
            //    Category = c.CategoryName
            //}).ToList();

            //ou assim
            foreach (var item in motives)
            {
                var categoryDto = new CategoryDto
                {
                    Id = item.Id,
                    Category = item.Reason,
                };
                motifList.Add(categoryDto);
            }

            return Ok(motifList);
        }

        [HttpGet("/motives/{id}")]
        public async Task<IActionResult> GetMotifById(int id)
        {

            var motif = await _businessContext.Motifs
                .Where(c => c.Id == id)
                .Select(mo => new ReasonDto
                {
                    Id = mo.Id,
                    Reason = mo.Reason,
                }).FirstOrDefaultAsync();

            if (motif == null)
            {
                return BadRequest("Error trying to find a certain category");
            }

            return Ok(motif);
        }
    }
}
