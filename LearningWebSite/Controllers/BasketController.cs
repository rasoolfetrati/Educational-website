using LearningWebSite.Core.Services.BasketService;
using LearningWebSite.Core.Services.CommentService;
using Microsoft.AspNetCore.Mvc;

namespace LearningWebSite.Controllers
{
    [ApiController]
    public class BasketController : ControllerBase
    {
        private readonly IBasketService _basketService;
        private readonly ICommentServices _commentServices;

        public BasketController(IBasketService basketService, ICommentServices commentServices)
        {
            _basketService = basketService;
            _commentServices = commentServices;
        }

        [HttpGet]
        [Route("GetUserBasketCount")]
        public IActionResult GetUserBasketCount()
        {
            var count = _basketService.BasketItemsCount(User.Identity.Name);
            return new JsonResult(count);
        }

        [HttpPost]
        [Route("AddBasket/{courseId}")]
        public async Task<IActionResult> AddBasket(int courseId)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return new JsonResult("جهت افزودن دوره به سبد خرید خود ابتدا وارد سایت شوید!");
            }
            var result = await _basketService.AddCourseToBasket(
                courseId,
                User.Identity.Name.ToString()
            );
            if (result.Status == Core.InfraStructure.OperationResultStatus.Error)
            {
                return new JsonResult(result.Message);
            }
            return Ok(result.Message);
        }

        [HttpPost]
        [Route("GetBasket")]
        public IActionResult GetBasket()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return new JsonResult("جهت افزودن دوره به سبد خرید خود ابتدا وارد سایت شوید!");
            }
            var data = _basketService.GetBasketItems(User.Identity.Name);
            return Ok(data);
        }

        [HttpPost]
        [Route("DeleteComment/{commentId}")]
        public IActionResult DeleteComment(int commentId)
        {
            if (commentId == 0 || commentId == null)
            {
                return NotFound("مشکلی پیش اومد...");
            }
            _commentServices.DeleteComment(commentId);
            return Ok("کامنت شما با موفقیت حذف شد!");
        }
    }
}
