using ChatApp.Models;

namespace ChatApp.Services
{
    public interface ILoginService
    {
        public object LoginUser(LoginModel user);
        public string CreateToken(LoginModel user);
        public object GoogleAuth(string Token);
        public object CheckToken(string token);
        public object LogOut(string token);
    }
}
