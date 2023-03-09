using ChatApp.Models;
using ChatApp.Services;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using System.Net;
using System.Security.Claims;

namespace ChatApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PasswordController : ControllerBase
    {
        private readonly IPasswordService passwordService;

        public PasswordController(IPasswordService passwordservice)
        {
            this.passwordService = passwordservice;
        }
        [HttpPost]
        [Authorize(Roles = "Password")]
        [Route("ResetPassword")]
        public IActionResult ResetPassword(ForgetPassword repass)
        {
            var user = HttpContext.User;
            var email = user.FindFirst(ClaimTypes.Name)?.Value;
            var response = passwordService.ResetPassUser(email,repass);
            return Ok(response);
        }
        [HttpPost]
        [Authorize(Roles = "Login")]
        [Route("ChangePassword")]
        public IActionResult ChangePassword(ChangePassword repass)
        {
            var user = HttpContext.User;
            var email = user.FindFirst(ClaimTypes.Name)?.Value;
            var response = passwordService.ChangePassUser(email,repass);
            return Ok(response);
        }
        [HttpPost]
        [Route("ForgetPassword")]
        public IActionResult ForgetPasssword(ForgetPasswordMails mail)
        {
            var response = passwordService.ForgetPassword(mail);
            return Ok(response);
        }
    }
}
