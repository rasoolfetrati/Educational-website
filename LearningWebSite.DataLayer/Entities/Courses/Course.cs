using LearningWebSite.DataLayer.Entities.Basket;
using LearningWebSite.DataLayer.Entities.Comments;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LearningWebSite.DataLayer.Entities.Courses;

public class Course
{
    [Key]
    public int CourseId { get; set; }

    [Required]
    public int GroupId { get; set; }

    public int? SubGroup { get; set; }

    [Required]
    public string TeacherId { get; set; }

    [Display(Name = "عنوان دوره")]
    [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
    [MaxLength(450, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
    public string CourseTitle { get; set; }
    [Display(Name = "Url دوره")]
    [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
    [MaxLength(450, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
    public string Slug { get; set; }
    [Display(Name = "نحوه برگذاری دوره")]
    [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
    public string CoursePresentation { get; set; }
    [Display(Name = "شرح دوره")]
    [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
    public string CourseDescription { get; set; }

    [Display(Name = "قیمت دوره")]
    [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
    public int CoursePrice { get; set; }

    [MaxLength(50)]
    public string CourseImageName { get; set; }

    [MaxLength(100)]
    public string DemoFileName { get; set; }

    [Required]
    public DateTime CreateDate { get; set; } = DateTime.Now;
    [Required]
    public int courseLevel { get; set; }
    [Required]
    public int courseStatus { get; set; }
    public DateTime? UpdateDate { get; set; }
    public bool IsDelete { get; set; } = false;

    #region Relations



    [ForeignKey("GroupId")]
    public CourseGroups CourseGroup { get; set; }

    [ForeignKey("SubGroup")]
    public CourseGroups subGroup { get; set; }

    public List<CourseEpisode> CourseEpisodes { get; set; }
    public List<OrderDetail> OrderDetails { get; set; }
    public List<UserInCourse> UserCourses { get; set; }
    public List<Comment> Comments { get; set; }
    //public List<CourseVote> CourseVotes { get; set; }
    #endregion
}

