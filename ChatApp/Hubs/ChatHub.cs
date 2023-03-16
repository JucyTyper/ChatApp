using ChatApp.Data;
using ChatApp.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Data;
using System.Security.Claims;

namespace ChatApp.Hubs
{
    [Authorize(AuthenticationSchemes =JwtBearerDefaults.AuthenticationScheme)]
    public class ChatHub:Hub
    {
        ResponseModel response = new ResponseModel();
        ResponseModel2 response2 = new ResponseModel2();
        public ChatHub(ChatAppDatabase _db)
        {
            this._db = _db;
        }
        private static Dictionary<string,string> userConnId= new Dictionary<string,string>();
        private readonly ChatAppDatabase _db;

        public async override Task<Task> OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            var user1 = httpContext.User;
            var email = user1.FindFirst(ClaimTypes.Name)?.Value;
            userConnId.Add(email, Context.ConnectionId);
            await Clients.All.SendAsync("refresh");
            return base.OnConnectedAsync();
        }
        public async Task<object> sendMessage(string email,string msg)
        {
            try
            {
                var httpContext = Context.GetHttpContext();
                var user1 = httpContext.User;
                var userEmail1 = user1.FindFirst(ClaimTypes.Name)?.Value;
                var chatEntity = _db.chatEntities.Where(x => (x.senderEmail == email || x.receiverEmail == email) && (x.senderEmail == userEmail1 || x.receiverEmail == userEmail1)).Select(x => x);
                var message = new MessageModel
                {
                    message = msg,
                    senderEmail = userEmail1,
                    receiverEmail = email,
                    chatMapId = chatEntity.First().chatId
                };
                chatEntity.First().lastUpdated = DateTime.Now;
                _db.messages.Add(message);
                _db.SaveChanges();

                var connId = userConnId.Where(x => x.Key == email).Select(x => x.Value).First();
                await Clients.Client(connId).SendAsync("receiveMessage", userEmail1, msg);
                return response2;
            }
            catch(Exception ex)
            {
                response2.StatusCode = 500;
                response2.Message = ex.Message;
                response2.IsSuccess = false;
                return response2;
            }
            
        }

        public async Task<string> addchat(string email)
        {
            var httpContext = Context.GetHttpContext();
            var user1 = httpContext.User;
            var userEmail1 = user1.FindFirst(ClaimTypes.Name)?.Value;
            var chatEntity = _db.chatEntities.Where(x => (x.senderEmail == email || x.receiverEmail == email) && (x.senderEmail == userEmail1 || x.receiverEmail == userEmail1)).Select(x => x);
            if(chatEntity.Count()==0) 
            {
                var chat = new ChatModel
                {
                    receiverEmail= email,
                    senderEmail = userEmail1,
                };
                _db.chatEntities.Add(chat);
                _db.SaveChanges();
                return chat.chatId.ToString();
            }
            return chatEntity.First().chatId.ToString();
        }
        public async Task<object> chatMap()
        {
            try
            {
                var httpContext = Context.GetHttpContext();
                var user1 = httpContext.User;
                var userEmail = user1.FindFirst(ClaimTypes.Name)?.Value;
                var chatMap = _db.chatEntities.Where(x => x.senderEmail == userEmail || x.receiverEmail == userEmail).Select(x => x).OrderByDescending(x => x.lastUpdated).Take(10).ToList();
                response.IsSuccess = true;
                response.Message = "Chat maps";
                response.Data = chatMap;
                return response;
            }
            catch (Exception ex)
            {
                response2.StatusCode = 500;
                response2.Message = ex.Message;
                response2.IsSuccess = false;
                return response2;
            }
        }
        public async Task<object> previousMessages(string MapId)
        {
            try
            {
                var prevMsg = _db.messages.Where(x => (x.chatMapId == new Guid(MapId)) && (x.isDeleted == false)).Select(x => x).OrderByDescending(x => x.dateTime).Take(10).ToList();
                var revPrevMsg = prevMsg.Select(x => x).Reverse();
                response.IsSuccess = true;
                response.Message = "All Messages";
                response.Data = revPrevMsg;
                return response;
            }
            catch (Exception ex)
            {
                response2.StatusCode = 500;
                response2.Message = ex.Message;
                response2.IsSuccess = false;
                return response2;
            }
        }
        public async Task<object> getUsers()
        {
            try
            {
                var httpContext = Context.GetHttpContext();
                var user1 = httpContext.User;
                var userEmail = user1.FindFirst(ClaimTypes.Name)?.Value;
                var chatMap = _db.chatEntities.Where(x => x.senderEmail == userEmail || x.receiverEmail == userEmail).Select(x => x).ToList();
                var onlineUsers = userConnId.Select(x => x.Key).ToList();
                List<string> connEmails = new List<string>();
                foreach (var room in chatMap)
                {
                    if (room.senderEmail != userEmail)
                    {
                        connEmails.Add(room.senderEmail);
                    }
                    else if (room.senderEmail == userEmail)
                    {
                        connEmails.Add(room.receiverEmail);
                    }
                }
                List<RoomViewModel> users = new List<RoomViewModel>();
                foreach (var email in connEmails)
                {
                    var user = _db.users.Where(x => x.Email == email).Select(x => x).First();
                    var chatRoomId = _db.chatEntities.Where(x => (x.senderEmail == email || x.receiverEmail == email) && (x.senderEmail == userEmail || x.receiverEmail == userEmail)).Select(x => x.chatId).ToList();
                    RoomViewModel connUser = new RoomViewModel
                    {
                        firstName = user.FirstName,
                        lastName = user.LastName,
                        email = user.Email,
                        chatRoomId = chatRoomId.First()
                    };
                    if (onlineUsers.Contains(email)) { connUser.isActive = true; }
                    else { connUser.isActive = false; }
                    users.Add(connUser);
                }
                response.IsSuccess = true;
                response.Message = "Online Users list";
                response.Data = users;
                return response;
            }
            catch (Exception ex)
            {
                response2.StatusCode = 500;
                response2.Message = ex.Message;
                response2.IsSuccess = false;
                return response2;
            }
            
        }
        public override Task OnDisconnectedAsync(Exception? exception)
        {
            var httpContext = Context.GetHttpContext();
            var user1 = httpContext.User;
            var email = user1.FindFirst(ClaimTypes.Name)?.Value;
            userConnId.Remove(email);
            return base.OnDisconnectedAsync(exception);
        }
    }
}
