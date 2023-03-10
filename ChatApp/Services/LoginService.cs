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
        ResponseModel2 response2 = new ResponseModel2();
        UserResponse DataOut = new UserResponse();
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
                    response2.StatusCode = 404;
                    response2.Message = "User doesn't Exist";
                    response2.IsSuccess = false;
                    return response2;
                }

                if (!VerifyPasswordHash(user.Password, _user.First().Password))
                {
                    response2.StatusCode = 404;
                    response2.Message = "wrong Password";
                    response2.IsSuccess=false;
                    return response2;
                }
                var token = CreateToken(user);
                DataOut.Token = token;
                DataOut.Name = _user.First().FirstName;
                DataOut.Email = _user.First().Email;
                DataOut.UserID = _user.First().UserId;
                response.Data = DataOut;
                return response;
            }
            catch (Exception ex)
            {
                response2.StatusCode = 500;
                response2.Message = ex.Message;
                response2.IsSuccess = false;
                return response2;
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
        public object GoogleAuth(string Token)
        {
            try
            {
                var GoogleUser =  GoogleJsonWebSignature.ValidateAsync(Token);
                //var _user = _db.users.Where(x => x.Email == GoogleUser.Email).Select(x => new {x.Email,x.UserId,x.FirstName,x.LastName});
                var _user = _db.users.Where(x => x.Email == GoogleUser.Result.Email ).Select(x => new { x.UserId, x.Email, x.FirstName, x.LastName, x.DateOfBirth, x.Created, x.LastActive, x.PhoneNo, x.Updated });
                
                if (_user.Count() != 0)
                {
                    var Log = new LoginModel();
                    Log.Email = GoogleUser.Result.Email;
                    Console.WriteLine(Log.Email);
                    Log.Password = null;
                    string Authtoken = CreateToken(Log);
                    DataOut.Token = Authtoken;
                    DataOut.Name = GoogleUser.Result.GivenName;
                    DataOut.Email = GoogleUser.Result.Email;
                    DataOut.UserID = _user.First().UserId;
                    response.Message = "user Logged in";
                    response.IsSuccess = true;
                    response.Data = DataOut;
                    return response;
                }
                var user = new UserModel();
                user.Email = GoogleUser.Result.Email;
                user.Password = CreatePasswordHash("temp");
                user.FirstName = GoogleUser.Result.GivenName;
                user.LastName = GoogleUser.Result.FamilyName;
                user.DateOfBirth = DateTime.MinValue;
                user.PhoneNo = 0;
                var Login = new LoginModel();
                Login.Email = user.Email;
                Login.Password = null;
                string token = CreateToken(Login);
                DataOut.Token = token;
                DataOut.Name = user.FirstName;
                DataOut.Email = user.Email;
                DataOut.UserID = user.UserId;
                response.Data= DataOut;
                _db.users.Add(user);
                _db.SaveChanges();
                return response;
            }
            catch(Exception ex)
            {
                response2.StatusCode = 500;
                response2.Message = ex.StackTrace;
                response2.IsSuccess = false;
                return response2;
            }
        }
        private byte[] CreatePasswordHash(string password)
        {
            byte[] salt = Encoding.ASCII.GetBytes(configuration.GetSection("Password:salt").Value!);
            var hmac = new HMACSHA512(salt);
            var passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

            return passwordHash;
        }
        public object? CheckToken(string token)
        {
            var tokenCheck = _db.blackListTokens.Where(x=> x.token== token).Count();  
            if(tokenCheck != 0) 
            {
                response.StatusCode = 400;
                response.Message = "Invalid Token";
                response.IsSuccess = false;
                return response2;
            }
            return null;
        }
        public object LogOut(string token)
        {
            var bLtoken = new BlackListToken
            {
                token= token,
            };
            _db.blackListTokens.Add(bLtoken);
            _db.SaveChanges();
            response2.StatusCode = 200;
            response2.Message = "Logged Out";
            response2.IsSuccess = true;
            return response2;
        }
    }
}
