using LearningWebSite.Core.InfraStructure;
using LearningWebSite.Core.Services.CourseService;
using LearningWebSite.Core.Services.WalletService;
using LearningWebSite.Core.ViewModel.BasketVM;
using LearningWebSite.DataLayer.Context;
using LearningWebSite.DataLayer.Entities.Basket;
using LearningWebSite.DataLayer.Entities.Courses;
using LearningWebSite.DataLayer.Entities.UserWallet;
using Microsoft.EntityFrameworkCore;

namespace LearningWebSite.Core.Services.BasketService;

public interface IBasketService
{
    Task<OperationResult> AddCourseToBasket(int courseId, string username);
    List<ShowBasketVM> GetBasketItems(string username);
    Task<List<ShowBasketVM>> GetCourses(string username);
    int GetTotalPriceUserBasket(string username);
    Task<int> CreateOrder(List<int> ids, string username);
    void UpdateOrderPrice(int orderId);
    int orderSum(int orderId, string username);
    int BasketItemsCount(string username);
    void UpdateOrder(int orderId, string username);
    void AddToUserCourse(int orderId, string username, string code);
    void PayWithWallet(string username, int orderId);
    void DeleteUserBasket(string username);
    List<Basket> GetUserBaskets(string username);
    int GetUserBasketsCount(string username);
    void DeleteBasket(int basketId);
    Order GetOrderById(int orderId);
    void UpdateOrder(Order order);
    int GetOrderId(string username);
}
public class BasketService : IBasketService
{
    private readonly ApplicationDbContext context;
    private readonly IFactorService _factorService;
    private readonly ICourseService courseService;
    private readonly IUserService userService;
    public BasketService(ApplicationDbContext context, IFactorService factorService, ICourseService courseService, IUserService userService)
    {
        this.context = context;
        _factorService = factorService;
        this.courseService = courseService;
        this.userService = userService;
    }

    public async Task<OperationResult> AddCourseToBasket(int courseId, string username)
    {
        var course = await courseService.GetCourseById(courseId);
        if (context.Baskets.Any(b => b.CourseId == courseId && b.UserName == username))
        {
            return OperationResult.Error("این دوره از قبل در سبد خرید شما موجود است.");
        }
        Basket basket = new Basket()
        {
            CourseId = courseId,
            CoursePrice = course.CoursePrice,
            IsFinally = false,
            UserName = username,
            CourseImage = course.CourseImageName,
            CourseTitle = course.CourseTitle,
        };
        List<int> ids = new List<int>();
        ids.Add(courseId);
        await context.Baskets.AddAsync(basket);
        await CreateOrder(ids, username);
        await context.SaveChangesAsync();
        return OperationResult.Success($"دوره {course.CourseTitle} با موفقیت به سبد خرید شما افزوده شد.");
    }

    public void AddToUserCourse(int orderId, string username, string code)
    {
        var orderUpdate = context.Orders.First(o => o.OrderId == orderId);

        var order = context.Orders.Include(o => o.OrderDetail).ThenInclude(od => od.Course)
               .Where(o => o.Username == username && o.OrderId == orderId && !o.IsPay).ToList();
        if (order != null)
        {
            foreach (var item in order)
            {
                foreach (var item2 in item.OrderDetail)
                {
                    context.UserInCourses.AddRange(new UserInCourse()
                    {
                        CourseId = item2.CourseId,
                        UserName = username
                    });
                }
            }
            orderUpdate.OrderId = orderId;
            orderUpdate.IsPay = true;
            context.Orders.Update(orderUpdate);
            _factorService.AddOnlineFactor(orderId, username, code);
            context.SaveChanges();
        }
    }

    public int BasketItemsCount(string username)
    {
        var count = context.Baskets.Where(u => u.UserName == username).Count();
        if (count == null || count == 0)
        {
            return 0;
        }
        return count;
    }

