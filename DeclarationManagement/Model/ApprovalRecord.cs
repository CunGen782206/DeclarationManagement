using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeclarationManagement.Model;

public class ApprovalRecord
{
    /// <summary>
    /// 审批记录ID（自增）
    /// </summary>
    public int ApprovalID { get; set; }
    
    /// <summary>
    /// 申请表单ID
    /// </summary>
    public int ApplicantID { get; set; }
   
    /// <summary>
    /// 审批人ID
    /// </summary>
    public int ApproverID { get; set; }
    
    /// <summary>
    /// 申请时间
    /// </summary>
    public DateTime ApprovalDate { get; set; }
    
    /// <summary>
    /// 审批决定
    /// </summary>
    public bool Decision { get; set; }
    
    /// <summary>
    /// 审批意见（拒绝或者同意的意见）
    /// </summary>
    public string Comments { get; set; }

    // 导航属性
    [ForeignKey("ApplicantID")]
    public ApplicationForm ApplicationForm { get; set; }
    
    [ForeignKey("ApproverID")]
    public User Approver { get; set; }
    
}