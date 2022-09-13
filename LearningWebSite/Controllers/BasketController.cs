using LearningWebSite.Core.Services.BasketService;
using LearningWebSite.Core.Services.CommentService;
using LearningWebSite.Core.Services.DiscountService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LearningWebSite.Controllers
{
    [ApiController]
    public class BasketController : ControllerBase
    {
        private readonly IBasketService _basketService;
        private readonly IDiscountService _discountService;
        private readonly ICommentServices _commentServices;

        public BasketController(IBasketService basketService, ICommentServices commentServices, IDiscountService discountService)
        {
            _basketService = basketService;
            _commentServices = commentServices;
            _discountService = discountService;
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
        [Route("RemoveBasket/{basketId}")]
        [HttpPost]
        public IActionResult RemoveBasket(int basketId)
        {
            _basketService.DeleteBasket(basketId);
            return Ok($"این ایتم با موفقیت از سبد خرید شما حذف شد!");
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

        [HttpGet]
        [Route("useDiscount")]
        [IgnoreAntiforgeryToken]
        public IActionResult useDiscount(string code, int orderId)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var userId = claim.Value;
            var status = _discountService.UseDiscount(userId, orderId, code);
            if (status == DiscountStatus.Success)
            {
                return new JsonResult(new { code = 200, message = "کد تخفیف شما با موفقیت اعمال شد!" });
            }
            if (status == DiscountStatus.NotFound)
            {
                return new JsonResult(new { code = 404, message = "کد تخفیفی یافت نشد!" });
            }
            if (status == DiscountStatus.Expire)
            {
                return new JsonResult(new { code = 300, message = "مهلت استفاده از کد تخفیف به اتمام رسیده است!" });
            }
            if (status == DiscountStatus.Used)
            {
                return new JsonResult(new { code = 500, message = "شما قبلا از این کد استفاده کرده اید" });
            }
            if (status == DiscountStatus.Finished)
            {
                return new JsonResult(new { code = 501, message = "کد تخفیف تمام شده است!" });
            }
            return BadRequest();
        }
        [HttpPost]
        [Route("getOrdersum")]
        [Authorize]
        public IActionResult getOrdersum()
        {
            var sum = _basketService.GetTotalPriceUserBasket(User.Identity.Name);
            return new JsonResult(new {sum=sum});
        }
    }
}
