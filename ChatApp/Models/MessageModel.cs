namespace ChatApp.Models
{
    public class MessageModel
    {
        public Guid messageId { get; set; } = Guid.NewGuid();
        public string message { get; set; } = string.Empty;
        public Guid senderId { get; set; }
        public Guid receiverId { get; set; }
        public DateTime dateTime { get; set; } = DateTime.Now;
        public string filePath { get; set; } = string.Empty;
        public bool isDeleted { get; set; } = false;
    }
}
