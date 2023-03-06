using System.ComponentModel.DataAnnotations;
using System.Data;

namespace ChatApp.Models
{
    public class UserModel
    {
        [Key]
        public Guid UserId { get; set; } = Guid.NewGuid();
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public byte[] Password { get; set; }
        public long PhoneNo { get; set; }
        public DateTime DateOfBirth { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
        public DateTime Updated { get; set; } = DateTime.Now;
        public DateTime LastActive { get; set; } = DateTime.Now;
        public bool IsDeleted { get; set; } = false;
        public string ProfileImagePath { get; set; } = string.Empty;
    }
}
