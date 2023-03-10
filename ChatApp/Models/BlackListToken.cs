using System.ComponentModel.DataAnnotations;

namespace ChatApp.Models
{
    public class BlackListToken
    {
        [Key]
        public Guid tokenId { get; set; } = Guid.NewGuid();
        public string token { get; set; }
    }
}
