namespace ChatApp.Models
{
    public class RoomViewModel
    {
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string email { get; set; }
        public Guid chatRoomId { get; set; }
        public bool isActive { get; set; }= false;
        public string ProfileImagePath { get; set; } = string.Empty;
    }
}