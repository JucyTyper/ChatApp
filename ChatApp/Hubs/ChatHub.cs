using ChatApp.Data;
using ChatApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json.Linq;
using SendGrid.Helpers.Mail;
using System;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace ChatApp.Hubs
{
    //[Authorize(]
    public class ChatHub:Hub
    {
        public ChatHub(ChatAppDatabase _db)
        {
            this._db = _db;
        }
        private static Dictionary<string,string> userConnId= new Dictionary<string,string>();
        private readonly ChatAppDatabase _db;

        public async Task<string> saveData(string email)
        {
            var con = userConnId.Where(x => x.Key == email).Select(x => x);
            if (con.Count() == 0)
            {
                userConnId.Add(email, Context.ConnectionId);
            }
            userConnId.Remove(email);
            userConnId.Add(email.ToLower(), Context.ConnectionId);
            return Context.ConnectionId;
        }
        public override Task OnConnectedAsync()
        {
            //var httpContext = Context.GetHttpContext();
            //var user1 = httpContext.User;
            //var email = user1.FindFirst(ClaimTypes.Name)?.Value;
            //var user = _db.users.Where(x => x.Email == email).Select(x=>x);
            //user.First().isOnline = true;
            //_db.SaveChanges();
            //userConnId.Add(Context.ConnectionId, email);
            return base.OnConnectedAsync();
        }
        public async Task<string> SendMessage(string userEmail,string email,string msg)
        {
            //var httpContext = Context.GetHttpContext();
            //var user1 = httpContext.User;
            //var userEmail = user1.FindFirst(ClaimTypes.Name)?.Value;
            var chatEntity = _db.chatEntities.Where(x => (x.senderEmail == email || x.receiverEmail == email) && (x.senderEmail == userEmail || x.receiverEmail == userEmail)).Select(x => x);
            var message = new MessageModel
            {
                message= msg,
                senderEmail = userEmail,
                receiverEmail = email,
                chatMapId = chatEntity.First().chatId
            };
            _db.messages.Add(message);
            _db.SaveChanges();
            var connId = userConnId.Where(x => x.Key == email).Select(x => x.Value).First();
            await Clients.Client(connId).SendAsync("receiveMessage", msg);
            return "Done";
        }

        public async Task<string> Addchat(string userEmail,string email)
        {
            //var httpContext = Context.GetHttpContext();
            //var user1 = httpContext.User;
            //var userEmail = user1.FindFirst(ClaimTypes.Name)?.Value;
            var chatEntity = _db.chatEntities.Where(x => (x.senderEmail == email || x.receiverEmail == email) && (x.senderEmail == userEmail || x.receiverEmail == userEmail)).Select(x => x);
            if(chatEntity.Count()==0) 
            {
                var chat = new ChatModel
                {
                    receiverEmail= email,
                    senderEmail = userEmail,
                };
                _db.chatEntities.Add(chat);
                _db.SaveChanges();
                return chat.chatId.ToString();
            }
            return chatEntity.First().chatId.ToString();
        }
        public override Task OnDisconnectedAsync(Exception? exception)
        {
            //var httpContext = Context.GetHttpContext();
            //var user1 = httpContext.User;
            //var email = user1.FindFirst(ClaimTypes.Name)?.Value;
            //var user = _db.users.Where(x => x.Email == email).FirstOrDefault();
            //user.isOnline = false;
            //userConnId.Remove(Context.ConnectionId);
            return base.OnDisconnectedAsync(exception);
        }
    }
}
