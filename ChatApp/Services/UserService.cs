using ChatApp.Data;
using ChatApp.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace ChatApp.Services
{
    public class UserService:IUserService
    {
        ResponseModel response = new ResponseModel();
        private readonly ChatAppDatabase _db;
        private readonly IConfiguration configuration;

        public UserService(ChatAppDatabase _db,IConfiguration configuration)
        {
            this._db = _db;
            this.configuration = configuration;
        }
        public object CreateUser(AddUser user)
        {
            try
            {
                var EmailCount = _db.users.Where(x=>x.Email == user.Email).Select(x=>x);
                if (EmailCount.Count() != 0)
                {
                    response.StatusCode = 400;
                    response.Message = "Email Already Exist";
                    return response;
                }
                var userModel = new UserModel();
                userModel.FirstName = user.FirstName;
                userModel.LastName = user.LastName;
                userModel.Email = user.Email;
                userModel.PhoneNo = user.PhoneNo;
                userModel.DateOfBirth = user.DateOfBirth;
                RegisterPassword(userModel, user.Password);
                _db.users.Add(userModel);
                _db.SaveChanges();
                response.StatusCode=200;
                response.Message = "User added Successfully";
                response.Data = userModel;
                return response;
            }
            catch(Exception ex) 
            {
                response.StatusCode = 500;
                response.Message = ex.Message;
                return response;
            }
            
        }
        private void RegisterPassword(UserModel user, string Password)
        {
            byte[] salt = Encoding.ASCII.GetBytes(configuration.GetSection("Password:salt").Value!);
            user.Password = CreatePasswordHash(Password, salt);
        }

        private byte[] CreatePasswordHash(string password, byte[] salt)
        {

            var hmac = new HMACSHA512(salt);
            var passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

            return passwordHash;
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
            catch(Exception ex)
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
        private string CreateToken(LoginModel user) 
        { 
            List<Claim> claims = new List<Claim>
            {

                new Claim(ClaimTypes.Name,user.Email)
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
        public object GetUser(Guid id, string FirstName, string Email, long phoneNo)
        {
            try
            {
                var users = _db.users.Where(x => (x.UserId == id || id == Guid.Empty) && (x.IsDeleted == false) && (x.PhoneNo == phoneNo || phoneNo == 0) && (x.FirstName == FirstName || FirstName == string.Empty)&&
                (x.Email == Email || Email == string.Empty)).Select(x=>x);
                if (users.Count() == 0)
                {
                    response.StatusCode = 400;
                    response.Message = "User Not Found";
                    return response;
                }
                response.Data= users;
                return response;
            }
            catch(Exception ex)
            {
                response.StatusCode = 500;
                response.Message = ex.Message;
                return response;
            }
        }
        public object UpdateUser(Guid id, string Email, UpdateUser user)
        {
            try
            {
                var _user = _db.users.Where(x => (x.UserId == id || id == Guid.Empty) && (x.IsDeleted == false) && 
                (x.Email == Email || Email == string.Empty)).Select(x => x);
                if (_user.Count() == 0)
                {
                    response.StatusCode = 400;
                    response.Message = "User Not Found";
                    return response;
                }
                _user.First().DateOfBirth = user.DateOfBirth;
                _user.First().PhoneNo = user.PhoneNo;
                _user.First().FirstName = user.FirstName;
                _user.First().LastName = user.LastName;
                _user.First().Updated = DateTime.Now;
                response.Data = _user;
                return response;
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.Message = ex.Message;
                return response;
            }
        }
        public object DeleteUser(Guid id, string Email)
        {
            try
            {
                var _user = _db.users.Where(x => (x.UserId == id || id == Guid.Empty) && (x.IsDeleted == false) &&
                (x.Email == Email || Email == string.Empty)).Select(x => x);
                if (_user.Count() == 0)
                {
                    response.StatusCode = 400;
                    response.Message = "User Not Found";
                    return response;
                }
                _user.First().IsDeleted = true;
                response.Data = _user;
                return response;
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.Message = ex.Message;
                return response;
            }
        }
    }
}
