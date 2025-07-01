using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Triopet.BusinessContext;
using Triopet.BusinessContext.Entities;
using Triopet.Shared;

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

            var images = await _businessContext.Images.ToListAsync();

            List<Image> ImageList = new List<Image>();
            foreach (var image in images)
            {
                var ImageDto = new Image
                {
                    Id = image.Id,
                    ImageUrl = image.ImageUrl,
                    ImageName = image.ImageName
                };

                ImageList.Add(ImageDto);
            }

            return Ok(ImageList);
        }

        [HttpPost("/addimage")]
        public async Task<IActionResult> AddImage([FromBody] Image? image)
        {
            if (image == null)
            {
                return BadRequest("Invalid image data.");
            }
            var newImage = new BusinessContext.Entities.Image
            {
                ImageName = image.ImageName,
                ImageUrl = image.ImageUrl,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _businessContext.Images.Add(newImage);

            await _businessContext.SaveChangesAsync(true);

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
        [HttpDelete("/deleteimage")]
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
            existingImage.IsDeleted = true;
            existingImage.UpdatedAt = DateTime.UtcNow;
            await _businessContext.SaveChangesAsync(true);
            return NoContent();
        }
    }
}
