using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeclarationManagement.Model;

public class ApprovalFlow
{
    [Key]
    public int RequestID { get; set; }

    [Required]
    public int StepNumber { get; set; }

    [Required]
    public int RequestType { get; set; }   // 关联User表UserID

    // 导航属性
    [ForeignKey("RequestType")]
    public virtual User RequestUser { get; set; }
}