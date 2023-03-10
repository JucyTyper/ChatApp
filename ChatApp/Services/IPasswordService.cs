using ChatApp.Models;

namespace ChatApp.Services
{
    public interface IPasswordService
    {
        public object ResetPassUser(string Email, ForgetPassword cred);
        public object ChangePassUser(string Email, ChangePassword cred);
        public object ForgetPassword(ForgetPasswordMails mails);
        public ResponseModel2 CheckToken(string token);
    }
}
