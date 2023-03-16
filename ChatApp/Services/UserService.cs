using ChatApp.Data;
using ChatApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace ChatApp.Services
{
    public class UserService:IUserService
    {
        ResponseModel response = new ResponseModel();
        ResponseModel2 response2 = new ResponseModel2();
        UserResponse DataOut= new UserResponse();
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
                if (user.DateOfBirth== DateTime.MinValue||user.Email==string.Empty||user.FirstName == string.Empty||user.LastName == string.Empty)
                {
                    response2.StatusCode = 400;
                    response2.IsSuccess = false;
                    response2.Message = "Fill all details";
                    return response2;
                }
                var EmailCount = _db.users.Where(x=>x.Email == user.Email).Select(x=>x);
                if (EmailCount.Count() != 0)
                {
                    response2.StatusCode = 400;
                    response2.Message = "Email Already Exist";
                    response2.IsSuccess = false;
                    return response2;
                }
                TimeSpan ageTimeSpan = DateTime.Now - user.DateOfBirth;
                int age = (int)(ageTimeSpan.Days / 365.25);

                // Perform your DOB validation here based on your specific requirements
                if (age < 12)
                {
                    // The user is not enough
                    response2.StatusCode = 400;
                    response2.Message = "Not allowed to register. User is underage.Must be atleast 12 years old";
                    response2.IsSuccess = false;
                    return response2;
                }
                else if (age > 130)
                {
                    response2.StatusCode = 400;
                    response2.Message = "Not allowed to register. User is overage.Must be atmost 130 years old";
                    response2.IsSuccess = false;
                    return response2;
                }
                string regexPatternEmail = "^[a-z0-9._%+-]+@[a-z0-9.-]+\\.[a-z]{2,4}$";
                if (!Regex.IsMatch(user.Email, regexPatternEmail))
                {
                    response2.StatusCode = 400;
                    response2.Message = "Enter Valid Email";
                    response2.IsSuccess = false;
                    return response2;
                }
                string regexPatternPassword = "^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$";
                if (!Regex.IsMatch(user.Password, regexPatternPassword))
                {
                    response2.StatusCode = 400;
                    response2.Message = "Enter Valid Password";
                    response2.IsSuccess = false;
                    return response2;
                }
                string regexPatternPhone = "^[6-9]\\d{9}$";
                if (!Regex.IsMatch(user.PhoneNo.ToString(), regexPatternPhone))
                {
                    response2.StatusCode = 400;
                    response2.Message = "Enter Valid PhoneNo";
                    response2.IsSuccess = false;
                    return response2;
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
                var Login = new LoginModel();
                Login.Email = user.Email;
                Login.Password= user.Password;
                var token = CreateToken(Login);
                response.StatusCode=200;
                response.Message = "User added Successfully";
                DataOut.Token = token;
                DataOut.Name = userModel.FirstName;
                DataOut.Email = userModel.Email;
                DataOut.UserID = userModel.UserId;
                response.Data = DataOut;
                return response;
            }
            catch(Exception ex) 
            {
                response2.StatusCode = 500;
                response2.Message = ex.Message;
                response2.IsSuccess = false;
                return response2;
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
                new Claim(ClaimTypes.Role,"Login")
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(configuration.GetSection("jwt:Key").Value));
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: cred
                );
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }
        public object GetUser(Guid id, string searchString, string Email)
        {
            try
            {
                var users = _db.users.Where(x => (x.UserId == id || id == Guid.Empty) && (x.IsDeleted == false) && (EF.Functions.Like(x.FirstName,"%"+searchString+"%")|| EF.Functions.Like(x.LastName, "%" + searchString + "%")|| EF.Functions.Like(x.FirstName + x.LastName, "%" + searchString + "%") || searchString == null)&&
                (x.Email == Email || Email == null)).Select(x=> new {x.UserId,x.Email,x.FirstName,x.LastName,x.DateOfBirth,x.Created,x.LastActive,x.PhoneNo,x.Updated});
                if (users.Count() == 0)
                {
                    response.StatusCode = 400;
                    response.Message = "User Not Found";
                    response.Data= users;
                    response.IsSuccess = true;
                    return response;
                }
                response.Data= users;
                return response;
            }
            catch(Exception ex)
            {
                response2.StatusCode = 500;
                response2.Message = ex.Message;
                response2.IsSuccess = false;
                return response2;
            }
        }
        public object UpdateUser(Guid id, string Email, UpdateUser user)
        {
            try
            {
                var _user = _db.users.Where(x => (x.UserId == id || id == Guid.Empty) && (x.IsDeleted == false) && 
                (x.Email == Email || Email == null)).Select(x => x);
                if (_user.Count() == 0)
                {
                    response2.StatusCode = 400;
                    response2.Message = "User Not Found";
                    response2.IsSuccess = false;
                    return response2;
                }
                if(user.DateOfBirth!= DateTime.MinValue)
                    _user.First().DateOfBirth = user.DateOfBirth;
                if (user.PhoneNo != -1)
                    _user.First().PhoneNo = user.PhoneNo;
                if(user.FirstName != string.Empty)
                    _user.First().FirstName = user.FirstName;
                if (user.LastName != string.Empty)
                    _user.First().LastName = user.LastName;
                _user.First().Updated = DateTime.Now;
                _db.SaveChanges();
                response.Data = _user;
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
        public object DeleteUser(Guid id, string Email)
        {
            try
            {
                var _user = _db.users.Where(x => (x.UserId == id || id == Guid.Empty) && (x.IsDeleted == false) &&
                (x.Email == Email || Email == null)).Select(x => x);
                if (_user.Count() == 0)
                {
                    response2.StatusCode = 400;
                    response2.Message = "User Not Found";
                    response2.IsSuccess = false;
                    return response2;
                }

                _user.First().IsDeleted = true;
                _db.SaveChanges();
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
        public ResponseModel2 CheckToken(string token)
        {
            var tokenCheck = _db.blackListTokens.Where(x => x.token == token.ToString()).Select(x => x);
            if (tokenCheck.Count() != 0)
            {
                response2.StatusCode = 400;
                response2.Message = "Invalid Token";
                response2.IsSuccess = false;
                return response2;
            }
            return response2;
        }
    }
}
