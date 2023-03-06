using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace ChatApp.Models
{
    public class AddUser
    {
        [Required]
        [StringLength(50,MinimumLength = 2)]
        public string FirstName { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string LastName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        //[Required]
        //[RegularExpression("^(?=.*\\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[^\\da-zA-Z]).{8,16}$", ErrorMessage = "Passwords must be at least 8 characters and upper case (A-Z), lower case (a-z), number (0-9) and special character (e.g. !@#$%^&*)")]
        public string Password { get; set; }
        //[Required]
        //[Range(1000000000, 9999999999)]
        public long PhoneNo { get; set; }
        [Required]
        public DateTime DateOfBirth { get; set; }
    }
}
