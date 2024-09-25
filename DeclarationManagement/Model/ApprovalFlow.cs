using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeclarationManagement.Model;

/// <summary>
/// 审批流程表
/// </summary>
public class ApprovalFlow
{
    /// <summary>
    /// 申请流程ID
    /// </summary>
    public int RequestID { get; set; }
    
    /// <summary>
    /// 流程处理编号
    /// </summary>
    public int StepNumber { get; set; }
    
    /// <summary>
    /// 审批人ID
    /// </summary>
    public int UserID { get; set; }

    
    /// <summary>
    /// User使用
    /// </summary>
    [ForeignKey("UserID")]
    // 导航属性
    public User User { get; set; }
}