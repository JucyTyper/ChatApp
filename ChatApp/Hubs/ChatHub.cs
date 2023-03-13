using Microsoft.AspNetCore.SignalR;
using SendGrid.Helpers.Mail;

namespace ChatApp.Hubs
{
    public class ChatHub:Hub
    {
        public async Task SendMessage(string msg)
        {
            await Clients.Clients(this.Context.ConnectionId).SendAsync("receiveMessage", msg+" Muskan");
        }
    }
}
