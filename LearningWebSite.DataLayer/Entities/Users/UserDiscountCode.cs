using LearningWebSite.DataLayer.Entities.Discounts;
using System.ComponentModel.DataAnnotations;

namespace LearningWebSite.DataLayer.Entities.Users;

public class UserDiscountCode
{
    [Key]
    public int UC_Id { get; set; }
    public int DiscountId { get; set; }
    public string UserId { get; set; }
    #region Relations

    public CustomUser User { get; set; }
    public Discount Discount { get; set; }
    #endregion
}
