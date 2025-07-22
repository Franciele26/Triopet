using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Extensions;
using Triopet.BusinessContext;
using Triopet.BusinessContext.Entities;
using Triopet.Shared.Models;
using Triopet.Shared.Models.DashBoardDtos;

namespace Triopet.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IBusinessContext _businessContext;

        public DashboardController(IBusinessContext Context)
        {
            _businessContext = Context;
        }

        [HttpGet("/lowstock")]
        public async Task<IActionResult> GetLowStockProducts()
        {
            var calledLowStockItems = await _businessContext.Products
                .Include(c => c.Category)
                .Include(t => t.AnimalType)
                .Where(p => !p.IsDeleted && p.Quantity <= 5)
                .ToListAsync();

            List<LowStockProductsDto> lowStockProductsDtos = new List<LowStockProductsDto>();

            foreach (var item in calledLowStockItems)
            {
                var lowStockItems = new LowStockProductsDto
                {
                    Id = item.Id,
                    Name = item.Name,
                    Category = new CategoryDto
                    {
                        Id = item.Category.Id,
                        Category = item.Category.CategoryName,
                    },
                    AnimalType = new AnimalTypeDto
                    {
                        Id = item.AnimalType.Id,
                        AnimalType = item.AnimalType.Type,
                    },
                    QUantity = item.Quantity,
                };
                lowStockProductsDtos.Add(lowStockItems);
            }
            return Ok(lowStockProductsDtos);
        }

        [HttpGet("/topfivemovements")]
        public async Task<IActionResult> TopFiveMovements()
        {
            var top5Movements = await _businessContext.Products
                .Include(c => c.Category)
                .Include(t => t.AnimalType)
                .Select(p => new
                {
                    Product = p,
                    EntryCount = _businessContext.ProductEntries.Count(pe => pe.ProductId == p.Id),
                    ExitCount = _businessContext.ProductExits.Count(pe => pe.ProductId == p.Id),
                })
                .Select(x => new
                {
                    x.Product,
                    TotalMovements = x.EntryCount + x.ExitCount
                })
                .OrderByDescending(x => x.TotalMovements)
                .Take(5)
                .ToListAsync();


            List<TopFiveMovementDto> topFiveList = new();

            foreach (var item in top5Movements)
            {
                var topMovementsDto = new TopFiveMovementDto
                {
                    Id = item.Product.Id,
                    Name = item.Product.Name,
                    Category = new CategoryDto
                    {
                        Id = item.Product.Category.Id,
                        Category = item.Product.Category.CategoryName,
                    },
                    AnimalType = new AnimalTypeDto
                    {
                        Id = item.Product.AnimalType.Id,
                        AnimalType = item.Product.AnimalType.Type
                    },
                    TotalMovements = item.TotalMovements,
                };
                topFiveList.Add(topMovementsDto);
            }
            return Ok(topFiveList);
        }

        [HttpGet("/topSoldItems/{categoryId}")]
        public async Task<IActionResult> TopSoldItems(int categoryId)
        {
            try
            {
                //Fazer para receber o categoryId apartir do frontend
                //Venda - motif - sales
                var productsByCategory = await _businessContext.Products
                .Include(pe => pe.ProductExits)
                    .ThenInclude(e => e.Exit)
                        .ThenInclude(m => m.Motif)
                .Where(p => !p.IsDeleted && p.CategoryId == categoryId)
                .ToListAsync();

                var topSalesList = new List<TopProductsSoldPerCategory>();

                foreach (var item in productsByCategory)
                {
                    //ir buscar os produtos do fetch anterior, os produtos que for de reason Vedas e sumar a quantidade nas saidas
                    //para demonstrar numero produtos vendidos
                    var totalSold = item.ProductExits
                        .Where(pe => pe.Exit.Motif.Reason == "Venda" || pe.Exit.Motif.Reason == "Sales")
                        .Sum(pe => pe.Quantity);

                    //se houver vendas, converter para dto
                    if (totalSold > 0)
                    {
                        topSalesList.Add(new TopProductsSoldPerCategory
                        {
                            Id = item.Id,
                            Name = item.Name,
                            SoldQuantity = totalSold
                        });
                    }
                }
                //ir buscar SO o top 3 dessa lista
                var topThree = topSalesList.OrderByDescending(s => s.SoldQuantity)
                    .Take(3).ToList();


                return Ok(topThree);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error trying to get the top 3 for category {categoryId}: {ex.Message}");
            }
        }


    }
}
