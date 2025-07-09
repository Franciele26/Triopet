using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using Triopet.BusinessContext;
using Triopet.BusinessContext.Entities;
using Triopet.Shared.Models;

namespace Triopet.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExitController : ControllerBase
    {
        private readonly IBusinessContext _businessContext;

        public ExitController(IBusinessContext businessContext)
        {
            _businessContext = businessContext;
        }

        [HttpGet("/exits")]
        public async Task<IActionResult> GetExits()
        {
            var exits = await _businessContext.Exits
                .Include(e => e.ProductExits)
                    .ThenInclude(pe => pe.Product)
                        .ThenInclude(c => c.Category)
                .Include(e => e.ProductExits)
                    .ThenInclude(pe => pe.Product)
                        .ThenInclude(t => t.AnimalType)
                .Include(e => e.ProductExits)//remover depois isto, em baixo passo uma list image nova = ta vazio, como nao é preciso
                    .ThenInclude(ep => ep.Product)
                        .ThenInclude(i => i.Images)
                .Include(m => m.Motif)
                .Where(d => !d.IsDeleted)
                .ToListAsync();

            List<ExitDto> existList = new List<ExitDto>();

            foreach (var exit in exits)
            {
                var exitDto = new ExitDto
                {
                    Id = exit.Id,
                    DateOfExit = exit.ExitDate,
                    ReasonId = exit.MotifId,
                    Reason = new ReasonDto
                    {
                        Id = exit.Motif.Id,
                        Reason = exit.Motif.Reason,
                    },
                    ProductExitDtos = exit.ProductExits
                    .Select(pe => new ProductExitDto
                    {
                        ExitId = pe.ExitId,
                        ProductId = pe.ProductId,
                        Quantity = pe.Quantity,
                        Product = new ProductDto
                        {
                            Id = pe.Product.Id,
                            Name = pe.Product.Name,
                            Description = pe.Product.Description,
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
                existList.Add(exitDto);
            }
            return Ok(existList);
        }

        [HttpGet("/exits/{id}")]
        public async Task<ExitDto> GetExitById(int id)
        {
            var exit = await _businessContext.Exits
                .Include(e => e.ProductExits)
                    .ThenInclude(pe => pe.Product)
                        .ThenInclude(c => c.Category)
                .Include(e => e.ProductExits)
                    .ThenInclude(pe => pe.Product)
                        .ThenInclude(t => t.AnimalType)
                .Include(e => e.ProductExits)//remover depois isto, em baixo passo uma list image nova = ta vazio, como nao é preciso
                    .ThenInclude(ep => ep.Product)
                        .ThenInclude(i => i.Images)
                .Include(m => m.Motif)
                .Where(d => !d.IsDeleted)
                .Select(ze => new ExitDto
                {
                    Id = ze.Id,
                    DateOfExit = ze.ExitDate,
                    ReasonId = ze.MotifId,
                    Reason = new ReasonDto
                    {
                        Id = ze.Motif.Id,
                        Reason = ze.Motif.Reason,
                    },
                    ProductExitDtos = ze.ProductExits
                    .Select(pe => new ProductExitDto
                    {
                        ExitId = pe.ExitId,
                        ProductId = pe.ProductId,
                        Quantity = pe.Quantity,
                        Product = new ProductDto
                        {
                            Id = pe.Product.Id,
                            Name = pe.Product.Name,
                            Description = pe.Product.Description,
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

            if (exit == null)
            {
                return new ExitDto();
            }

            return exit;
        }

        [HttpPut("/exits/{id}")]
        public async Task<IActionResult> DeleteExit(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Error trying to find the exist log");
            }

            var foundExit = await _businessContext.Exits
                .Include(e => e.ProductExits)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (foundExit == null)
            {
                return NotFound("Error trying to find exitId");
            }

            //devolver a quantidade dos items
            foreach (var item in foundExit.ProductExits)
            {
                var product = await _businessContext.Products
                    .FindAsync(item.ProductId);

                if (product != null)
                {
                    product.Quantity += item.Quantity;
                }
            }
            /*
             Se quisese remover os registos mesmo
                // Remover os registos da tabela mista
                _businessContext.ProductExits.RemoveRange(exit.ProductExits);

                // Remover o Exit
                _businessContext.Exits.Remove(exit);
             */


            foundExit.IsDeleted = true;
            foundExit.UpdatedAt = DateTime.UtcNow;

            var response = await _businessContext.SaveChangesAsync(true);
            if (response > 0)
            {
                return Ok(response);
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Error trying to delete exit");
            }
        }

        [HttpPost("/exits")]
        public async Task<IActionResult> AddNewExitAction([FromBody] ExitDto exitDto)
        {
            if (exitDto == null)
            {
                return BadRequest("Error trying to create the exitDto");
            }

            var newExit = new Exit
            {
                ExitDate = DateTime.UtcNow,
                MotifId = exitDto.ReasonId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsDeleted = false,
                ProductExits = new List<ProductExit>()
            };

            foreach (var pe in exitDto.ProductExitDtos)
            {
                var product = await _businessContext.Products.FindAsync(pe.ProductId);
                if (product == null)
                    return NotFound($"Produto com ID {pe.ProductId} não encontrado.");

                if (product.Quantity < pe.Quantity)
                    return BadRequest($"Stock insuficiente para '{product.Name}'.");

                product.Quantity -= pe.Quantity;
                newExit.ProductExits.Add(new ProductExit
                {
                    ProductId = pe.ProductId,
                    Quantity = pe.Quantity
                });
            }
            _businessContext.Exits.Add(newExit);

            var response = await _businessContext.SaveChangesAsync(true);

            if (response > 0)
            {
                return Ok();
            }
            else
            {
                return StatusCode(StatusCodes.Status400BadRequest, "Error trying to edit the exit log");
            }
        }

        [HttpPut("/exits")]
        public async Task<IActionResult> UpdateExits([FromBody] ExitDto exitDto)
        {
            if (exitDto.Id <= 0)
            {
                return BadRequest("Error trying to find that exit log");
            }

            var existingExitLog = await _businessContext.Exits
                .Include(e => e.ProductExits)
                .FirstOrDefaultAsync(e => e.Id == exitDto.Id);

            if (existingExitLog == null)
            {
                return NotFound("Exit log not found");
            }

            //meter de volta o valor da quantidade
            foreach (var item in existingExitLog.ProductExits)
            {
                var prod = await _businessContext.Products.FindAsync(item.ProductId);
                if (prod != null)
                {
                    prod.Quantity += item.Quantity;
                }
            }

            //atualizar os dados
            existingExitLog.ExitDate = exitDto.DateOfExit;
            existingExitLog.MotifId = exitDto.ReasonId;
            existingExitLog.UpdatedAt = DateTime.UtcNow;

            //apagar a lista atual
            existingExitLog.ProductExits.Clear();

            //a dicionar os novos ProductExits e subtrair stock dos produtos
            foreach (var pe in exitDto.ProductExitDtos)
            {
                var prod = await _businessContext.Products.FindAsync(pe.ProductId);
                if (prod == null)
                    return NotFound($"Product {pe.ProductId} not found");

                if (prod.Quantity < pe.Quantity)
                    return BadRequest($"Insufficient stock for product '{prod.Name}'.");

                prod.Quantity -= pe.Quantity;

                existingExitLog.ProductExits.Add(new ProductExit
                {
                    ProductId = pe.ProductId,
                    Quantity = pe.Quantity
                });
            }


            var response = await _businessContext.SaveChangesAsync(true);

            if (response > 0)
            {
                return Ok();
            }
            else
            {
                return StatusCode(StatusCodes.Status400BadRequest, "Error trying to edit the exit log");
            }
        }
    }
}
