using ChatApp.Models;

namespace ChatApp.Services
{
    public interface IUserService
    {
        public object CreateUser(AddUser user);
        public object GetUser(Guid id, string searchString, string Email);
        public object GetUserProfile(string email);
        public object UpdateUser(Guid id,string email,UpdateUser user);
        public object DeleteUser(Guid id, string email);
        public ResponseModel2 CheckToken(string token);
    }
}
