using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Triopet.BusinessContext;
using Triopet.BusinessContext.Entities;
using Triopet.Shared;
using Triopet.Shared.Models;


namespace Triopet.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EntryController : ControllerBase
    {
        private readonly IBusinessContext _businessContext;

        public EntryController(IBusinessContext businessContext)
        {
            _businessContext = businessContext;
        }
        [HttpGet("/entries")]
        public async Task<IActionResult> GetEntries()
        {
            var entries = await _businessContext.Entries
                .Include(e => e.ProductEntries)
                    .ThenInclude(pe => pe.Product)
                        .ThenInclude(p => p.Category)
                .Include(e => e.ProductEntries)
                    .ThenInclude(pe => pe.Product)
                        .ThenInclude(p => p.AnimalType)
                .Include(e => e.ProductEntries)
                    .ThenInclude(pe => pe.Product)
                        .ThenInclude(p => p.Images)
                .Where(d => !d.IsDeleted)
                .ToListAsync();

            List<EntryDto> entryList = new List<EntryDto>();

            foreach (var entry in entries)
            {
                var entryDto = new EntryDto
                {
                    Id = entry.Id,
                    DateOfEntry = entry.EntryDate,
                    ProductEntries = entry.ProductEntries
                    .Select(pe => new ProductEntryDto
                    {
                        EntryId = pe.EntryId,
                        ProductId = pe.ProductId,
                        Quantity = pe.Quantity,
                        PriceUnitOfEntry = pe.PriceUnit,
                        Product = new ProductDto
                        {
                            Id = pe.Product.Id,
                            Name = pe.Product.Name,
                            Description = "",
                            PricePerUnit = pe.Product.Price,
                            Category = new CategoryDto
                            {
                                Id = pe.Product.Category.Id,
                                Category = pe.Product.Category.CategoryName,
                            },
                            AnimalType = new AnimalTypeDto
                            {
                                Id = pe.Product.AnimalType.Id,
                                AnimalType = pe.Product.AnimalType.Type,
                            },
                            Images = new List<ImageDto>()

                        }
                    }).ToList()
                };

                entryList.Add(entryDto);
            }

            return Ok(entryList);
        }
        //Criar um novo ProdutDto para as listas
        [HttpGet("/entries/{id}")]
        public async Task<EntryDto> GetEntryById(int id)
        {
            var entry = await _businessContext.Entries
                .Include(e => e.ProductEntries)
                    .ThenInclude(pe => pe.Product)
                        .ThenInclude(p => p.Category)
                .Include(e => e.ProductEntries)
                    .ThenInclude(pe => pe.Product)
                        .ThenInclude(p => p.AnimalType)
                .Include(e => e.ProductEntries)
                    .ThenInclude(pe => pe.Product)
                        .ThenInclude(p => p.Images)
                .Where(d => !d.IsDeleted)
                .Select(et => new EntryDto
                {
                    Id = et.Id,
                    DateOfEntry = et.EntryDate,
                    ProductEntries = et.ProductEntries
                    .Select(pe => new ProductEntryDto
                    {
                        EntryId = pe.EntryId,
                        ProductId = pe.ProductId,
                        Quantity = pe.Quantity,
                        PriceUnitOfEntry = pe.PriceUnit,
                        Product = new ProductDto
                        {
                            Id = pe.Product.Id,
                            Name = pe.Product.Name,
                            Description = "",
                            PricePerUnit = pe.Product.Price,
                            Category = new CategoryDto
                            {
                                Id = pe.Product.Category.Id,
                                Category = pe.Product.Category.CategoryName,
                            },
                            AnimalType = new AnimalTypeDto
                            {
                                Id = pe.Product.AnimalType.Id,
                                AnimalType = pe.Product.AnimalType.Type,
                            },
                            Images = new List<ImageDto>()

                        }
                    }).ToList(),
                }).FirstOrDefaultAsync();

            if (entry == null)
            {
                return new EntryDto();

            }
            return entry;
        }

        [HttpPost("/entries")]
        public async Task<IActionResult> AddEntry([FromBody] EntryDto? entry)
        {
            if (entry == null)
            {
                return BadRequest("Invalid entry data.");
            }
            var newEntry = new BusinessContext.Entities.Entry
            {
                EntryDate = entry.DateOfEntry,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _businessContext.Entries.Add(newEntry);

            await _businessContext.SaveChangesAsync(true);

            return CreatedAtAction(nameof(GetEntries), new { id = newEntry.Id }, new EntryDto
            {
                Id = entry.Id,
                DateOfEntry = entry.DateOfEntry

            });
        }
        [HttpPut("/deleteentries/id")]
        public async Task<IActionResult> DeleteEntry(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Error trying to find entry id");
            }

            var foundEntry = await _businessContext.Entries.FindAsync(id);

            if (foundEntry == null)
            {
                return NotFound("Entry not found");
            }

            foundEntry.IsDeleted = true;
            foundEntry.UpdatedAt = DateTime.UtcNow;

            var response = await _businessContext.SaveChangesAsync(true);

            if (response > 0)
            {
                return Ok(response);
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error trying to delete entry");
            }

        }
        [HttpPut("/entries")]
        public async Task<IActionResult> UpdateEntry([FromBody] Entry entry)
        {
            if (entry.Id == 0)
            {
                return BadRequest("Error getting entry Id");

            }

            var existingEntry = await _businessContext.Entries.FindAsync(entry.Id);

            if (existingEntry == null)
            {
                return NotFound("Patient not found");

            }
            existingEntry.Id = entry.Id;
            existingEntry.EntryDate = DateTime.UtcNow;

            var response = await _businessContext.SaveChangesAsync(true);

            if (response > 0)
            {
                return Ok();
            }
            else
            {
                return StatusCode(StatusCodes.Status400BadRequest, "Error trying to edit the entry");
            }
        }
    }
}
