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
        private readonly IFileService fileService;
        public FileController(IFileService fileservice)
        {
            this.fileService = fileservice;
        }
        [HttpPost]
        [Route("Profileimage")]
        [DisableRequestSizeLimit]
        public IActionResult uploadImage(string Email, [FromForm] fileUpload ImageFile)
        {
            var message = fileService.UploadImage(Email,ImageFile);
            return Ok(message);
        }
        [HttpPost]
        [Route("file")]
        [DisableRequestSizeLimit]
        public IActionResult uploadFile(int type,string Email, [FromForm] fileUpload rawFile)
        {
            var message = fileService.UploadFile(type,Email, rawFile);
            return Ok(message);
        }
    }
}
