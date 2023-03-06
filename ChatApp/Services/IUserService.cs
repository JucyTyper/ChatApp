using ChatApp.Models;

namespace ChatApp.Services
{
    public interface IUserService
    {
        public object CreateUser(AddUser user);
        public object LoginUser(LoginModel user);
    }
}
