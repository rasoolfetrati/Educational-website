﻿using LearningWebSite.Core.InfraStructure;
using LearningWebSite.Core.Services.CommentService;
using LearningWebSite.Core.Services.CourseService;
using LearningWebSite.Core.ViewModel.CourseVM;
using LearningWebSite.DataLayer.Entities.Courses;
using LearningWebSite.DataLayer.Entities.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LearningWebSite.Areas.Admin.Controllers;

[Area("Admin")]
[AutoValidateAntiforgeryToken]
public class CourseController : AdminControllerBase
{
    private ICourseService _courseService;
    private readonly UserManager<CustomUser> _userManager;
    private ICommentServices _commentServices;
    // private readonly ITelegramBotClient _botClient;
    public CourseController(
        ICourseService courseService,
        UserManager<CustomUser> userManager,
        ICommentServices commentServices
    // ITelegramBotClient botClient
    )
    {
        _courseService = courseService;
        _userManager = userManager;
        _commentServices = commentServices;
        //_botClient = botClient;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="currentPageIndex">This is a for page index</param>
    /// <param name="courseName">this is for (courses filter)</param>
    /// <returns>view with data</returns>
    [HttpGet]
    public IActionResult Index(string courseName, int currentPageIndex = 1)
    {
        var data = _courseService.GetAllCourseForAdmin(courseName, currentPageIndex);

        return View(data);
    }
    [HttpGet]
    public IActionResult CreateCourse()
    {
        var groups = _courseService.GetCourseGroups();
        ViewBag.Groups = new SelectList(groups, "Value", "Text");

        var subGroups = _courseService.GetCourseSubGroups(int.Parse(groups.First().Value));
        ViewBag.subGroups = new SelectList(subGroups, "Value", "Text");

        var teachers = _courseService.GetTeachers();
        if (teachers.Count > 0)
        {
            ViewBag.Teachers = new SelectList(teachers, "Value", "Text", teachers.First());
        }
        return View();
    }

    [HttpPost]
    [DisableRequestSizeLimit]
    public async Task<IActionResult> CreateCourse(CourseViewModel courseViewModel)
    {
        var url = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}";
        await _courseService.CreateCourse(courseViewModel);
        //Message message = await _botClient.SendPhotoAsync(
        //chatId: "@testRasool79",
        //photo: $"{url}/{courseViewModel.CourseImageName}",
        //caption: $"{courseViewModel.CourseTitle}");
        return RedirectAndShowAlert(OperationResult.Success("دوره با موفقیت افزوده شد!"), RedirectToAction(nameof(Index)));
    }

    [HttpGet]
    public async Task<IActionResult> EditCourse(int courseId)
    {
        var data = await _courseService.GetCourseById(courseId);
        var user = await _userManager.FindByIdAsync(data.TeacherId);
        var fullname = user.FirstName + " " + user.LastName;
        var groups = _courseService.GetCourseGroups();
        ViewBag.Groups = new SelectList(groups, "Value", "Text", data.GroupId);

        var subGroups = _courseService.GetCourseSubGroups(data.GroupId);
        ViewBag.subGroups = new SelectList(subGroups, "Value", "Text", data.SubGroup);

        var teachers = _courseService.GetTeachers();
        ViewBag.Teachers = new SelectList(
            teachers,
            "Value",
            "Text",
            teachers.First(t => t.Text == fullname)
        );
        return View(data);
    }

    [HttpPost]
    [DisableRequestSizeLimit]
    public async Task<IActionResult> EditCourse(Course courseViewModel, IFormFile imgCourseUp, IFormFile demoUp)
    {
        _courseService.UpdateCourse(courseViewModel, imgCourseUp, demoUp);
        return RedirectAndShowAlert(OperationResult.Success("با موفقیت ویرایش شد!"), RedirectToAction(nameof(Index)));
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

        if (fileEpisode != null)
        {
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
            await _courseService.CreateEpisode(courseEpisode, fileEpisode);
            return RedirectAndShowAlert(
             OperationResult.Success("ساخت اپیزود با موفقیت انجام شد."),
             RedirectToAction(
                 nameof(IndexEpisode),
                 new { courseId = courseEpisode.CourseId }));
        }
        else
        {
            // var url = $"{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}";

            await _courseService.CreateEpisode(courseEpisode, fileEpisode);

            //var courseImageName = _courseService.GetCourseById(courseEpisode.CourseId).Result.CourseImageName;

            //اطلاع دادن به کاربر ها در صورت ارسال اپیزود جدید
            //Message message = await _botClient.SendPhotoAsync(
            //chatId: "@testRasool79",
            //photo: $"{url}/CourseImage/{courseImageName}",
            //caption: $"{courseEpisode.EpisodeTitle}");

            return RedirectAndShowAlert(
                OperationResult.Success("ساخت اپیزود با موفقیت انجام شد."),
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
        var data = await _courseService.GetCourseEpisodeById(episodId);
        return View(data);
    }

    [HttpPost]
    public async Task<IActionResult> EditEpisode(CourseEpisode courseEpisode, IFormFile fileEpisode)
    {
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
    [HttpPost]
    public async Task<IActionResult> AddSource(int courseId, IFormFile project)
    {
        if (project != null)
        {
            await _courseService.AddSource(courseId, project);
            return RedirectAndShowAlert(OperationResult.Success("سورس دوره با موفقیت افزوده شد!"), RedirectToAction("IndexEpisode", "Course", new { courseId = courseId }));
        }
        return RedirectAndShowAlert(OperationResult.Error("لطفا فایل خود را انتخاب کنید!"), RedirectToAction("IndexEpisode", "Course", new { courseId = courseId }));
    }

    [Route("GetSubGroups/{id}")]
    public IActionResult GetSubGroups(int id)
    {
        List<SelectListItem> list = new List<SelectListItem>()
        {
            new SelectListItem() { Text = "انتخاب کنید", Value = "" }
        };
        list.AddRange(_courseService.GetCourseSubGroups(id));
        return Json(new SelectList(list, "Value", "Text"));
    }

    [Route("DeletedCourses")]
    [HttpGet]
    public IActionResult DeletedCourses()
    {
        return View(_courseService.GetDeleteCourses());
    }

    [Route("CommentsList")]
    [HttpGet]
    public IActionResult GetComments()
    {
        var data = _commentServices.GetComments();
        return View(data);
    }
}
