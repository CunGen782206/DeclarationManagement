using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeclarationManagement.Model;

public class TableSummary
{
    /// <summary>
    /// 表汇总ID
    /// </summary>
    public int SummaryID { get; set; }
    
    /// <summary>
    /// 用户ID
    /// </summary>
    public int UserID { get; set; }
    
    /// <summary>
    /// 申请表单ID
    /// </summary>
    public int ApplicantID { get; set; }
    
    /// <summary>
    /// 审批表汇总的操作记录（当前审批是否已操作）
    /// </summary>
    public bool State { get; set; }
    
    /// <summary>
    /// 当前审核是否通过 
    /// </summary>
    public bool ApprovalEnding { get; set; }

    // 导航属性
    [ForeignKey("UserID")]
    public User User { get; set; }
    
    [ForeignKey("ApplicantID")]
    public ApplicationForm ApplicationForm { get; set; }

}