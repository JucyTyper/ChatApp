using ChatApp.Data;
using ChatApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
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

        //calling constructor and creating a database and IConfiguration variable reference variable
        public UserService(ChatAppDatabase _db,IConfiguration configuration)
        {
            this._db = _db;
            this.configuration = configuration;
        }
        public object CreateUser(AddUser user)
        {
            try
            {
                //checking if the user filled all entities 
                if (user.DateOfBirth== DateTime.MinValue||user.Email==string.Empty||user.FirstName == string.Empty||user.LastName == string.Empty)
                {
                    response2.StatusCode = 400;
                    response2.IsSuccess = false;
                    response2.Message = "Fill all details";
                    return response2;
                }
                //checking if the email already exist
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

                // Performing DOB validation here based specific requirements
                //for age less than 12
                if (age < 12)
                {
                    response2.StatusCode = 400;
                    response2.Message = "Not allowed to register. User is underage.Must be atleast 12 years old";
                    response2.IsSuccess = false;
                    return response2;
                }
                //for age greater than 130
                else if (age > 130)
                {
                    response2.StatusCode = 400;
                    response2.Message = "Not allowed to register. User is overage.Must be atmost 130 years old";
                    response2.IsSuccess = false;
                    return response2;
                }
                //Checking email pattern
                Console.WriteLine("hi");
                string regexPatternEmail = "^[a-z0-9._%+-]+@[a-z0-9.-]+\\.[a-z]{2,4}$";
                if (!Regex.IsMatch(user.Email, regexPatternEmail))
                {
                    response2.StatusCode = 400;
                    response2.Message = "Enter Valid Email";
                    response2.IsSuccess = false;
                    return response2;
                }
                //Checking Password Pattern
                string regexPatternPassword = "^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$";
                if (!Regex.IsMatch(user.Password, regexPatternPassword))
                {
                    response2.StatusCode = 400;
                    response2.Message = "Enter Valid Password";
                    response2.IsSuccess = false;
                    return response2;
                }
                //Checking PhoneNo. Pattern
                string regexPatternPhone = "^[6-9]\\d{9}$";
                if (!Regex.IsMatch(user.PhoneNo.ToString(), regexPatternPhone))
                {
                    response2.StatusCode = 400;
                    response2.Message = "Enter Valid PhoneNo";
                    response2.IsSuccess = false;
                    return response2;
                }
                // Creating and user entity and populating it 
                var userModel = new UserModel();
                userModel.FirstName = user.FirstName;
                userModel.LastName = user.LastName;
                userModel.Email = user.Email;
                userModel.PhoneNo = user.PhoneNo;
                userModel.DateOfBirth = user.DateOfBirth;
                //calling a userdefined function to save the password in hash
                RegisterPassword(userModel, user.Password);
                //adding user Entity in Database
                _db.users.Add(userModel);
                //Saving changes in Database
                _db.SaveChanges();
                //Creating a Login Entity of the register to generate Token
                var Login = new LoginModel();
                Login.Email = user.Email;
                Login.Password= user.Password;
                //Calling a user Defined Function to generate JWT token
                var token = CreateToken(Login);
                //generating Response
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
        //A userdefined Function to register Password
        private void RegisterPassword(UserModel user, string Password)
        {
            byte[] salt = Encoding.ASCII.GetBytes(configuration.GetSection("Password:salt").Value!);
            //Generating password hash and saving it
            user.Password = CreatePasswordHash(Password, salt);
        }
        //A function to generate passwordHash
        private byte[] CreatePasswordHash(string password, byte[] salt)
        {
            var hmac = new HMACSHA512(salt);
            var passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            return passwordHash;
        }
        //A function to create token
        public string CreateToken(LoginModel user)
        {
            //giving claims
            List<Claim> claims = new List<Claim>
            {

                new Claim(ClaimTypes.Name,user.Email),
                new Claim(ClaimTypes.Role,"Login")
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetSection("jwt:Key").Value));
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: cred
                );
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }
        // a function to get users
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
                if (user.DateOfBirth != DateTime.MinValue)
                {
                    TimeSpan ageTimeSpan = DateTime.Now - user.DateOfBirth;
                    int age = (int)(ageTimeSpan.Days / 365.25);

                    // Performing DOB validation here based specific requirements
                    //for age less than 12
                    if (age < 12)
                    {
                        response2.StatusCode = 400;
                        response2.Message = "User is underage.Must be atleast 12 years old";
                        response2.IsSuccess = false;
                        return response2;
                    }
                }

                if (user.PhoneNo != -1)
                {
                    //Checking PhoneNo. Pattern
                    string regexPatternPhone = "^[6-9]\\d{9}$";
                    if (!Regex.IsMatch(user.PhoneNo.ToString(), regexPatternPhone))
                    {
                        response2.StatusCode = 400;
                        response2.Message = "Enter Valid PhoneNo";
                        response2.IsSuccess = false;
                        return response2;
                    }
                }
                if (user.DateOfBirth!= DateTime.MinValue)
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
