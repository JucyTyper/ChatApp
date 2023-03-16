using ChatApp.Data;
using ChatApp.Models;

namespace ChatApp.Services
{
    public class FileService:IFileService
    {
        ResponseModel response = new ResponseModel();
        private readonly ChatAppDatabase _db;
        private readonly IConfiguration configuration;

        public FileService(ChatAppDatabase _db, IConfiguration configuration)
        {
            this._db = _db;
            this.configuration = configuration;
        }
        public object UploadImage(string Email,fileUpload imageFile)
        {
            try
            {
                string ImageName = Email + " " + DateTime.Now.Hour + "-" + DateTime.Now.Minute + " " + DateTime.Now.Day + "-" + DateTime.Now.Month + "-" + DateTime.Now.Year + imageFile.file.FileName;
                string path = "C:\\Users\\ChicMic\\source\\repos\\ChatApp\\ChatApp\\Assets\\";
                var filestream = System.IO.File.Create(path + ImageName);
                imageFile.file.CopyTo(filestream);
                filestream.Flush();
                var _user = _db.users.Where(x => (x.Email == Email)).Select(x => x);
                if (_user.Count() == 0)
                {
                    response.StatusCode = 400;
                    response.Message = "User Not Found";
                    return response;
                }
                _user.First().ProfileImagePath = path + ImageName;
                response.Message = ImageName + "image uploaded successfully";
                return response;
            }
            catch(Exception ex)
            {
                response.StatusCode = 500;
                response.Message = ex.Message;
                return response;
            }
        }
        public object UploadFile(string Email, fileUpload rawFile)
        {
            try
            {
                string fileName = Email + " " + DateTime.Now.Hour + "-" + DateTime.Now.Minute + " " + DateTime.Now.Day + "-" + DateTime.Now.Month + "-" + DateTime.Now.Year + rawFile.file.FileName;
                string path = "C:\\Users\\Ajay\\Source\\Repos\\JucyTyper\\ChatApp\\ChatApp\\Assets\\Files\\";
                var filestream = System.IO.File.Create(path + fileName);
                rawFile.file.CopyTo(filestream);
                filestream.Flush();
                var _user = _db.users.Where(x => (x.Email == Email)).Select(x => x);
                if (_user.Count() == 0)
                {
                    response.StatusCode = 400;
                    response.Message = "User Not Found";
                    return response;
                }
                _user.First().ProfileImagePath = path + fileName;
                response.Message = fileName + "image uploaded successfully";
                return response;
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.Message = ex.Message;
                return response;
            }
        }

    }
}
