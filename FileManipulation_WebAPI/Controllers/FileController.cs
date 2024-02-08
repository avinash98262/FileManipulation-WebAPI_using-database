using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Models;
using Repository;

namespace FileManipulation_WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        

        private readonly IFileRepository _fileRepository;

        public FileController(IFileRepository fileRepository)
        {
            _fileRepository = fileRepository;
        }

        [HttpGet("")]
        public string Get()
        {
            return "Welcome to the File Manipulation Project";
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllFiles()
        {
            var files = await _fileRepository.GetAllFiles();
            return Ok(files);
        }

        [HttpPost("")]
        public async Task<IActionResult> Upload([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            var fileName = Path.GetFileName(file.FileName);
            var filePath = _fileRepository.GetUniqueFilePath(fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var fileSize = new FileInfo(filePath).Length;
            var fileTitle = Path.GetFileNameWithoutExtension(fileName); // Example: Remove the extension to use the file name as the title

            var uploadedFile = await _fileRepository.UploadFile(fileTitle, fileName, filePath, fileSize);

            return Ok(uploadedFile);
        }

        [HttpGet("{filename}")]
        public async Task<IActionResult> DownloadFile([FromRoute] string filename)
        {
            try
            {
                var file = await _fileRepository.GetFileByName(filename);
                if (file == null)
                {
                    return NotFound($"File '{filename}' not found.");
                }

                var provider = new FileExtensionContentTypeProvider();
                if (!provider.TryGetContentType(file.FileName, out var contentType))
                {
                    contentType = "application/octet-stream";
                }

                var fileBytes = await System.IO.File.ReadAllBytesAsync(file.FilePath);
                return File(fileBytes, contentType, file.FileName);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while downloading the file.");
            }
        }

        [HttpDelete("{fileName}")]
        public async Task<IActionResult> DeleteFile([FromRoute] string fileName)
        {
            var result = await _fileRepository.DeleteFile(fileName);

            if (result)
            {
                return Ok($"File '{fileName}' deleted successfully.");
            }

            return NotFound($"File '{fileName}' not found.");
        }


    }
}
