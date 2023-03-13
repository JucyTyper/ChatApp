namespace ChatApp.Models
{
    public class ChatModel
    {
        public Guid chatId { get; set; } = Guid.NewGuid();
        public Guid senderId { get; set; }
        public Guid receiverId { get; set; }
        public DateTime dateTime { get; set; } = DateTime.Now;
        public bool isDeleted { get; set; } = false;
    }
}
