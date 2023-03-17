using ChatApp.Models;

namespace ChatApp.Services
{
    public interface IFileService
    {
        public object UploadImage(string Email, fileUpload imageFile);
        public object UploadFile(int type,string Email, fileUpload rawFile);
    }
}
