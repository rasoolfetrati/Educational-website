using LearningWebSite.DataLayer.Entities.Users;
using System.ComponentModel.DataAnnotations;

namespace LearningWebSite.DataLayer.Entities.Courses;

public class UserInCourse
{
    [Key]
    public int UC_ID { get; set; }
    public int CourseId { get; set; }
    public string UserName { get; set; }


    public CustomUser User { get; set; }
    public Course Course { get; set; }
}
