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
                            Quantity = pe.Product.Quantity,
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
                .Where(d => !d.IsDeleted && d.Id == id)
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
                            Quantity = pe.Product.Quantity,
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
        public async Task<IActionResult> AddNewEntry([FromBody] EntryDto entryDto)
        {

            var today = DateTime.Now;
            var minDate = today.AddDays(-14);
            var maxDate = today.AddDays(7);

            if (entryDto == null)
            {
                return BadRequest("Error trying to create the entryDto");
            }

            if (entryDto.DateOfEntry < minDate || entryDto.DateOfEntry > maxDate)
            {
                return BadRequest($"Invalid date: the selected date must be in between min: {minDate} and max {maxDate}");
            }

            var newEntry = new Entry
            {
                EntryDate = entryDto.DateOfEntry,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsDeleted = false,
                ProductEntries = new List<ProductEntry>()
            };

            foreach (var pe in entryDto.ProductEntries)
            {
                var product = await _businessContext.Products.FindAsync(pe.ProductId);
                if (product == null)
                    return NotFound($"Produt ID: {pe.ProductId} not found.");

                if (pe.Quantity < 0)
                {
                    return BadRequest($"Impossible to add a product with less then 0 quantity, '{product.Name}'.");
                }
                product.Quantity += pe.Quantity;
                newEntry.ProductEntries.Add(new ProductEntry
                {
                    ProductId = pe.ProductId,
                    Quantity = pe.Quantity,
                    PriceUnit= pe.PriceUnitOfEntry
                });
            }
            _businessContext.Entries.Add(newEntry);

            var response = await _businessContext.SaveChangesAsync(true);

            if (response > 0)
            {
                return Ok("Produt added with success");
            }
            else
            {
                return StatusCode(StatusCodes.Status400BadRequest, "Error trying to edit the entry log");
            }
        }
        [HttpDelete("/entries/{id}")]
        public async Task<IActionResult> DeleteEntry(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Error trying to find the entry log");
            }

            var foundEntry = await _businessContext.Entries
                .Include(e => e.ProductEntries.Where(pe => !pe.IsDeleted))
                .FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted);

            if (foundEntry == null)
            {
                return NotFound("Error trying to find entryId");
            }

            //devolver a quantidade dos items
            foreach (var item in foundEntry.ProductEntries)
            {
                var product = await _businessContext.Products
                    .FindAsync(item.ProductId);

                if (product != null)
                {
                    product.Quantity -= item.Quantity;
                }
                item.IsDeleted = true;
            }

            foundEntry.IsDeleted = true;
            foundEntry.UpdatedAt = DateTime.UtcNow;

            var response = await _businessContext.SaveChangesAsync(true);
            if (response > 0)
            {
                return Ok($"Products deleted with success.\nResponse:{response}");
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error trying to delete entry");
            }
        }

        [HttpPut("/entries")]
        public async Task<IActionResult> UpdateEntries([FromBody] EntryDto entryDto)
        {
            if (entryDto.Id <= 0)
            {
                return BadRequest("Error trying to find that entry log");
            }

            var existingEntryLog = await _businessContext.Entries
                .Include(e => e.ProductEntries)
                .FirstOrDefaultAsync(e => e.Id == entryDto.Id);

            var oldDate = existingEntryLog.EntryDate;

            var today = DateTime.Now;
            var minDate = today.AddDays(-14);
            var maxDate = today.AddDays(7);

            if (entryDto.DateOfEntry != oldDate)
            {
                if (entryDto.DateOfEntry < minDate || entryDto.DateOfEntry > maxDate)
                {
                    return BadRequest($"Invalid date: the selected date must be in between min: {minDate} and max {maxDate}");
                }
            }

            if (existingEntryLog == null)
            {
                return NotFound("Entry log not found");
            }

            //remover de volta o valor da quantidade
            foreach (var item in existingEntryLog.ProductEntries)
            {
                var prod = await _businessContext.Products.FindAsync(item.ProductId);
                if (prod != null)
                {
                    prod.Quantity -= item.Quantity;
                }
            }

            //atualizar os dados
            existingEntryLog.EntryDate = entryDto.DateOfEntry;
            existingEntryLog.UpdatedAt = DateTime.UtcNow;

            //apagar a lista atual
            existingEntryLog.ProductEntries.Clear();

            //adicionar os novos ProductExits e subtrair stock dos produtos
            foreach (var pe in entryDto.ProductEntries)
            {
                var prod = await _businessContext.Products.FindAsync(pe.ProductId);
                if (prod == null)
                    return NotFound($"Product {pe.ProductId} not found");

                if (pe.Quantity < 0)
                    return BadRequest($"Impossible to add negative numbers to stock for product '{prod.Name}'.");

                if (pe.PriceUnitOfEntry < 0)
                    return BadRequest($"Impossible to add a product with negative value '{prod.Name}'.");

                prod.Quantity += pe.Quantity;

                existingEntryLog.ProductEntries.Add(new ProductEntry
                {
                    ProductId = pe.ProductId,
                    Quantity = pe.Quantity,
                    PriceUnit=pe.PriceUnitOfEntry
                });
            }


            var response = await _businessContext.SaveChangesAsync(true);

            if (response > 0)
            {
                return Ok("Entry was edited with success");
            }
            else
            {
                return StatusCode(StatusCodes.Status400BadRequest, "Error trying to edit the entry log");
            }
        }
    }
}
