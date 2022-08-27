using LearningWebSite.DataLayer.Entities.Users;
using LearningWebSite.DataLayer.Entities.UserWallet;
using System.ComponentModel.DataAnnotations;

namespace LearningWebSite.DataLayer.Entities.Basket;

public class Order
{
    [Key]
    public int OrderId { get; set; }
    public string Username { get; set; }
    public bool IsPay { get; set; } = false;
    public int OrderSum { get; set; }
    public DateTime PayDate { get; set; } = DateTime.UtcNow;

    public virtual List<OrderDetail> OrderDetail { get; set; }
}