    public async Task<int> CreateOrder(List<int> ids, string username)
    {
        List<OrderDetail> list = new List<OrderDetail>();
        Order order = context.Orders
            .FirstOrDefault(o => o.Username == username && !o.IsPay);
        var course = context.Courses.Where(c => ids.Contains(c.CourseId)).ToList();
        if (order == null)
        {
            order = new Order()
            {
                Username = username,
                IsPay = false,
                PayDate = DateTime.UtcNow,
                OrderSum = course.Sum(c => c.CoursePrice),
            };
            await context.Orders.AddAsync(order);
            await context.SaveChangesAsync();

            foreach (var item in course)
            {
                list.Add(new OrderDetail()
                {
                    CourseId = item.CourseId,
                    coursePrice = item.CoursePrice,
                    OrderID = order.OrderId,
                });
            }
            await context.AddRangeAsync(list);
            await context.SaveChangesAsync();
        }
        else
        {
            if (!context.orderDetails.Any(o => ids.Contains(o.CourseId)))
            {
                foreach (var item in course)
                {
                    list.Add(new OrderDetail()
                    {
                        CourseId = item.CourseId,
                        coursePrice = item.CoursePrice,
                        OrderID = order.OrderId,
                    });
                }
                await context.orderDetails.AddRangeAsync(list);

                await context.SaveChangesAsync();
                UpdateOrderPrice(order.OrderId);
            }
        }
        return order.OrderId;

    }
    public void DeleteBasket(int basketId)
    {
        var basket = context.Baskets.Find(basketId);
        var orderDetail = context.orderDetails.SingleOrDefault(c => c.CourseId == basket.CourseId);
        context.orderDetails.Remove(orderDetail);
        context.Baskets.Remove(basket);
        context.SaveChanges();
        UpdateOrderPrice(orderDetail.OrderID);
    }

    public void DeleteUserBasket(string username)
    {
        var baskets = context.Baskets.Where(u => u.UserName == username).ToList();
        context.RemoveRange(baskets);
        context.SaveChanges();
    }

    public List<ShowBasketVM> GetBasketItems(string username)
    {
        var userFactor = context.Baskets.Where(b => b.UserName == username).ToList();
        return userFactor.Select(b => new ShowBasketVM()
        {
            CoursePrice = b.CoursePrice,
            CourseImage = b.CourseImage,
            SumOrder = userFactor.Sum(b => b.CoursePrice),
            CourseTitle = b.CourseTitle,
            CourseId = b.CourseId,
            BasketId = b.BasketId,
        }).ToList();
    }

    public async Task<List<ShowBasketVM>> GetCourses(string username)
    {
        var userBasket = await context.Baskets
                               .Where(t => t.UserName == username && !t.IsFinally).ToListAsync();
        return userBasket.Select(c => new ShowBasketVM()
        {
            CourseId = c.CourseId,
            CoursePrice = c.CoursePrice,
            CourseTitle = c.CourseTitle,
            CourseImage = c.CourseImage,
            BasketId = c.BasketId,
        }).ToList();
    }

    public Order GetOrderById(int orderId)
    {
        return context.Orders.Find(orderId);
    }

    public int GetOrderId(string username)
    {
        return context.Orders.Single(u => u.Username == username && !u.IsPay).OrderId;
    }

    public int GetTotalPriceUserBasket(string username)
    {
        return context.Orders.Where(u => u.Username == username && !u.IsPay).Sum(p => p.OrderSum);
    }

    public List<Basket> GetUserBaskets(string username)
    {
        return context.Baskets.Where(b => b.UserName == username && !b.IsFinally).ToList();
    }

    public int GetUserBasketsCount(string username)
    {
        return context.Baskets.Where(u => u.UserName == username).Count();
    }

    public int orderSum(int orderId, string username)
    {
        return context.Orders.Single(c => c.OrderId == orderId && c.Username == username).OrderSum;
    }

    public void PayWithWallet(string username, int orderId)
    {
        var walletAmount = userService.WalletBalance(username);
        var order = context.Orders.Find(orderId);
        if (walletAmount > order.OrderSum)
        {
            var order2 = context.Orders.Include(o => o.OrderDetail).ThenInclude(od => od.Course)
         .Where(o => o.Username == username && o.OrderId == orderId && !o.IsPay).ToList();
            if (order != null)
            {
                foreach (var item in order2)
                {
                    foreach (var item2 in item.OrderDetail)
                    {
                        context.UserInCourses.AddRange(new UserInCourse()
                        {
                            CourseId = item2.CourseId,
                            UserName = username
                        });
                    }
                }
                order.OrderId = orderId;
                order.IsPay = true;
                DeleteUserBasket(username);
                context.Orders.Update(order);
                _factorService.AddWalletFactor(orderId, username, "");
                context.SaveChanges();
            }
        }
    }
    public void UpdateOrder(int orderId, string username)
    {
        var order = context.Orders.FirstOrDefault(o => o.OrderId == orderId && o.Username == username && !o.IsPay);
        order.IsPay = true;
        context.Orders.Update(order);
        context.SaveChanges();
    }

    public void UpdateOrder(Order order)
    {
        context.Orders.Update(order);
        context.SaveChanges();
    }

    public void UpdateOrderPrice(int orderId)
    {
        var orderDetail = context.Orders.Find(orderId);
        orderDetail.OrderId = orderId;
        orderDetail.OrderSum = context.orderDetails.Where(o => o.OrderID == orderId).Sum(o => o.coursePrice);
        context.Orders.Update(orderDetail);
        context.SaveChanges();
    }

}