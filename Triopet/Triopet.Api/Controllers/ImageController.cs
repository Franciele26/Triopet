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

        [HttpGet("/")]
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
    }
}
