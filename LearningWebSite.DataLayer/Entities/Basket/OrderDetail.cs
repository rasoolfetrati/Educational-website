using LearningWebSite.DataLayer.Entities.Courses;
using System.ComponentModel.DataAnnotations.Schema;

namespace LearningWebSite.DataLayer.Entities.Basket;

public class OrderDetail
{
    public int orderDetailId { get; set; }
    public int OrderID { get; set; }
    public int CourseId { get; set; }
    public int coursePrice { get; set; }

    #region RelationShips

    public virtual Order Order { get; set; }

    [ForeignKey("CourseId")]
    public virtual Course Course { get; set; }

    #endregion
}
