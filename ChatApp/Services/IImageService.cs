using ChatApp.Models;

namespace ChatApp.Services
{
    public interface IImageService
    {
        public object UploadImage(string Email, ImageUpload imageFile);
    }
}
