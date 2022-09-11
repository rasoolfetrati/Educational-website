using LearningWebSite.DataLayer.Context;
using LearningWebSite.DataLayer.Entities.Discounts;

namespace LearningWebSite.Core.Services.DiscountService;

public interface IDiscountService
{
    Task AddDiscountCode(Discount discount);
    IList<Discount> GetDiscountCodes();
    void EditDiscount(Discount discount);
    void DeleteDiscountCode(int discountId);
    bool IsDiscountCodeExist(string code);
    Discount FindDiscountById(int discountId);
}
public class DiscountService : IDiscountService
{
    ApplicationDbContext context;
    public DiscountService(ApplicationDbContext context)
    {
        this.context=context;
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
        throw new NotImplementedException();
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
        return context.Discounts.Any(c=>c.DiscountCode == code);
    }
}
