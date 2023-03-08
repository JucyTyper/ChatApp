using ChatApp.Models;

namespace ChatApp.Services
{
    public interface IPasswordService
    {
        public object ResetPassUser(ForgetPassword cred);
        public object ChangePassUser(ChangePassword cred);
        public object ForgetPassword(ForgetPasswordMails mails);
    }
}
