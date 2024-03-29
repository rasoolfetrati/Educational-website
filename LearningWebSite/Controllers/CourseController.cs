﻿#region Usings
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
using System.IO.Compression;
using System.Net;
#endregion

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

        [HttpGet("course/{courseId}/{Slug}")]
        public IActionResult Index(int courseId, string Slug)
        {
            if (courseService.IsCourseExist(courseId))
            {
                var course = courseService.showCourseViewModel(courseId, Slug);
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
            List<int> Categories = null,
            int take = 0
        )
        {
            ViewBag.selectedGroups = Categories;
            ViewBag.selectedtype = getType;
            ViewBag.sort = sort;
            ViewBag.Groups = courseService.GetAllGroups();
            ViewBag.pageId = pageId;
            return View(courseService.GetCourse(pageId, filter, getType, sort, Categories, take));
        }

        [Authorize]
        public async Task<IActionResult> OrderView()
        {
            var data = await basketService.GetCourses(User.Identity.Name);
            if (data.Count <= 0)
            {
                ViewBag.Error = "در سبد خرید شما محصولی وجود ندارد!";
                return View();
            }
            ViewBag.orderId = basketService.GetOrderId(User.Identity.Name);
            ViewBag.UserWallet = userService.WalletBalance(User.Identity.Name);
            ViewBag.Total = basketService.GetTotalPriceUserBasket(User.Identity.Name);
            return View(data);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Payment(List<int> courseId, string paymethod, int sumOrder)
        {
            if (string.IsNullOrWhiteSpace(paymethod))
            {
                return RedirectAndShowAlert(
                        OperationResult.Error("لطفا روش پرداخت را انتخاب نمایید!"),
                        RedirectToAction(nameof(OrderView))
                    );
            }
            var url = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}";
            int orderId = basketService.GetOrderId(User.Identity.Name);
            if (paymethod.Equals("wallet"))
            {
                int walletBalance = userService.WalletBalance(User.Identity.Name);
                if (walletBalance < sumOrder)
                {
                    return RedirectAndShowAlert(
                        OperationResult.Error("موجودی کیف پول شما کمتر از مبلغ سفارش می باشد!"),
                        RedirectToAction(nameof(OrderView))
                    );
                }
                basketService.PayWithWallet(User.Identity.Name, orderId);
                return RedirectAndShowAlert(OperationResult.Success("خرید شما با موفقیت انجام شد!"), Redirect($"{url}/User"));
            }
            else
            {
                var payment = new ZarinpalSandbox.Payment(sumOrder);
                var response = await payment.PaymentRequest(
                    "خرید دوره",
                    $"{url}/OnlineCoursePayment/{orderId}/{sumOrder}",
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

        [Route("OnlineCoursePayment/{orderId}/{sumOrder}")]
        [Authorize]
        public async Task<IActionResult> OnlineCoursePayment(int orderId, int sumOrder)
        {
            if (
                HttpContext.Request.Query["Status"] != ""
                && HttpContext.Request.Query["Status"].ToString().ToLower().Trim() == "ok"
                && HttpContext.Request.Query["Authority"] != ""
            )
            {
                string authority = HttpContext.Request.Query["Authority"];
                var result = basketService.orderSum(orderId, User.Identity.Name);
                if (result == sumOrder)
                {

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
                        RedirectToAction("Index", "Home", new { area = "User" }));
                    }
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
                    RedirectToAction("index", new { episode.CourseId, episode.Course.Slug })
                );
            }
            if (!string.IsNullOrWhiteSpace(episode.FileUrl))
            {
                var result = DownloadExtention.GetUrlContent(episode.FileUrl);
                if (result != null)
                {
                    return File(result.Result, "application/force-download", episode.EpisodeTitle + "" + Path.GetExtension(episode.FileUrl));
                }
                return Ok("file is not exist");
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
              RedirectToAction("index", new { episode.CourseId, episode.Course.Slug })
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
                RedirectToAction("index", new { episode.CourseId, episode.Course.Slug })
            );
        }
        [Route("PlayOnline/{episodeId}")]
        [HttpGet]
        public IActionResult PlayOnline(int episodeId)
        {
            var episode = courseService.GetEpisodeById(episodeId);
            if (episode == null)
            {
                return new JsonResult(new { status = 500, message = "مشکلی پیش آمده است!" });
            }
            if (episode.EpisodeTime == TimeSpan.Zero)
            {
                return new JsonResult(new { status = 400, message = "سورس دوره غیر قابل پخش است!" });
            }
            if (!string.IsNullOrWhiteSpace(episode.FileUrl))
            {
                return new JsonResult(new { status = 200, value = episode.FileUrl });
            }
            if (!string.IsNullOrWhiteSpace(episode.EpisodeFileName))
            {
                string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/course/Episode", episode.EpisodeFileName);
                var extractPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/course/ExtractedEpisodes");
                if (!System.IO.File.Exists(filePath))
                {
                    return new JsonResult(new { status = 500, message = "مشکلی پیش آمده است!" });
                }
                if (!episode.Login && episode.IsFree)
                {
                    var res = ExtractFile(filePath, extractPath, episode.EpisodeFileName);
                    return new JsonResult(new { status = 200, value = res.Value });
                }
                if (episode.IsFree && User.Identity.IsAuthenticated && episode.Login)
                {
                    var res = ExtractFile(filePath, extractPath, episode.EpisodeFileName);
                    return new JsonResult(new { status = 200, value = res.Value });
                }
                if (!episode.IsFree && User.Identity.IsAuthenticated && userService.IsUserInCourse(episode.CourseId, User.Identity.Name))
                {
                    var res = ExtractFile(filePath, extractPath, episode.EpisodeFileName);
                    return new JsonResult(new { status = 200, value = res.Value });
                }
            }
            return new JsonResult(new { status = HttpStatusCode.Unauthorized, message = "برای مشاهده ویدیو باید لاگین کنید!" });
        }
        public JsonResult ExtractFile(string filePath, string extractPath, string filename)
        {
            ZipFile.ExtractToDirectory(filePath, extractPath, overwriteFiles: true);
            var targetFilePath = Directory.GetFiles(extractPath, filename.Replace(".zip", ".mp4"), SearchOption.AllDirectories)
            .FirstOrDefault();

            if (targetFilePath != null)
            {
                var finalpath = targetFilePath.Split("wwwroot", StringSplitOptions.RemoveEmptyEntries);
                return new JsonResult(finalpath[1]);
            }

            return null;

        }
    }
    public static class DownloadExtention
    {
        public static async Task<byte[]?> GetUrlContent(string url)
        {
            using (var client = new HttpClient())
            using (var result = await client.GetAsync(url))
                return result.IsSuccessStatusCode ? await result.Content.ReadAsByteArrayAsync() : null;
        }
    }
}
