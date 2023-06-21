using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace LearningWebSite.DataLayer.Entities.Courses
{
    public class CourseEpisode
    {
        [Key]
        public int EpisodeId { get; set; }

        public int CourseId { get; set; }

        [Display(Name = "عنوان بخش")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(400, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string EpisodeTitle { get; set; }

        [Display(Name = "زمان")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public TimeSpan EpisodeTime { get; set; }

        [Display(Name = "فایل")]
        [MaxLength(150)]
        [AllowNull]
        public string? EpisodeFileName { get; set; }

        [Display(Name = "رایگان")]
        public bool IsFree { get; set; }
        [Display(Name = "نیاز به لاگین دارد؟")]
        public bool Login { get; set; }
        [AllowNull]
        [Display(Name = "آدرس فایل")]
        public string? FileUrl { get; set; } = string.Empty;
        public Course Course { get; set; }

    }
}
