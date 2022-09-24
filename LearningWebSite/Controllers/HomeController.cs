using Ganss.XSS;
using LearningWebSite.Areas.Admin.Controllers;
using LearningWebSite.Core.InfraStructure;
using LearningWebSite.Core.Services.ContactUsService;
using LearningWebSite.Core.Services.CourseService;
using LearningWebSite.DataLayer.Entities.ContactUs;
using Microsoft.AspNetCore.Mvc;

namespace LearningWebSite.Controllers
{
    public class HomeController : MainControllerBase
    {
        private readonly ICourseService _courseService;
        private readonly IContactUsService _contactUsService;
        public HomeController(ICourseService courseService, IContactUsService contactUsService)
        {
            _courseService = courseService;
            _contactUsService = contactUsService;
        }

        public IActionResult Index()
        {
            var data = _courseService.GetCoursesForIndex();
            return View(data);
        }
        [HttpGet]
        [Route("تماس_با_ما")]
        public IActionResult ContactUs()
        {
            return View();
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> ContactUs(Contacts contacts)
        {
            if (!ModelState.IsValid)
            {
                return View(contacts);
            }
            HtmlSanitizer htmlSanitizer = new();
            contacts.Message = htmlSanitizer.Sanitize(contacts.Message);
            await _contactUsService.SaveMessage(contacts);
            return RedirectAndShowAlert(OperationResult.Success("پیام شما با موفقیت ثبت شد!"), RedirectToAction("ContactUs"));
        }
        [Route("NotFound")]
        public IActionResult NotFound()
        {
            return PartialView("NotFound");
        }

    }
}