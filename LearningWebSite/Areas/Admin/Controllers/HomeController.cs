using Microsoft.AspNetCore.Mvc;

namespace LearningWebSite.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AutoValidateAntiforgeryToken]
    public class HomeController : AdminControllerBase
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
