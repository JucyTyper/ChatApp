using ChatApp.Models;
using ChatApp.Services;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
        [Authorize(Roles = "Login")]
        public IActionResult GetUser(Guid id,string? searchString, string? Email)
        {
            string token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            ResponseModel2 temp = userService.CheckToken(token);
            if (temp.IsSuccess == false)
            {
                return Ok(temp);
            }
            var response = userService.GetUser(id,searchString,Email);
            return Ok(response);
        }
        [HttpPut]
        [Authorize(Roles = "Login")]
        public IActionResult UpdateUser(Guid id,UpdateUser user)
        {
            string token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            ResponseModel2 temp = userService.CheckToken(token);
            if (temp.IsSuccess == false)
            {
                return Ok(temp);
            }
            var user1 = HttpContext.User;
            var email = user1.FindFirst(ClaimTypes.Name)?.Value;
            var response = userService.UpdateUser(id, email,user);
            return Ok(response);
        }
        [HttpDelete]
        [Authorize(Roles = "Login")]
        public IActionResult DeleteUser(Guid id, string Email)
        {
            string token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            ResponseModel2 temp = userService.CheckToken(token);
            if (temp.IsSuccess == false)
            {
                return Ok(temp);
            }
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
