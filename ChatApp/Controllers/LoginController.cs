using ChatApp.Models;

using Microsoft.AspNetCore.Mvc;
using ChatApp.Services;
using Microsoft.AspNetCore.Authorization;

namespace ChatApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly ILoginService loginService;

        public LoginController(ILoginService loginservice)
        {
            this.loginService = loginservice;
        }
        [HttpPost]
        public IActionResult Login(LoginModel model)
        {
            var response = loginService.LoginUser(model);
            return Ok(response);
        }
        [HttpPost]
        [Route("GoogleAuth")]
        public async Task<IActionResult> Test(GoogleToken Token)
        {
            var response = loginService.GoogleAuth(Token.Token);
            return Ok(response);
        }
        [HttpPost]
        [Route("logOut")]
        [Authorize(Roles = "Login")]
        public IActionResult Logout()
        {
            string token = Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var response = loginService.LogOut(token);
            return Ok(response);
        }
    }
}
