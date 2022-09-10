using LearningWebSite.Core.Services.CourseService;
using LearningWebSite.DataLayer.Entities.Courses;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace LearningWebSite.Controllers
{
    public class HomeController : Controller
    {
        private readonly ICourseService _courseService;
        public HomeController(ICourseService courseService)
        {
            _courseService = courseService;
        }

        public IActionResult Index()
        {
            var data = _courseService.GetCoursesForIndex();
            return View(data);
        }

        public IActionResult ContactUs()
        {
            return View();
        }
        [Route("NotFound")]
        public IActionResult NotFound()
        {
            return PartialView("NotFound");
        }
        
    }
}