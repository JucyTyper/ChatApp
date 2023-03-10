using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace ChatApp.Models
{
    public class AddUser
    {
        //[Required]
        public string FirstName { get; set; } = string.Empty;
        //[Required]
        public string LastName { get; set; } = string.Empty;
        //[Required]
        public string Email { get; set; } = string.Empty;
        //[Required]
        public string Password { get; set; } = string.Empty;
        //[Required]
        public long PhoneNo { get; set; } = 0;
        //[Required]
        public DateTime DateOfBirth { get; set; } = DateTime.MinValue;
    }
}
