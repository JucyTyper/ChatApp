using ChatApp.Models;
using ChatApp.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly IImageService ImageService;

        public FileController(IImageService Imageservice)
        {
            this.ImageService = Imageservice;
        }
        [HttpPost]
        [DisableRequestSizeLimit]
        public IActionResult UploadImage(string Email, [FromForm] ImageUpload ImageFile)
        {
            var message = ImageService.UploadImage(Email,ImageFile);
            return Ok(message);
        }
    }
}
