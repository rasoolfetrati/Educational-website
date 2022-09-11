using System.ComponentModel.DataAnnotations;

namespace LearningWebSite.DataLayer.Entities.ContactUs;

public class Contacts
{
    [Key]
    public int ContactId { get; set; }
    [Required(ErrorMessage = "لطفا نام کامل خود را وارد نمایید!")]
    [MaxLength(50)]
    public string Fullname { get; set; }
    [Required(ErrorMessage ="لطفا ایمیل خود را وارد نمایید!")]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; }
    [Required(ErrorMessage = "لطفا پیام خود را وارد نمایید!")]
    [MaxLength(500)]
    public string Message { get; set; }
}
