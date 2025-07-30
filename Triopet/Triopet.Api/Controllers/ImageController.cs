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
    public class ImageController : ControllerBase
    {
        private readonly IBusinessContext _businessContext;

        public ImageController(IBusinessContext businessContext)
        {
            _businessContext = businessContext;
        }

        [HttpGet("/getimages")]
        public async Task<IActionResult> GetImages()
        {

            var images = await _businessContext.Images
                            .Where(img => !img.IsDeleted)
                            .ToListAsync();

            List<ImageDto> ImageList = new List<ImageDto>();

            foreach (var image in images)
            {
                var ImageDto = new ImageDto
                {
                    Id = image.Id,
                    Url = image.ImageUrl,
                    Name = image.ImageName
                };

                ImageList.Add(ImageDto);
            }

            return Ok(ImageList);
        }

        [HttpPost("/addimage")]
        public async Task<IActionResult> AddImage([FromBody] ImageDto? image)
        {
            if (image == null)
            {
                return BadRequest("Invalid image data.");
            }
            var newImage = new BusinessContext.Entities.Image
            {
                ImageName = image.Name,
                ImageUrl = image.Url,
                ProductId = image.ProductId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _businessContext.Images.Add(newImage);

            //await _businessContext.SaveChangesAsync(true);

            var result = await _businessContext.SaveChangesAsync(true);
            if (result > 0)
            {
                return Ok();
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "mensagem de erro");
            }

        }
        [HttpDelete("/deleteimage/{id}")]
        public async Task<IActionResult> DeleteImage(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid image data.");
            }
            var existingImage = await _businessContext.Images.FindAsync(id);

            if (existingImage == null)
            {
                return NotFound("Image not found.");
            }

            var activeImagesCount = await _businessContext.Images
            .Where(img => img.ProductId == existingImage.ProductId && !img.IsDeleted && img.Id != id)
            .CountAsync();

            if (activeImagesCount == 0)
            {
                return BadRequest("Cannot delete the last active image of the product.");
            }


            existingImage.IsDeleted = true;
            existingImage.UpdatedAt = DateTime.UtcNow;
            var affected = await _businessContext.SaveChangesAsync(true);

            if (affected > 0)
                return NoContent();
            else
                return StatusCode(500, "Failed to update the image deletion flag.");
        }
    }
}
