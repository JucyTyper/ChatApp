using System.ComponentModel.DataAnnotations;

namespace ChatApp.Models
{
    public class MessageModel
    {
        [Key]
        public Guid messageId { get; set; } = Guid.NewGuid();
        public Guid chatMapId { get; set; }
        public string message { get; set; } = string.Empty;
        public string senderEmail { get; set; }
        public string receiverEmail { get; set; }
        public DateTime dateTime { get; set; } = DateTime.Now;
        public string fileName { get; set; } = string.Empty;
        public bool isDeleted { get; set; } = false;
        public int type { get; set; } = 1;
        public string fileUrl { get; set; } = string.Empty;
    }
}
