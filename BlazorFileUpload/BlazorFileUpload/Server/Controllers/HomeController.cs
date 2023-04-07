using BlazorFileUpload.Shared;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Xml;

namespace BlazorFileUpload.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HomeController : ControllerBase
    {
        private readonly IWebHostEnvironment _environment;

        public HomeController(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        [HttpGet]
        public IEnumerable<Employee> GetEmployees()
        {
            List<Employee> employees = new List<Employee>();

            XmlDocument doc = new XmlDocument();
            doc.Load(string.Concat(_environment.WebRootPath, "/Employee.xml"));

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
    }
}
