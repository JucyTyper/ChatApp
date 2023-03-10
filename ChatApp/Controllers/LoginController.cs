using ChatApp.Models;
using MailKit.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Net.Mail;
using System.Net;
using ChatApp.Services;
using Microsoft.AspNetCore.Authorization;
using Google.Apis.Auth;

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
