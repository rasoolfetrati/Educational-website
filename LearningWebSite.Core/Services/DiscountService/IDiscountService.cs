using LearningWebSite.Core.Services.BasketService;
using LearningWebSite.DataLayer.Context;
using LearningWebSite.DataLayer.Entities.Discounts;
using LearningWebSite.DataLayer.Entities.Users;
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
public class DiscountService : IDiscountService
{
    ApplicationDbContext context;
    IBasketService basketService;
    public DiscountService(ApplicationDbContext context, IBasketService basketService)
    {
        this.context = context;
        this.basketService = basketService;
    }
    public async Task AddDiscountCode(Discount discount)
    {
        await context.Discounts.AddAsync(discount);
        await context.SaveChangesAsync();
        await Task.CompletedTask;
    }

    public void DeleteDiscountCode(int discountId)
    {
        var discont = context.Discounts.Find(discountId);
        context.Discounts.Remove(discont);
        context.SaveChanges();
    }

    public void EditDiscount(Discount discount)
    {
        context.Discounts.Update(discount);
        context.SaveChanges();
    }

    public Discount FindDiscountById(int discountId)
    {
        return context.Discounts.Find(discountId);
    }

    public IList<Discount> GetDiscountCodes()
    {
        return context.Discounts.ToList();
    }

    public bool IsDiscountCodeExist(string code)
    {
        return context.Discounts.Any(c => c.DiscountCode == code);
    }

    public DiscountStatus UseDiscount(string userId, int orderId, string code)
    {
        var discount = context.Discounts.SingleOrDefault(d => d.DiscountCode == code);
        if (discount == null)
        {
            return DiscountStatus.NotFound;
        }
        if (discount.StartDate != null && discount.StartDate >= DateTime.Now)
            return DiscountStatus.Expire;
        if (discount.EndDate != null && discount.EndDate <= DateTime.Now)
            return DiscountStatus.Expire;
        if (discount.UsableCount != null && discount.UsableCount < 1)
            return DiscountStatus.Finished;

        var order = basketService.GetOrderById(orderId);
        if (context.UserDiscountCodes.Any(u => u.UserId == userId && u.DiscountId == discount.DiscountId))
        {
            return DiscountStatus.Used;
        }
        int percent = (order.OrderSum * discount.DiscountPercent) / 100;
        order.OrderId = orderId;
        order.OrderSum = order.OrderSum - percent;
        basketService.UpdateOrder(order);


        if (discount.UsableCount != null)
        {
            discount.UsableCount -= 1;
        }

        context.Discounts.Update(discount);
        context.UserDiscountCodes.Add(new UserDiscountCode()
        {
            DiscountId = discount.DiscountId,
            UserId = userId,
        });
        context.SaveChanges();

        return DiscountStatus.Success;
    }
}
