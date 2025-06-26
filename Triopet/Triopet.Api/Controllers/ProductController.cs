using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Triopet.BusinessContext;
using Triopet.Shared.Models;


namespace Triopet.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IBusinessContext _businessContext;
        public ProductController(IBusinessContext businessContext)
        {
            _businessContext = businessContext;
        }

        [HttpGet("/products")]
        public async Task<IActionResult> GetProducts()
        {
            var products = await _businessContext.Products
                .Where(x => x.IsDeleted.Equals(false)).ToListAsync();

            var productsIds = products.Select(p => p.Id).ToList();

            List<ProductDto> productsList = new List<ProductDto>();
            foreach (var product in products)
            {
                //criar categoria
                var category = await _businessContext.Categories
                    .Select(c => new CategoryDto 
                    {
                        Id = c.Id,
                        Category = c.CategoryName,
                    })
                    .FirstOrDefaultAsync(c => c.Id == product.CategoryId);

                //criar tipo
                var animalType = await _businessContext.AnimalTypes
                    .Select(a => new AnimalTypeDto
                    {
                        Id = a.Id,
                        AnimalType = a.Type,
                    })
                    .FirstOrDefaultAsync(a => a.Id == product.AnimalTypeId);

                //Converter firstImage para imageDto para ser utilizado em baixo
                var firstImage = await _businessContext.Images
                        .Where(i => i.ProductId == product.Id)
                        .OrderBy(i => i.ProductId)
                        .Take(1)
                        .Select(i => new ImageDto
                        {
                            Id = i.Id,
                            Url = i.ImageUrl,
                            Name = i.ImageName,
                        })
                        .ToListAsync();

                var newProductDto = new ProductDto
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    PricePerUnit = product.Price,
                    Images = firstImage,
                    Category = category,
                    AnimalType = animalType,
                };
                productsList.Add(newProductDto);
            }
            return Ok(productsList);
        }
    }
}
