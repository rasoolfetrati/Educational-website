using LearningWebSite.DataLayer.Entities.Users;
using System.ComponentModel.DataAnnotations;

namespace LearningWebSite.DataLayer.Entities.Discounts;

public class Discount
{

    [Key]
    public int DiscountId { get; set; }

    [Required]
    [MaxLength(150)]
    public string DiscountCode { get; set; }

    [Required]
    public int DiscountPercent { get; set; }

    public int? UsableCount { get; set; }

    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    #region Rels

    public List<UserDiscountCode> DiscountCodes { get; set; }

    #endregion
}
