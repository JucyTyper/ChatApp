using ChatApp.Models;
using ChatApp.Services;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using System.Net;

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
        [Authorize]
        [Route("ResetPassword")]
        public IActionResult ResetPassword(ForgetPassword repass)
        {
            var response = passwordService.ResetPassUser(repass);
            return Ok(response);
        }
        [HttpPost]
        [Authorize]
        [Route("ChangePassword")]
        public IActionResult ChangePassword(ChangePassword repass)
        {
            var response = passwordService.ChangePassUser(repass);
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
