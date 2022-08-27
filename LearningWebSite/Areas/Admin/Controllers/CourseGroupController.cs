using LearningWebSite.Core.InfraStructure;
using LearningWebSite.Core.Services.CourseService;
using LearningWebSite.DataLayer.Entities.Courses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LearningWebSite.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AutoValidateAntiforgeryToken]
    // [Authorize]
    public class CourseGroupController : AdminControllerBase
    {
        private readonly ICourseService _courseService;
        public CourseGroupController(ICourseService courseService)
        {
            _courseService = courseService;
        }

        public async Task<IActionResult> Index()
        {
            var data = await _courseService.GetAllGroups();
            return View(data);
        }
        [HttpGet]
        public IActionResult CreateGroup(int? parentId)
        {
            CourseGroups courseGroups = new CourseGroups()
            {
                ParentId = parentId
            };
            return View(courseGroups);
        }
        [HttpPost]
        public async Task<IActionResult> CreateGroup(CourseGroups courseGroups)
        {
            await _courseService.CreateGroup(courseGroups);
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public IActionResult EditGroup(int groupId)
        {
            var result = _courseService.GetGroup(groupId);
            return View(result);
        }
        [HttpPost]
        public IActionResult EditGroup(CourseGroups courseGroups)
        {
            _courseService.UpdateGroup(courseGroups);
            return RedirectAndShowAlert(OperationResult.Success(), RedirectToAction(nameof(Index)));
        }
        [HttpPost]
        public IActionResult DeleteGroup(int groupId)
        {
            _courseService.DeleteGroup(groupId);
            return RedirectAndShowAlert(OperationResult.Success(), RedirectToAction(nameof(Index)));
        }

    }
}
