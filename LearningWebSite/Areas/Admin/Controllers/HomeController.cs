using LearningWebSite.Core.InfraStructure;
using LearningWebSite.Core.Services.ContactUsService;
using LearningWebSite.Core.Services.DiscountService;
using LearningWebSite.DataLayer.Entities.Discounts;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace LearningWebSite.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AutoValidateAntiforgeryToken]
    public class HomeController : AdminControllerBase
    {
        IContactUsService contactUsService;
        IDiscountService DiscountService;
        public HomeController(IContactUsService contactUsService, IDiscountService discountService)
        {
            this.contactUsService = contactUsService;
            this.DiscountService = discountService;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        [Route("ContactUs")]
        public IActionResult ContactUs()
        {
            return View(contactUsService.GetAllMessages());
        }

        public async Task<IActionResult> Discount()
        {
            var data = DiscountService.GetDiscountCodes();
            return View(data);
        }
        [HttpGet]
        public async Task<IActionResult> CreateDiscount()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateDiscount(Discount discount, string sdDate, string edDate)
        {
            if (!string.IsNullOrWhiteSpace(discount.DiscountCode))
            {
                ModelState.AddModelError(String.Empty, "لطفا کد تخفیف را وارد نمایید!");
                return View(discount);
            }
            if (discount.DiscountPercent != null || discount.DiscountPercent <= 0)
            {
                ModelState.AddModelError(String.Empty, "لطفا تعداد کد تخفیف را وارد نمایید!");
                return View(discount);
            }
            if (!DiscountService.IsDiscountCodeExist(discount.DiscountCode))
            {
                ModelState.AddModelError(String.Empty, "این کد تخفیف از قبل موجود است!");
                return View(discount);
            }
            if (!string.IsNullOrEmpty(sdDate))
            {
                string[] std = sdDate.Split("/");
                discount.StartDate = new DateTime(int.Parse(std[0]), int.Parse(std[1]), int.Parse(std[2]),
                    new PersianCalendar());
            }

            if (!string.IsNullOrEmpty(edDate))
            {
                string[] edd = edDate.Split("/");
                discount.EndDate = new DateTime(int.Parse(edd[0]), int.Parse(edd[1]), int.Parse(edd[2]),
                    new PersianCalendar());
            }
            await DiscountService.AddDiscountCode(discount);
            return RedirectAndShowAlert(OperationResult.Success(), RedirectToAction(nameof(Discount)));
        }
    }
}
