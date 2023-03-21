using ChatApp.Data;
using ChatApp.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.ComponentModel;
using System.Data;
using System.Security.Claims;

namespace ChatApp.Hubs
{
    [Authorize(AuthenticationSchemes =JwtBearerDefaults.AuthenticationScheme)]
    public class ChatHub:Hub
    {
        // initializing response models
        ResponseModel response = new ResponseModel();
        ResponseModel2 response2 = new ResponseModel2();
        // calling constructor and adding database
        public ChatHub(ChatAppDatabase _db)
        {
            this._db = _db;
        }
        private static Dictionary<string,string> userConnId= new Dictionary<string,string>();
        private readonly ChatAppDatabase _db;

        //overriding a function from baseclass
        public async override Task<Task> OnConnectedAsync()
        {
            //getting email from token
            var httpContext = Context.GetHttpContext();
            var user1 = httpContext.User;
            var email = user1.FindFirst(ClaimTypes.Name)?.Value;
            //adding email and connectionId in connected person list
            userConnId.Add(email, Context.ConnectionId);
            //Sending everyone a message to refresh
            await Clients.All.SendAsync("refresh");
            return base.OnConnectedAsync();
        }
        // a function to send messages
        public async Task<object> sendMessage(string email,string msg,int type,string url)
        {
            try
            {
                //getting email from token
                var httpContext = Context.GetHttpContext();
                var user1 = httpContext.User;
                var userEmail1 = user1.FindFirst(ClaimTypes.Name)?.Value;
                //getting saved Channel between two users
                var chatEntity = _db.chatEntities.Where(x => (x.senderEmail == email || x.receiverEmail == email) && (x.senderEmail == userEmail1 || x.receiverEmail == userEmail1)).Select(x => x);
                //updating the updation time
                chatEntity.First().lastUpdated = DateTime.Now;
                //creating a message object which contains all data of a message
                var message = new MessageModel
                {
                    message = msg,
                    senderEmail = userEmail1,
                    receiverEmail = email,
                    type = type,
                    fileUrl = url,
                    chatMapId = chatEntity.First().chatId
                };
                //Adding message object to the database
                _db.messages.Add(message);
                //saving the database
                _db.SaveChanges();
                //getting connection Id of the receiver
                var connId = userConnId.Where(x => x.Key == email).Select(x => x.Value);
                //checking if connection Id exist
                if(connId.Count() != 0 ) 
                {
                    //sending message to the receiver
                    await Clients.Client(connId.First()).SendAsync("receiveMessage", message);
                    await Clients.Client(connId.First()).SendAsync("refresh");
                }
                // sending message to the sender
                await Clients.Caller.SendAsync("receiveMessage", message);
                // sending refresh message to sender 
                await Clients.Caller.SendAsync("refresh");

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
        //a function to create chat room
        public async Task<string> addchat(string email)
        {
            //Getting email of user
            var httpContext = Context.GetHttpContext();
            var user1 = httpContext.User;
            var userEmail1 = user1.FindFirst(ClaimTypes.Name)?.Value;
            //Getting chatentity of the sender and receiver
            var chatEntity = _db.chatEntities.Where(x => (x.senderEmail == email || x.receiverEmail == email) && (x.senderEmail == userEmail1 || x.receiverEmail == userEmail1)).Select(x => x);
            //Checking if chatroom exist or not ,if not then creating a new one 
            if(chatEntity.Count()==0) 
            {
                var chat = new ChatModel
                {
                    receiverEmail= email,
                    senderEmail = userEmail1,
                };
                //adding new chatroom to the database
                _db.chatEntities.Add(chat);
                _db.SaveChanges();
                //returning chatroomId
                return chat.chatId.ToString();
            }
            //returning chatroom Id
            return chatEntity.First().chatId.ToString();
        }
        public async Task<object> chatMap()
        {
            try
            {
                //Getting email
                var httpContext = Context.GetHttpContext();
                var user1 = httpContext.User;
                var userEmail = user1.FindFirst(ClaimTypes.Name)?.Value;
                //Getting All Chatrooms
                var chatMap = _db.chatEntities.Where(x => x.senderEmail == userEmail || x.receiverEmail == userEmail).Select(x => x).OrderByDescending(x => x.lastUpdated).Take(10).ToList();
                //Generating response
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
        public async Task<object> previousMessages(string MapId,int pageNumber)
        {
            try
            {
                //getting messages
                var prevMsg = _db.messages.Where(x => (x.chatMapId == new Guid(MapId)) && (x.isDeleted == false)).Select(x => x).OrderByDescending(x => x.dateTime).Skip((pageNumber-1)*20).Take(20).ToList();
                //reversing order of the messages
                var revPrevMsg = prevMsg.Select(x => x).Reverse();
                // Generating response
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
        // a method to get user with online status
        public async Task<object> getUsers()
        {
            try
            {
                //Getting Email
                var httpContext = Context.GetHttpContext();
                var user1 = httpContext.User;
                var userEmail = user1.FindFirst(ClaimTypes.Name)?.Value;
                var chatMap = _db.chatEntities.Where(x => x.senderEmail == userEmail || x.receiverEmail == userEmail).Select(x => x).OrderByDescending(x => x.lastUpdated).ToList();
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
