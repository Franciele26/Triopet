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
        public async Task<IActionResult> GetLowStockProducts()//list normal, que de para ordenar por nome ou quantidade
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
        public async Task<IActionResult> TopFiveMovements()//list de mud que de para filtrar por nome, cat, type ou totalmovements
        {
            var top5Movements = await _businessContext.Products
                .Where(p => !p.IsDeleted)
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
        public async Task<IActionResult> TopSoldItems(int categoryId)//3 tabelas - charts, em baixo, cada barra é uma categoria,
                                                                     //de lado fica o SoldQuantity
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
                topSalesList.Add(new TopProductsSoldPerCategoryDto
                {
                    Id = item.Id,
                    Name = item.Name,
                    SoldQuantity = totalSold,
                });
            }
            //ir buscar SO o top 3 dessa lista
            var topThree = topSalesList.OrderByDescending(s => s.SoldQuantity)
                .Take(3).ToList();


            return Ok(topThree);
        }


        //usar CategoryPrices para quando for so preciso os campos das categorias + preço

        [HttpGet("/averagePrice/")]
        public async Task<IActionResult> AveragePricePerCategory()//vai ser um pie chart
        {
            //produtos -> categoria para name -> select para criar o objt como dto
            var productsByCategory = await _businessContext.Products
                .Where(p => !p.IsDeleted)
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
        public async Task<IActionResult> StockValueAndQuantity()//vai ser uma lista
        {
            var productsStockAndTotal = await _businessContext.Products
                .Where(p => !p.IsDeleted)
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
                })
                .ToListAsync();

            return Ok(productsStockAndTotal);
        }

        [HttpGet("/valueInStock")]
        public async Task<IActionResult> ValueInStock()//nao sei, tem de mostrar nome, preço, para cada e um total
        {
            //produtos -> categoria para nomes / agrupar em id e nome de cat e fazer a soma dos valores em stock
            var valuesInStockPerCat = await _businessContext.Products
                .Where(p => !p.IsDeleted)
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

        [HttpGet("/entriesPerCategory")]
        public async Task<IActionResult> EntriesPerCategory()//nao sei, tem de mostrar nome, preço, para cada e um total
        {
            //produtos -> categoria para nomes / agrupar em id e nome de cat e fazer a soma dos valores em stock
            var entriesPerCategory = await _businessContext.ProductEntries
                .Include(pe => pe.Product)
                .ThenInclude(c => c.Category)
                .GroupBy(pe => new { pe.Product.CategoryId, pe.Product.Category.CategoryName })
                .Select(g => new MovementPerCategory
                {
                    CategoryId = g.Key.CategoryId,
                    CategoryName = g.Key.CategoryName,
                    NumberMovements = g.Count()
                }).ToListAsync();

            return Ok(entriesPerCategory);
        }

        [HttpGet("/exitsPerCategory")]
        public async Task<IActionResult> ExitsPerCategory()//nao sei, tem de mostrar nome, preço, para cada e um total
        {
            var sevenDaysAgo = DateTime.Now.AddDays(-7);

            //produtos -> categoria para nomes / agrupar em id e nome de cat e fazer a soma dos valores em stock
            var exitsPerCategory = await _businessContext.ProductExits
                .Include(pe => pe.Product)
                .ThenInclude(c => c.Category)
                .GroupBy(pe => new { pe.Product.CategoryId, pe.Product.Category.CategoryName })
                .Select(g => new MovementPerCategory
                {
                    CategoryId = g.Key.CategoryId,
                    CategoryName = g.Key.CategoryName,
                    NumberMovements = g.Count()
                }).ToListAsync();

            return Ok(exitsPerCategory);
        }

        [HttpGet("/sevenLastDaysEntries")]
        public async Task<IActionResult> SevenLastDaysEntries()//nao sei, tem de mostrar nome, preço, para cada e um total
        {
            var sevenDaysAgo = DateTime.Now.AddDays(-7);
            //produtos -> categoria para nomes / agrupar em id e nome de cat e fazer a soma dos valores em stock
            var sevenLastDaysEntries = await _businessContext.ProductEntries
                .Include(pe => pe.Product)
                .ThenInclude(c => c.Category)
                .Where(d => d.Entry.EntryDate >= sevenDaysAgo)
                .GroupBy(pe => new { pe.Product.CategoryId, pe.Product.Category.CategoryName })
                .Select(g => new MovementPerCategory
                {
                    CategoryId = g.Key.CategoryId,
                    CategoryName = g.Key.CategoryName,
                    NumberMovements = g.Count()
                }).ToListAsync();

            return Ok(sevenLastDaysEntries);
        }

        [HttpGet("/sevenLastDaysExits")]
        public async Task<IActionResult> SevenLastDaysExits()//nao sei, tem de mostrar nome, preço, para cada e um total
        {
            var sevenDaysAgo = DateTime.Now.AddDays(-7);

            //produtos -> categoria para nomes / agrupar em id e nome de cat e fazer a soma dos valores em stock
            var SevenLastDaysExits = await _businessContext.ProductExits
                .Include(pe => pe.Product)
                .ThenInclude(c => c.Category)
                 .Where(d => d.Exit.ExitDate <= sevenDaysAgo)
                .GroupBy(pe => new { pe.Product.CategoryId, pe.Product.Category.CategoryName })
                .Select(g => new MovementPerCategory
                {
                    CategoryId = g.Key.CategoryId,
                    CategoryName = g.Key.CategoryName,
                    NumberMovements = g.Count()
                }).ToListAsync();

            return Ok(SevenLastDaysExits);
        }
    }
}