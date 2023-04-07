using BlazorFileUpload.Server.Data;
using BlazorFileUpload.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Net;

namespace BlazorFileUpload.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;
        private readonly DataContext _context;

        public FileController(IWebHostEnvironment env, DataContext context)
        {
            _env = env;
            _context = context;
        }

        [HttpGet("{fileName}")]
        public async Task<IActionResult> DownloadFile(string fileName)
        {
            // Retrieve the database record for the requested file by its filename.
            var uploadResults = await _context.Uploads.FirstOrDefaultAsync(u => u.StoredFileName.Equals(fileName));

            // Create the full path to the file on the server by combining the root directory, "uploadsFiles" subdirectory, and the filename.
            var path = Path.Combine(_env.ContentRootPath, "uploadsFiles", fileName);

            // Create a new memory stream to hold the file data.
            var memory = new MemoryStream();

            // Use a file stream to read the file from disk and copy its contents to the memory stream.
            using (var stream = new FileStream(path, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }

            // Reset the memory stream position to the beginning.
            memory.Position = 0;

            // Return a file to the client with the contents of the memory stream, the file type from the database record, and the file name.
            return File(memory, uploadResults.ContentType, Path.GetFileName(path));
        }


        [HttpPost]
        public async Task<ActionResult<List<UploadResult>>> UploadFile(List<IFormFile> files)
        {
            List<UploadResult> uploadResults = new List<UploadResult>();

            foreach(var file in files)
            {
                var uploadResult = new UploadResult();
                string trustedFileNameForFileStorage;
                var untrustedFileName = file.FileName;
                uploadResult.FileName = untrustedFileName;
                

                trustedFileNameForFileStorage = Path.GetRandomFileName();
                var path = Path.Combine(_env.ContentRootPath, "uploadsFiles", trustedFileNameForFileStorage);

                await using FileStream fs = new(path, FileMode.Create);
                await file.CopyToAsync(fs);

                uploadResult.StoredFileName = trustedFileNameForFileStorage;
                uploadResult.ContentType = file.ContentType;
                uploadResults.Add(uploadResult);

                _context.Uploads.Add(uploadResult);
            }

            await _context.SaveChangesAsync();
            return Ok(uploadResults);
        }
    }
}
