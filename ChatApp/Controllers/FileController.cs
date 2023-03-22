using ChatApp.Models;
using ChatApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Security.Claims;

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
        [Authorize(Roles = "Login")]
        [DisableRequestSizeLimit]
        public IActionResult uploadImage( [FromForm] fileUpload ImageFile)
        {
            var user1 = HttpContext.User;
            var email = user1.FindFirst(ClaimTypes.Name)?.Value;
            var message = fileService.UploadImage(email,ImageFile);
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
