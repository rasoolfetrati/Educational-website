using LearningWebSite.Core.Services.CommentService;
using LearningWebSite.Core.Services.CourseService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LearningWebSite.Areas.Admin.Controllers
{
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class RemoveController : ControllerBase
    {
        private readonly ICourseService _courseService;
        private ICommentServices _commentServices;

        public RemoveController(ICourseService courseService, ICommentServices commentServices)
        {
            _courseService = courseService;
            _commentServices = commentServices;
        }

        [Route("DeleteEpisode/{episodId}")]
        [HttpPost]
        public async Task<IActionResult> DeleteEpisode(int episodId)
        {
            var episode = _courseService.IsEpisodeExist(episodId);
            if (!episode)
            {
                return BadRequest($"اپیزود مورد نظر یافت نشد!");
            }
            await _courseService.DeleteEpisode(episodId);
            return Ok($"اپیزود {episodId} با موفقیت حذف شد!!!");
        }

        [Route("DeleteCourse/{courseId}")]
        [HttpPost]
        public async Task<IActionResult> Deletecourse(int courseId)
        {
            var course = _courseService.IsCourseExist(courseId);
            if (!course)
            {
                return BadRequest($"دوره مورد نظر یافت نشد!");
            }
            _courseService.DeleteCourse(courseId);
            return Ok($"دوره {courseId} با موفقیت حذف شد!!!");
        }

        [Route("ConfirmComment/{id}")]
        [HttpPost]
        public IActionResult ConfirmComment(int id)
        {
            _commentServices.ConfirmComment(id);
            return Ok("کامنت با موفقیت تایید شد!");
        }
    }
}
