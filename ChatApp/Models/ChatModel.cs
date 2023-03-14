using System.ComponentModel.DataAnnotations;

namespace ChatApp.Models
{
    public class ChatModel
    {
        [Key]
        public Guid chatId { get; set; } = Guid.NewGuid();
        public string senderEmail { get; set; }
        public string receiverEmail { get; set; }
        public DateTime dateTime { get; set; } = DateTime.Now;
        public bool isDeleted { get; set; } = false;
        public DateTime lastUpdated { get; set; } = DateTime.Now;
    }
}
