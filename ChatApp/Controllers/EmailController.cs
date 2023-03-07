using ChatApp.Models;
using MailKit.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Net.Mail;
using System.Net;

namespace ChatApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> SentEmail(ForgetPasswordMails mail)
        {

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
            var response = new ResponseModel();
            // send the email
            client.Send(message);

            response.StatusCode = 200;
            response.Message = "Verification Email Sent";
            response.Data = string.Empty;
            return Ok(response);


            /*var client = new SendGridClient("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");
            var from = new EmailAddress("joshi2312002@gmail.com", "Ajay Joshi");
            var subject = "Sending with SendGrid is Fun";
            var to = new EmailAddress(mail.email, "Ajay Joshi");
            var plainTextContent = mail.url;
            var htmlContent = "<strong>" + mail.url + "</strong>";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);
            return Ok(response);*/
        }
    }
}
