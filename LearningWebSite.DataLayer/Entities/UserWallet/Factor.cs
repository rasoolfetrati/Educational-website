
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace LearningWebSite.DataLayer.Entities.UserWallet;

public class Factor
{
    [Key]
    public int FactorId { get; set; }
    public string Username { get; set; }
    public int Amount { get; set; }
    public string Description { get; set; }
    public DateTime CreateDate { get; set; }
    public string? Code { get; set; }
    [AllowNull]
    public UserOperationType UserOperationType { get; set; }
    public bool IsPay { get; set; }

}
