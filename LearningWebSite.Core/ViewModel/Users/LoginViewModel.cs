using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace LearningWebSite.Core.ViewModel.Users
{
    public class LoginViewModel
    {

        [Display(Name = "ایمیل")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(50, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        [EmailAddress(ErrorMessage = "ایمیل وارد شده معتبر نمی باشد")]
        public string Email { get; set; }

        [Display(Name = "کلمه عبور")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(50, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
        public string Password { get; set; }

        [Display(Name = "مرا به خاطر بسپار")]
        public bool RememberMe { get; set; }
        [AllowNull]
        public string? returnUrl { get; set; }

    }

    //public class AdminLoginViewModel
    //{
    //    [Display(Name = "نام کاربری")]
    //    [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
    //    [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
    //    public string Username { get; set; }

    //    [Display(Name = "کلمه عبور")]
    //    [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
    //    [MaxLength(200, ErrorMessage = "{0} نمی تواند بیشتر از {1} کاراکتر باشد .")]
    //    public string Password { get; set; }

    //    [Display(Name = "مرا به خاطر بسپار")]
    //    public bool RememberMe { get; set; }
    //}
}
