using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Triopet.BusinessContext;
using Triopet.BusinessContext.Entities;
using Triopet.Shared.Models;

namespace Triopet.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnimalTypeController : ControllerBase
    {
        private readonly IBusinessContext _businessContext;

        public AnimalTypeController(IBusinessContext context)
        {
            _businessContext = context;
        }

        [HttpGet("/animaltype")]
        public async Task<IActionResult> GetAnimalTypes()
        {
            var animalTypes = await _businessContext.AnimalTypes.ToListAsync();

            if (animalTypes == null || animalTypes.Count == 0)
            {
                return NotFound("Error trying to find animal types");
            }

            var animalTypesList = new List<AnimalTypeDto>();

            foreach (var item in animalTypes)
            {
                var typesDto = new AnimalTypeDto
                {
                    Id = item.Id,
                    AnimalType = item.Type,
                };
                animalTypesList.Add(typesDto);
            }
            return Ok(animalTypesList);
        }
        [HttpGet("/animaltype/{id}")]
        public async Task<IActionResult> GetAnimalTypeById(int id)
        {
            var animalType = await _businessContext.AnimalTypes
                .Where(t => t.Id == id)
                .Select(ty => new AnimalTypeDto
                {
                    Id = ty.Id,
                    AnimalType = ty.Type
                })
                .FirstOrDefaultAsync();

            if(animalType == null)
            {
                return BadRequest("Error trying to find a certain animal type");
            }

            return Ok(animalType);
        }
    }
}
