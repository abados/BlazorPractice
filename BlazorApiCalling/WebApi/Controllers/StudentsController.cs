using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class StudentsController : Controller
	{
		[HttpGet]
		public IEnumerable<Student> Get()
		{
			List<Student> Students = new List<Student>();
			for (int i = 0; i < 9; i++)
			{
				Students.Add(new Student()
				{
					StudentId = i,
					Name = "Name"+i.ToString(),
					Roll = "Roll"+i.ToString()
				});
			}
			return Students;
		}
	}
}
