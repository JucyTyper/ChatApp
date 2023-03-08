using ChatApp.Models;
using ChatApp.Services;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService userService;

        public UserController(IUserService  userservice)
        {
                this.userService = userservice;
        }
        [HttpGet]
        [Authorize]
        public IActionResult GetUser(Guid id,string? FirstName, string? Email)
        {
            var response = userService.GetUser(id,FirstName,Email);
            return Ok(response);
        }
        [HttpPut]
        [Authorize]
        public IActionResult UpdateUser(Guid id, string Email,UpdateUser user)
        {
            var response = userService.UpdateUser(id, Email,user);
            return Ok(response);
        }
        [HttpDelete]
        [Authorize]
        public IActionResult DeleteUser(Guid id, string Email)
        {
            var response = userService.DeleteUser(id, Email);
            return Ok(response);
        }
        [HttpPost]
        [Route("Registration")]
        public IActionResult RegisterUser(AddUser user)
        {
            var response = userService.CreateUser(user);
            return Ok(response);
        }
    }
}
