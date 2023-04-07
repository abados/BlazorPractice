using BlazorFileUpload.Client.Pages;
using BlazorFileUpload.Server.Data;
using BlazorFileUpload.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Net;
using System.Xml;

namespace BlazorFileUpload.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;
        private readonly DataContext _context;
        private readonly IWebHostEnvironment _environment;

        public FileController(IWebHostEnvironment env, DataContext context, IWebHostEnvironment environment)
        {
            _env = env;
            _context = context;
            _environment= environment;
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

        [HttpGet]
        public async Task<List<Employee>> GetEmployeesAsync()
        {
            List<Employee> employees = new List<Employee>();
            string baseDirectory = AppContext.BaseDirectory;
            string projectDirectory = baseDirectory;

            for (int i = 0; i < 5; i++) // Go up three directories
            {
                projectDirectory = Directory.GetParent(projectDirectory).FullName;
            }

            string wwwrootPath = Path.Combine(projectDirectory, "Client", "wwwroot");
            string filePath = Path.Combine(wwwrootPath, "Employee.xml");

            XmlDocument doc = new XmlDocument();
            try { 
            doc.Load(filePath);

            foreach (XmlNode node in doc.SelectNodes("/Employees/Employee"))
            {
                employees.Add(new Employee()
                {
                    Id = int.Parse(node["Id"].InnerText),
                    Name = node["Name"].InnerText,
                    Country = node["Country"].InnerText,
                });
            }

            return employees;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
        }

    }
}
