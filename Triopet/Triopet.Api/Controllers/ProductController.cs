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
                .Include(c => c.Category)
                .Include(t => t.AnimalType)
                //.Include(i => i.Images)
                .Include(i => i.Images.Where(img => img.IsDeleted.Equals(false)))
                .Where(x => x.IsDeleted.Equals(false)).ToListAsync();

            var productsIds = products.Select(p => p.Id).ToList();

            List<ProductDto> productsList = new List<ProductDto>();
            foreach (var product in products)
            {
                var newProductDto = new ProductDto
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    PricePerUnit = product.Price,
                    Images = product.Images.Any()
                    ? new List<ImageDto>
                    {
                        product.Images
                            .OrderBy(img => img.Id)
                            .Select(img => new ImageDto
                            {
                                Id = img.Id,
                                Name = img.ImageName,
                                Url = img.ImageUrl,
                                ProductId = img.ProductId
                            })
                            .First()
                    }
                    : new List<ImageDto>(),
                    Category = new CategoryDto
                    {
                        Id = product.Category.Id,
                        Category = product.Category.CategoryName,
                    },
                    AnimalType = new AnimalTypeDto
                    {
                        Id = product.AnimalType.Id,
                        AnimalType = product.AnimalType.Type,
                    },
                };
                productsList.Add(newProductDto);
            }
            return Ok(productsList);
        }

        [HttpPost("/products")]
        public async Task<IActionResult> CreateProduct([FromBody] ProductDto newProduct)
        {
            if (newProduct.Images == null || newProduct.Images.Count == 0)
                return BadRequest("É obrigatório enviar pelo menos uma imagem.");

            var product = new Product
            {
                Name = newProduct.Name,
                Description = newProduct.Description,
                Price = newProduct.PricePerUnit,
                CategoryId = newProduct.Category.Id,
                AnimalTypeId = newProduct.AnimalType.Id,
                IsDeleted = false
            };

            _businessContext.Products.Add(product);
            await _businessContext.SaveChangesAsync(true);

            foreach (var imgDto in newProduct.Images)
            {
                var image = new Image
                {
                    ProductId = product.Id,
                    ImageUrl = imgDto.Url,
                    ImageName = imgDto.Name
                };
                _businessContext.Images.Add(image);
            }

            await _businessContext.SaveChangesAsync(true);

            return CreatedAtAction(nameof(GetProducts), new { id = product.Id }, newProduct);
        }

        [HttpPut("/products/{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Error trying to find product id");
            }

            var foundProduct = await _businessContext.Products.FindAsync(id);

            if (foundProduct == null)
            {
                return NotFound("Product not found");
            }

            foundProduct.IsDeleted = true;
            foundProduct.UpdatedAt = DateTime.UtcNow;

            await _businessContext.SaveChangesAsync(true);
            return Ok();
        }
    }
}
