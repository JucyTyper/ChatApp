using System.ComponentModel.DataAnnotations;

namespace ChatApp.Models
{
    public class UpdateUser
    {
        [StringLength(50, MinimumLength = 2)]
        public string FirstName { get; set; }
        [StringLength(50, MinimumLength = 2)]
        public string LastName { get; set; }

        //[Required]
        //[Range(1000000000, 9999999999)]
        public long PhoneNo { get; set; }
        public DateTime DateOfBirth { get; set; }
        public DateTime Updated { get; set; } = DateTime.Now;

    }
}
