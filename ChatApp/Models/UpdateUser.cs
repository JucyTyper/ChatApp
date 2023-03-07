using System.ComponentModel.DataAnnotations;

namespace ChatApp.Models
{
    public class UpdateUser
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public long PhoneNo { get; set; } = -1;
        public DateTime DateOfBirth { get; set; }  = DateTime.MinValue;
        public DateTime Updated { get; set; } = DateTime.Now;

    }
}
