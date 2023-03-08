using ChatApp.Data;
using ChatApp.Models;
using System.Net.Mail;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace ChatApp.Services
{
    public class PasswordService : IPasswordService
    {
        ResponseModel response = new ResponseModel();
        private readonly ChatAppDatabase _db;
        private readonly IConfiguration configuration;

        public PasswordService(ChatAppDatabase _db, IConfiguration configuration)
        {
            this._db = _db;
            this.configuration = configuration;
        }
        public object ResetPassUser(ForgetPassword cred)
        {
            try
            {
                var _user = _db.users.Where(x =>
                 (x.Email == cred.Email)).Select(x => x);
                if (_user.Count() == 0)
                {
                    response.StatusCode = 400;
                    response.Message = "User Not Found";
                    return response;
                }
                RegisterPassword(_user.First(), cred.Password);
                _db.SaveChanges();
                response.Message = "password Changed";
                return response;
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.Message = ex.Message;
                return response;
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
        public object ChangePassUser(ChangePassword cred)
        {
            try
            {
                var _user = _db.users.Where(x =>
                 (x.Email == cred.Email)).Select(x => x);
                if (_user.Count() == 0)
                {
                    response.StatusCode = 404;
                    response.Message = "User Not Found";
                    return response;
                }
                if (!VerifyPasswordHash(cred.OldPassword, _user.First().Password))
                {
                    response.StatusCode = 400;
                    response.Message = "wrong Old Password";
                    return response;
                }
                RegisterPassword(_user.First(), cred.NewPassword);
                _db.SaveChanges();
                response.Message = "password Changed";
                return response;
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.Message = ex.Message;
                return response;
            }
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
        public object ForgetPassword(ForgetPasswordMails mail)
        {
            var _user = _db.users.Where(x => x.Email == mail.email).Select(x => x);
            if (_user.Count() == 0)
            {
                response.StatusCode = 404;
                response.Message = "Email Not Found";
                return response;
            }
            MailMessage message = new MailMessage();
            // set the sender and recipient email addresses
            message.From = new MailAddress("ajay.joshi@chatapp.chicmic.co.in");
            message.To.Add(new MailAddress(mail.email));

            // set the subject and body of the email
            message.Subject = "Verify your account";
            message.Body = "Please verify your reset password attempt. Your one time link for verification is " + mail.url;

            // create a new SmtpClient object
            SmtpClient client = new SmtpClient();

            // set the SMTP server credentials and port
            client.Credentials = new NetworkCredential("ajay.joshi@chicmic.co.in", "Chicmic@2022");
            client.Host = "mail.chicmic.co.in";
            client.Port = 587;
            client.EnableSsl = true;
            // send the email
            client.Send(message);

            response.StatusCode = 200;
            response.Message = "Verification Email Sent";
            response.Data = string.Empty;
            return response;
        }
    }
}
