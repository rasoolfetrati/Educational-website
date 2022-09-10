using System.ComponentModel.DataAnnotations;

namespace LearningWebSite.DataLayer.Entities.ContactUs;

public class Contacts
{
    [Key]
    public int ContactId { get; set; }
    [Required(ErrorMessage = "لطقا نام کامل خود را وارد نمایید!")]
    public string Fullname { get; set; }
    [Required(ErrorMessage ="لطقا ایمیل خود را وارد نمایید!")]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; }
    [Required(ErrorMessage = "لطقا پیام خود را وارد نمایید!")]
    public string Message { get; set; }
}
