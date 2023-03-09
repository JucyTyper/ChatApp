using ChatApp.Data;
using ChatApp.Models;
using Google.Apis.Auth;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace ChatApp.Services
{
    public class LoginService : ILoginService
    {
        ResponseModel response = new ResponseModel();
        private readonly ChatAppDatabase _db;
        private readonly IConfiguration configuration;

        public LoginService(ChatAppDatabase _db, IConfiguration configuration)
        {
            this._db = _db;
            this.configuration = configuration;
        }
        public object LoginUser(LoginModel user)
        {
            try
            {
                var _user = _db.users.Where(x => x.Email == user.Email).Select(x => x);
                if (_user.Count() == 0)
                {
                    response.StatusCode = 404;
                    response.Message = "User doesn't Exist";
                    return response;
                }

                if (!VerifyPasswordHash(user.Password, _user.First().Password))
                {
                    response.StatusCode = 404;
                    response.Message = "wrong Password";
                    return response;
                }
                var token = CreateToken(user);
                response.Data = token;
                return response;
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.Message = ex.Message;
                return response;
            }


        }
        private bool VerifyPasswordHash(string password, byte[] passwordHash)
        {
            byte[] salt = Encoding.ASCII.GetBytes(configuration.GetSection("Password:salt").Value!);
            using (var hmac = new HMACSHA512(salt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }
        public string CreateToken(LoginModel user)
        {
            List<Claim> claims = new List<Claim>
            { 
                new Claim(ClaimTypes.Name,user.Email),
                new Claim(ClaimTypes.Role ,"Login")
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(configuration.GetSection("jwt:Key").Value));
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha384Signature);
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: cred
                );
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }
        public async Task<object> GoogleAuth(string Token)
        {
            try
            {
                var GoogleUser =  await GoogleJsonWebSignature.ValidateAsync(Token);
                var _user = _db.users.Where(x => x.Email ==GoogleUser.Email).Select(x => x);
                if (_user.Count() != 0)
                {
                    var Log = new LoginModel();
                    Log.Email = GoogleUser.Email;
                    Log.Password = null;
                    string Authtoken = CreateToken(Log);
                    response.Data = Authtoken;
                    return response;
                }
                var user = new UserModel();
                user.Email = GoogleUser.Email;
                user.Password = null;
                user.FirstName = GoogleUser.GivenName;
                user.LastName = GoogleUser.FamilyName;
                user.DateOfBirth = DateTime.MinValue;
                user.PhoneNo = -1;
                var Login = new LoginModel();
                Login.Email = user.Email;
                Login.Password = null;
                string token = CreateToken(Login);
                response.Data= token;
                Console.WriteLine(token);
                return response;
            }
            catch(Exception ex)
            {
                response.StatusCode = 500;
                response.Message = ex.Message;
                return response;
            }
        }
    }
}
