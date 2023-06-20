using LearningWebSite.DataLayer.Entities.Discounts;
using Microsoft.EntityFrameworkCore;

namespace LearningWebSite.Core.Services.DiscountService;

public interface IDiscountService
{
    Task AddDiscountCode(Discount discount);
    IList<Discount> GetDiscountCodes();
    void EditDiscount(Discount discount);
    void DeleteDiscountCode(int discountId);
    bool IsDiscountCodeExist(string code);
    Discount FindDiscountById(int discountId);
    DiscountStatus UseDiscount(string userId, int orderId, string code);
}
