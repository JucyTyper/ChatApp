using ChatApp.Models;

namespace ChatApp.Services
{
    public interface IUserService
    {
        public object ForgetPassUser(ForgetPassword cred);
        public object ResetPassUser(ResetPassword cred);
        public object CreateUser(AddUser user);
        public object LoginUser(LoginModel user);
        public object GetUser(Guid id, string FirstName, string Email);
        public object UpdateUser(Guid id,string email,UpdateUser user);
        public object DeleteUser(Guid id, string email);
        public string CreateToken(LoginModel user);
    }
}
