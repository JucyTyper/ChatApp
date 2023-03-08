using ChatApp.Models;

namespace ChatApp.Services
{
    public interface ILoginService
    {
        public object LoginUser(LoginModel user);
        public string CreateToken(LoginModel user);
        public Task<object> GoogleAuth(string Token);
    }
}
