using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeclarationManagement.Model;

public class TableSummary
{
    [Key]
    public int SummaryID { get; set; }

    [Required]
    public int UserID { get; set; } // 当前用户ID

    [ForeignKey("UserID")]
    public User User { get; set; }

    [Required]
    public int FormID { get; set; } // 表单ID

    [ForeignKey("FormID")]
    public ApplicationForm ApplicationForm { get; set; }

    [Required]
    public bool HasOperated { get; set; } = false; // 是否已操作

    public bool? ApprovalResult { get; set; } // 审核结果，true: 同意，false: 拒绝，null: 未审核

    [Required]
    public bool ApprovalEnding { get; set; } = false; // 当前审核是否结束

}