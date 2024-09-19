using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeclarationManagement.Model;

public class ApprovalFlow
{
    [Key] public int FlowID { get; set; }

    [Required] public int StepNumber { get; set; } // 流程步骤编号

    [Required] [StringLength(50)] public string Role { get; set; } // 该步骤对应的审批角色
}