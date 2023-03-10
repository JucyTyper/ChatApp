namespace ChatApp.Models
{
    public class UserResponse
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public Guid UserID { get; set; }
        public string Token { get; set; }
    }
}
