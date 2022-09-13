using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LearningWebSite.DataLayer.Entities.Comments;
using LearningWebSite.DataLayer.Entities.Courses;
using Microsoft.AspNetCore.Http;

namespace LearningWebSite.Core.ViewModel.CourseVM
{
    public class CourseIndexViewModel
    {
        public int CourseId { get; set; }
        public string CourseTitle { get; set; }
        public string CourseImageName { get; set; }
        public int CoursePrice { get; set; }

    }
    public class CourseViewModel
    {

        [Required]
        public int GroupId { get; set; }

        public int? SubGroup { get; set; }

        [Required]
        public string TeacherId { get; set; }

        [Display(Name = "عنوان دوره")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(450, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string CourseTitle { get; set; }

        [Display(Name = "شرح دوره")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string CourseDescription { get; set; }

        [Display(Name = "قیمت دوره")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public int CoursePrice { get; set; }

        [Required]
        public int courseLevel { get; set; }
        [Required]
        public int courseStatus { get; set; }
        [MaxLength(50)]
        public string CourseImageName { get; set; }

        [MaxLength(100)]
        public string DemoFileName { get; set; }

        [Required]
        public DateTime CreateDate { get; set; } = DateTime.Now;

        public DateTime? UpdateDate { get; set; }
        public bool IsDelete { get; set; } = false;
        public IFormFile imgCourseUp { get; set; }
        public IFormFile demoUp { get; set; }
    }
    public class ShowCourseViewModelWithIndex
    {
        public List<ShowCourseViewModel> showCourseViewModels { get; set; }
        public int CurrentPageIndex { get; set; }
        public int PageCount { get; set; }
    }
    public class ShowCourseViewModel
    {
        public int CourseId { get; set; }

        public int GroupId { get; set; }

        public int? SubGroup { get; set; }

        public string TeacherId { get; set; }

        public string CourseTitle { get; set; }

        public string CourseDescription { get; set; }

        public int CoursePrice { get; set; }

        [Required]
        public int courseLevel { get; set; }
        [Required]
        public int courseStatus { get; set; }
        public string CourseImageName { get; set; }

        public string DemoFileName { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime? UpdateDate { get; set; }
        public bool IsDelete { get; set; }
        public List<Comment> comments { get; set; }
    }

    public class GetUserCourseViewModel
    {
        public int CourseId { get; set; }
        public string CourseTitle { get; set; }
        public string CourseImageName { get; set; }
    }
     public class ShowCourseListItemViewModel
    {
        public int CourseId { get; set; }
        public string Title { get; set; }
        public string ImageName { get; set; }
        public int Price { get; set; }
        public List<CourseEpisode> CourseEpisodes { get; set; }
    }
}
