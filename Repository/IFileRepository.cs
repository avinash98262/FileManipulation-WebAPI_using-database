using Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public interface IFileRepository
    {
        public  Task<List<FileModel>> GetAllFiles();
        public  Task<FileModel> GetFileByName(string fileName);
      
        public Task<FileModel> UploadFile(string fileTitle, string fileName, string filePath, long fileSize);
        public Task<bool> DeleteFile(string fileName);

        public string GetUniqueFilePath(string fileName);



    }
}
