using LearningWebSite.DataLayer.Entities.Courses;
using LearningWebSite.DataLayer.Entities.Users;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LearningWebSite.DataLayer.Entities.Comments;

public class Comment
{
    [Key]
    public int CommentID { get; set; }
    public int? ParentId { get; set; }
    public int CourseId { get; set; }
    public string UserId { get; set; }
    public DateTime SaveDate { get; set; }
    public bool IsApprove { get; set; }=false;
    public string CommentText { get; set; }
    [ForeignKey("CourseId")]
    public virtual Course Course { get; set; }
    [ForeignKey("UserId")]
    public virtual CustomUser CustomUser { get; set; }
}
