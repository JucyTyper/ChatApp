using ChatApp.Models;

namespace ChatApp.Services
{
    public interface IUserService
    {
        public object CreateUser(AddUser user);
        public object GetUser(Guid id, string FirstName, string Email);
        public object UpdateUser(Guid id,string email,UpdateUser user);
        public object DeleteUser(Guid id, string email);
    }
}
