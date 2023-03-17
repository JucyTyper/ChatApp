﻿using ChatApp.Data;
using ChatApp.Models;
using Microsoft.AspNetCore.Identity;

namespace ChatApp.Services
{
    public class FileService:IFileService
    {
        ResponseModel response = new ResponseModel();
        ResponseModel2 response2 = new ResponseModel2();
        private readonly ChatAppDatabase _db;

        public FileService(ChatAppDatabase _db)
        {
            this._db = _db;

        }
        public object UploadImage(string Email,fileUpload imageFile)
        {
            try
            {
                string ImageName = Email + " " + DateTime.Now.Hour + "-" + DateTime.Now.Minute + " " + DateTime.Now.Day + "-" + DateTime.Now.Month + "-" + DateTime.Now.Year + imageFile.file.FileName;
                var folderName = "Assets//Images//";
                var path = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                var fullpath = path + "//" + ImageName;
                //string path = "C:\\Users\\ChicMic\\source\\repos\\ChatApp\\ChatApp\\Assets\\";
                var filestream = System.IO.File.Create(fullpath);
                imageFile.file.CopyTo(filestream);
                filestream.Close();
                var _user = _db.users.Where(x => (x.Email == Email)).Select(x => x);
                if (_user.Count() == 0)
                {
                    response.StatusCode = 400;
                    response.Message = "User Not Found";
                    return response;
                }
                _user.First().ProfileImagePath = fullpath;
                response2.Message = "image successfullu uploaded" ;
                response2.IsSuccess = true;
                return response2;
            }
            catch(Exception ex)
            {
                response2.StatusCode = 500;
                response2.Message = ex.Message;
                response2.IsSuccess = false;
                return response2;
            }
        }
        public object UploadFile(int type, string Email, fileUpload rawFile)
        {
            try
            {
                string folderName;
                string path;
                string fileName = Email + " " + DateTime.Now.Hour + "-" + DateTime.Now.Minute + " " + DateTime.Now.Day + "-" + DateTime.Now.Month + "-" + DateTime.Now.Year + rawFile.file.FileName;
                if(type== 2)
                {
                    folderName = "Assets//Images//"; //Path.Combine("Assets", "Images");
                    path = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                }
                else
                {
                    folderName = "Assets//Files//";
                    path = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                }
                var fullpath = path + "//" + fileName; //Path.Combine(path, fileName);
                var filestream = System.IO.File.Create(fullpath);
                rawFile.file.CopyTo(filestream);
                filestream.Close();
                response.Message = fileName + "image uploaded successfully";
                response.Data = folderName  + fileName;// Path.Combine(folderName, fileName);
                return response;
            }
            catch (Exception ex)
            {
                response2.StatusCode = 500;
                response2.Message = ex.Message;
                response2.IsSuccess = false;
                return response2;
            }
        }

    }
}
