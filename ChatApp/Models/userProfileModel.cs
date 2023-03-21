namespace ChatApp.Models
{
    public class userProfileModel
    {
        public Guid UserId { get; set; } 
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public long PhoneNo { get; set; }
        public string DateOfBirth { get; set; }
        public string ProfileImagePath { get; set; } = string.Empty;

    }
}
