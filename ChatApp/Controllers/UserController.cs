using ChatApp.Models;
using ChatApp.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
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
        public IActionResult GetUser(Guid id,string FirstName, string Email,long phoneNo)
        {
            var response = userService.GetUser(id,FirstName,Email,phoneNo);
            return Ok(response);
        }
        [HttpPut]
        public IActionResult UpdateUser(Guid id, string Email,UpdateUser user)
        {
            var response = userService.UpdateUser(id, Email,user);
            return Ok(response);
        }
        [HttpDelete]
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
        [HttpPost]
        [Route("Login")]
        public IActionResult Login(LoginModel model)
        {
            var response = userService.LoginUser(model);
            return Ok(response);
        }
        /*public async Task Login()
        {
            await HttpContext.ChallengeAsync(GoogleDefaults.AuthenticationScheme, new AuthenticationProperties()
            {
                RedirectUri = Url.Action("GoogleResponse")
            });
        }
        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            var claims = result.Principal.Identities.FirstOrDefault().Claims.Select(claim => new
            {
                claim.Issuer,
                claim.OriginalIssuer,
                claim.Type,
                claim.Value
            });
            return Ok(claims);
        }*/
    }
}
