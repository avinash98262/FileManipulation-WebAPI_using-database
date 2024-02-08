using Data;
using Microsoft.EntityFrameworkCore;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class FileRepository :IFileRepository
    {

        private readonly FileContext _context;

        public FileRepository(FileContext context)
        {
            _context = context;
        }

        public async Task<List<FileModel>> GetAllFiles()
        {
            return await _context.Files.Where(f => !f.IsDeleted).ToListAsync();
        }

        public async Task<FileModel> GetFileByName(string fileName)
        {
            return await _context.Files.FirstOrDefaultAsync(f => f.FileName == fileName && !f.IsDeleted);
        }


        public async Task<FileModel> UploadFile(string fileTitle, string fileName, string filePath, long fileSize)
        {
            try
            {
                // Create a new FileModel object with the provided parameters
                var fileModel = new FileModel
                {
                    FileTitle = fileTitle,
                    FileName = fileName,
                    FilePath = filePath,
                    FileSize = fileSize,
                    UploadDateTime = DateTime.Now
                };

                // Add the FileModel object to the context
                _context.Files.Add(fileModel);

                // Save changes to the database
                await _context.SaveChangesAsync();

                // Return the uploaded FileModel object
                return fileModel;
            }
            catch (Exception ex)
            {
                // Handle any exceptions, such as database errors
                // You can log the exception or throw a custom exception here
                throw new Exception("An error occurred while uploading the file.", ex);
            }
        }

        public string GetUniqueFilePath(string fileName)
        {
            var directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "Upload", "Files");
            var filePath = Path.Combine(directoryPath, fileName);

            // Check if the file already exists
            if (System.IO.File.Exists(filePath))
            {
                // Get the file extension
                var extension = Path.GetExtension(fileName);

                // Remove the extension from the file name
                var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);

                // Attempt to find a unique file name by appending numbers
                var uniqueFileName = fileNameWithoutExtension;
                var count = 1;
                while (System.IO.File.Exists(filePath))
                {
                    uniqueFileName = $"{fileNameWithoutExtension}_{count}{extension}";
                    filePath = Path.Combine(directoryPath, uniqueFileName);
                    count++;
                }

                // Return the unique file path
                return filePath;
            }

            // If the file does not exist, return the original file path
            return filePath;
        }

        public async Task<bool> DeleteFile(string fileName)
        {
            var fileModel = await _context.Files.FirstOrDefaultAsync(f => f.FileName == fileName && !f.IsDeleted);
            if (fileModel == null)
            {
                return false; // File not found or already deleted
            }

            fileModel.IsDeleted = true;
            fileModel.DeletionDateTime = DateTime.Now;

            await _context.SaveChangesAsync();

            return true;
        }

    }
}
