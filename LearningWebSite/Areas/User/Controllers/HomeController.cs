using LearningWebSite.Core.InfraStructure;
using LearningWebSite.Core.Services;
using LearningWebSite.Core.Services.BasketService;
using LearningWebSite.Core.Services.CourseService;
using LearningWebSite.Core.Services.WalletService;
using LearningWebSite.Core.ViewModel.Users;
using LearningWebSite.DataLayer.Context;
using LearningWebSite.DataLayer.Entities.Courses;
using LearningWebSite.DataLayer.Entities.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LearningWebSite.Areas.User.Controllers
{
    [Area("User")]
    [Authorize]
    [AutoValidateAntiforgeryToken]
    public class HomeController : UserControllerBase
    {
        private readonly SignInManager<CustomUser> _signInManager;
        private readonly IUserService _userService;
        private readonly UserManager<CustomUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IFactorService _walletService;
        private readonly IBasketService basketService;
        private readonly ICourseService _courseService;

        public HomeController(SignInManager<CustomUser> signInManager, IUserService userService,
            UserManager<CustomUser> userManager, ApplicationDbContext context
            , IFactorService walletService, IBasketService basketService, ICourseService courseService) : base(signInManager)
        {
            _signInManager = signInManager;
            _userService = userService;
            _userManager = userManager;
            _context = context;
            _walletService = walletService;
            this.basketService = basketService;
            _courseService = courseService;
        }

        public IActionResult Index()
        {
            ViewBag.Factors = _walletService.GetFactors(User.Identity.Name);
            ViewBag.GetUserCourses = _userService.GetUserCourses(User.Identity.Name);
            ViewBag.WalletBalance = _userService.WalletBalance(User.Identity.Name);
            var data = _userService.GetSideBarView(User.Identity.Name);
            ViewBag.Basket = basketService.GetUserBaskets(User.Identity.Name);
            ViewBag.BasketCount = basketService.GetUserBasketsCount(User.Identity.Name);
            ViewBag.UserCourses = _userService.getUserCourses(User.Identity.Name);
            ViewBag.TeacherCourses = _courseService.GetTeacherCourses(User.Identity.Name);
            return View(data);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditInfo(UserViewModel userViewModel, IFormFile image)
        {
            if (!string.IsNullOrWhiteSpace(userViewModel.Password) && userViewModel.Password.Equals(userViewModel.RePassword))
            {
                bool changepassword = await _userService.ResetPasswordUserAsync(userViewModel, User.Identity.Name);
                if (changepassword)
                {
                    return await RedirectAndShowAlert(OperationResult.Success("کلمه عبور شما با موفقیت تغییر کرد! لطفا مجددا وارد سایت شوید"), RedirectToAction(nameof(Index)), true);
                }
            }
            else
            {
                await _userService.updateUserAsync(User.Identity.Name, userViewModel, image);
            }
            return await RedirectAndShowAlert(OperationResult.Success("اطلاعات شمت با موفقیت بروز شد!"), RedirectToAction(nameof(Index)), false);
        }
        [HttpGet]
        public IActionResult IndexEpisode(int courseId)
        {
            ViewData["CourseId"] = courseId;
            var data = _courseService.GetAllCourseEpisodes(courseId);
            return View(data);
        }
        [HttpGet]
        public IActionResult CreateEpisode(int courseId)
        {
            CourseEpisode courseEpisode = new() { CourseId = courseId };
            return View(courseEpisode);
        }

        [HttpPost]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> CreateEpisode(CourseEpisode courseEpisode, IFormFile fileEpisode)
        {

            if (fileEpisode == null)
            {
                ViewData["IsNullFile"] = true;
                return View(courseEpisode);
            }
            var getExtension = Path.GetExtension(fileEpisode.FileName);
            if (getExtension != ".zip")
            {
                ModelState.AddModelError("", "پسوند فایل حتما باید zip باشد.");
                return View(courseEpisode);
            }
            bool isExist = _courseService.CheckExistFile(fileEpisode.FileName);
            if (isExist)
            {
                ViewData["IsExistFile"] = true;
                return View(courseEpisode);
            }
            else
            {
                await _courseService.CreateEpisode(courseEpisode, fileEpisode);
                return RedirectAndShowAlert(
                    OperationResult.Success(),
                    RedirectToAction(
                        nameof(IndexEpisode),
                        new { courseId = courseEpisode.CourseId }
                    )
                );
            }
        }

        [HttpGet]
        public async Task<IActionResult> EditEpisode(int episodId)
        {
            //ViewBag.episodeID = episodId;
            var data = await _courseService.GetCourseEpisodeById(episodId);
            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> EditEpisode(CourseEpisode courseEpisode, IFormFile fileEpisode)
        {
            // ViewBag.episodeID = courseEpisode.EpisodeId;
            if (fileEpisode == null && courseEpisode.EpisodeId <= 0)
            {
                ViewData["IsNullFile"] = true;
                var getExtension = Path.GetExtension(fileEpisode.FileName);
                if (getExtension != ".zip")
                {
                    ModelState.AddModelError("", "پسوند فایل حتما باید zip باشد.");
                }
                return View(courseEpisode);
            }
            if (fileEpisode != null)
            {
                bool isExist = _courseService.CheckExistFile(fileEpisode.FileName);
                if (isExist)
                {
                    ViewData["IsExistFile"] = true;
                    return View(courseEpisode);
                }
            }
            await _courseService.UpdateEpisode(courseEpisode, fileEpisode);
            return RedirectAndShowAlert(
                OperationResult.Success(),
                RedirectToAction(nameof(IndexEpisode), new { courseId = courseEpisode.CourseId })
            );
        }
    }
}
