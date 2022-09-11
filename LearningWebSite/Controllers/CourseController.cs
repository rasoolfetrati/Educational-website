using Ganss.XSS;
using LearningWebSite.Areas.Admin.Controllers;
using LearningWebSite.Core.InfraStructure;
using LearningWebSite.Core.Services;
using LearningWebSite.Core.Services.BasketService;
using LearningWebSite.Core.Services.CommentService;
using LearningWebSite.Core.Services.CourseService;
using LearningWebSite.Core.Services.WalletService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LearningWebSite.Controllers
{
    public class CourseController : MainControllerBase
    {
        private readonly ICourseService courseService;
        private readonly IBasketService basketService;
        private readonly IFactorService walletService;
        private readonly IUserService userService;
        private readonly ICommentServices _commentServices;

        public CourseController(
            ICourseService courseService,
            IBasketService basketService,
            IFactorService walletService,
            IUserService userService,
            ICommentServices commentServices
        )
        {
            this.courseService = courseService;
            this.basketService = basketService;
            this.walletService = walletService;
            this.userService = userService;
            _commentServices = commentServices;
        }

        [HttpGet("course/{courseId}/{CourseTitle}")]
        public IActionResult Index(int courseId, string CourseTitle)
        {
            CourseTitle = CourseTitle.Replace(" ", "-");
            if (courseService.IsCourseExist(courseId))
            {
                var course = courseService.showCourseViewModel(courseId, CourseTitle);
                ViewBag.Episode = courseService.GetAllCourseEpisodes(courseId);
                ViewBag.EpisodeCount = courseService.GetEpisodeCount(courseId);
                ViewBag.Fullname = courseService.GetTeacherName(courseId);
                ViewBag.CourseTime = courseService.GetTimeSpan(courseId);
                return View(course);
            }
            return NotFound();
        }
        [HttpGet]
        public async Task<IActionResult> AllCourse(
            int pageId = 1,
            string filter = "",
            string getType = "all",
            string sort = "lates",
            List<int> selectedGroups = null,
            int take = 0
        )
        {
            ViewBag.selectedGroups = selectedGroups;
            ViewBag.selectedtype = getType;
            ViewBag.sort = sort;
            ViewBag.Groups = await courseService.GetAllGroups();
            ViewBag.pageId = pageId;
            return View(courseService.GetCourse(pageId, filter, getType, sort, selectedGroups, 4));
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> OrderView()
        {
           
            var data = await basketService.GetCourses(User.Identity.Name);
            if (data.Count<=0)
            {
                ViewBag.Error = "در سبد خرید شما محصولی وجود ندارد!";
                return View();
            }
            ViewBag.Total = basketService.GetTotalPriceUserBasket(User.Identity.Name);
            ViewBag.UserWallet = userService.WalletBalance(User.Identity.Name);
            return View(data);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Payment(List<int> courseId, string paymethod)
        {
            var url = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}";
            var orderId = await basketService.CreateOrder(courseId, User.Identity.Name);
            var orderSum = basketService.orderSum(orderId, User.Identity.Name);
            if (paymethod.Equals("wallet"))
            {
                int walletBalance = userService.WalletBalance(User.Identity.Name);
                if (walletBalance < orderSum)
                {
                    return RedirectAndShowAlert(
                        OperationResult.Error("موجودی کیف پول شما کمتر از مبلغ سفارش می باشد!"),
                        RedirectToAction(nameof(OrderView), new { courseId = courseId })
                    );
                }
                basketService.PayWithWallet(User.Identity.Name, orderId);
                return RedirectAndShowAlert(OperationResult.Success(), Redirect("/"));
            }
            else
            {
                var payment = new ZarinpalSandbox.Payment(orderSum);
                var response = await payment.PaymentRequest(
                    "خرید دوره",
                    $"{url}/OnlineCoursePayment/" + orderId,
                    User.Identity.Name,
                    "09902036655"
                );
                if (response.Status == 100)
                {
                    return Redirect(
                        "https://sandbox.zarinpal.com/pg/StartPay/" + response.Authority
                    );
                }
            }
            return BadRequest();
        }

        [Route("OnlineCoursePayment/{orderId}")]
        [Authorize]
        public async Task<IActionResult> OnlineCoursePayment(int orderId)
        {
            if (
                HttpContext.Request.Query["Status"] != ""
                && HttpContext.Request.Query["Status"].ToString().ToLower().Trim() == "ok"
                && HttpContext.Request.Query["Authority"] != ""
            )
            {
                string authority = HttpContext.Request.Query["Authority"];
                var result = basketService.orderSum(orderId, User.Identity.Name);
                var payment = new ZarinpalSandbox.Payment(result);
                var response = await payment.Verification(authority);
                if (response.Status == 100)
                {
                    basketService.DeleteUserBasket(User.Identity.Name);
                    string code = response.RefId.ToString();
                    basketService.AddToUserCourse(orderId, User.Identity.Name, code);
                    return RedirectAndShowAlert(OperationResult.Success("خرید شما با موفقیت انجام شد!"), Redirect("/"));
                }
                else
                {
                    return RedirectAndShowAlert(
                        OperationResult.Error("خطایی پیش اومد"),
                        RedirectToAction("Index", "Home", new { area = "User" })
                    );
                }
            }
            else if (
                HttpContext.Request.Query["Status"] != ""
                && HttpContext.Request.Query["Status"].ToString().ToLower().Trim() == "nok"
                && HttpContext.Request.Query["Authority"] != ""
            )
            {
                return RedirectAndShowAlert(
                    OperationResult.Error("شما پرداخت را لغو کردید"),
                    Redirect("/")
                );
            }

            return RedirectAndShowAlert(
                OperationResult.Error("مشکلی پیش اومد..."),
                RedirectToAction("Index", "Home", new { area = "User" })
            );
        }

        [HttpPost]
        [Route("savecomment")]
        public async Task<JsonResult> savecomment(int txtcourseId, string txtComment, int commentId)
        {
            if (string.IsNullOrWhiteSpace(txtComment))
            {
                return Json(new { status = "error", message = "نظر خود را وارد نمایید!" });
            }
            HtmlSanitizer htmlSanitizer = new();
            txtComment = htmlSanitizer.Sanitize(txtComment);
            await _commentServices.AddCommentAsync(
                txtcourseId,
                txtComment,
                commentId,
                User.Identity.Name
            );
            return Json(new { status = "success", message = "نظر شما با موفقیت ثبت شد!" });
        }

        [HttpGet]
        [Route("getcomments/{courseId}")]
        public IActionResult getcomments(int courseId)
        {
            var data = _commentServices.GetCourseComments(courseId);
            return PartialView("_comments", data);
        }

        [Route("downloadFile/{episodeId}")]
        public IActionResult DownloadFile(int episodeId)
        {
            var episode = courseService.GetEpisodeById(episodeId);
            if (episode == null)
            {
                return RedirectAndShowAlert(
                    OperationResult.Error("مشکلی پیش اومد..."),
                    RedirectToAction("index", new { episode.CourseId, episode.Course.CourseTitle })
                );
            }
            string filePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot/course/Episode",
                episode.EpisodeFileName
            );
           
            if (!System.IO.File.Exists(filePath))
            {
                return RedirectAndShowAlert(
              OperationResult.Error("فایل یافت نشد!"),
              RedirectToAction("index", new { episode.CourseId,episode.Course.CourseTitle})
                );
            }
            string fileName = episode.EpisodeFileName;
            if (episode.IsFree && User.Identity.IsAuthenticated)
            {
                byte[] file = System.IO.File.ReadAllBytes(filePath);
                return File(file, "application/force-download", fileName);
            }
            if (User.Identity.IsAuthenticated)
            {
                if (userService.IsUserInCourse(episode.CourseId, User.Identity.Name))
                {
                    byte[] file = System.IO.File.ReadAllBytes(filePath);
                    return File(file, "application/force-download", fileName);
                }
            }
            return RedirectAndShowAlert(
                OperationResult.Error("شما این دوره را نخریده اید!"),
                RedirectToAction("index", new { episode.CourseId, episode.Course.CourseTitle })
            );
        }
    }
}
