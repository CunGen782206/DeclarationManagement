using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeclarationManagement.Model;

public class ApplicationForm
{
    /// <summary> 表单ID </summary>
    public int FormID { get; set; }
    
    /// <summary> 项目负责人 </summary>
    public string ProjectLeader { get; set; }
    
    /// <summary>
    /// 联系方式
    /// </summary>
    public string ContactWay { get; set; }
    
    /// <summary>
    /// 所属部门
    /// </summary>
    public string Department { get; set; }
    
    /// <summary>
    /// 项目名称
    /// </summary>
    public string ProjectName { get; set; }
    
    /// <summary>
    /// 项目类别
    /// </summary>
    public string ProjectCategory { get; set; }
    
    /// <summary>
    /// 项目等级
    /// </summary>
    public string ProjectLevel { get; set; }
    
    /// <summary>
    /// 奖项级别
    /// </summary>
    public string AwardLevel { get; set; }
    
    /// <summary>
    /// 参与形式
    /// </summary>
    public string ParticipationForm { get; set; }
    
    /// <summary>
    /// 认定批文文件名称
    /// </summary>
    public string ApprovalFileName { get; set; }
    
    /// <summary>
    /// 认定批文文件号
    /// </summary>
    public string ApprovalFileNumber { get; set; }
    
    /// <summary>
    /// 项目内容
    /// </summary>
    public string ItemDescription { get; set; }
    
    /// <summary>
    /// 项目成果
    /// </summary>
    public string ProjectOutcome { get; set; }
    
    /// <summary>
    /// 最终处理意见
    /// </summary>
    public bool Decision { get; set; }
    
    /// <summary>
    /// 审核部门
    /// </summary>
    public string AuditDepartment { get; set; }
    
    /// <summary>
    /// 原因
    /// </summary>
    public string Comments { get; set; }
    public string RecognitionLevel { get; set; }
    public decimal DeemedAmount { get; set; }
    public string Remarks { get; set; }
    public int UserID { get; set; }
    public bool State { get; set; } = false;
    public DateTime ApprovalDate { get; set; }
    public bool ApprovalEnding { get; set; } = false;

    // 导航属性
    public User User { get; set; }
}