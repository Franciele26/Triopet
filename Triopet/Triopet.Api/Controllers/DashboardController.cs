using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Extensions;
using System.Runtime.Intrinsics.Arm;
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

                var topSalesList = new List<TopProductsSoldPerCategoryDto>();

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
                        topSalesList.Add(new TopProductsSoldPerCategoryDto
                        {
                            Id = item.Id,
                            Name = item.Name,
                            SoldQuantity = totalSold,
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

        //usar CategoryPrices para quando for so preciso os campos das categorias + preço

        [HttpGet("/averagePrice/")]
        public async Task<IActionResult> AveragePricePerCategory()
        {
            //produtos -> categoria para name -> select para criar o objt como dto
            var productsByCategory = await _businessContext.Products
                .Include(c => c.Category)
                .Where(dc => !dc.IsDeleted)
                .GroupBy(ci => new { ci.CategoryId, ci.Category.CategoryName })
                .Select(av => new CategoryPricesDto
                {
                    CategoryId = av.Key.CategoryId,
                    CategoryName = av.Key.CategoryName,
                    Price = av.Average(p => p.Price)
                })
                .ToListAsync();

            return Ok(productsByCategory);
        }

        //nome -> stock -> valor do stock // para orders -> tambem preciso de categoria e tipo de animal
        [HttpGet("/stockValueQuantity")]
        public async Task<IActionResult> StockValueAndQuantity()
        {
            var productsStockAndTotal = await _businessContext.Products
                .Include(c => c.Category)
                .Include(t => t.AnimalType)
                .Select(svq => new ProductStockQuantityPerCategoryDto
                {
                    Id = svq.Id,
                    Name = svq.Name,
                    Category = new CategoryDto
                    {
                        Id = svq.Category.Id,
                        Category = svq.Category.CategoryName,
                    },
                    AnimalType = new AnimalTypeDto
                    {
                        Id = svq.AnimalType.Id,
                        AnimalType = svq.AnimalType.Type,
                    },
                    PricePerUnit = svq.Price,
                    Quantity = svq.Quantity,
                    TotalInStock = svq.Quantity * svq.Price,
                }).ToListAsync();

            return Ok(productsStockAndTotal);
        }
        //editar para adicionar tambem o valor total
        [HttpGet("/valueInStock")]
        public async Task<IActionResult> ValueInStock()
        {
            //produtos -> categoria para nomes / agrupar em id e nome de cat e fazer a soma dos valores em stock
            var valuesInStockPerCat = await _businessContext.Products
                .Include(c => c.Category)
                .GroupBy(g => new { g.CategoryId, g.Category.CategoryName })
                .Select(vsc => new CategoryPricesDto
                {
                    CategoryId = vsc.Key.CategoryId,
                    CategoryName = vsc.Key.CategoryName,
                    Price = vsc.Sum(p => p.Price * p.Quantity)
                }).ToListAsync();

            return Ok(valuesInStockPerCat);
        }
    }
}