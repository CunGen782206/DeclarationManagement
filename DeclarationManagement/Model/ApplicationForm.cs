using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace DeclarationManagement.Model;

public class ApplicationForm
{
    /// <summary>
    /// 表单ID（自动生成）
    /// </summary>
    public int ApplicationFormID { get; set; }//这个如果不指定未主键，则需要ApplicationForm+ID（类名+ID名）

    /// <summary>
    /// 项目负责人（可修改）
    /// </summary>
    public string ProjectLeader { get; set; } = "";

    /// <summary>
    /// 联系方式（可修改）
    /// </summary>
    public string ContactWay { get; set; } = "";

    /// <summary>
    /// 所属部门（可修改）
    /// </summary>
    public string Department { get; set; } = "";

    /// <summary>
    /// 项目名称（可修改）
    /// </summary>
    public string ProjectName { get; set; } = "";

    /// <summary>
    /// 项目类别（可修改）
    /// </summary>
    public string ProjectCategory { get; set; } = "";

    /// <summary>
    /// 项目等级（可修改）
    /// </summary>
    public string ProjectLevel { get; set; } = "";

    /// <summary>
    /// 奖项级别（可修改）
    /// </summary>
    public string AwardLevel { get; set; } = "";

    /// <summary>
    /// 参与形式（可修改）
    /// </summary>
    public string ParticipationForm { get; set; } = "";

    /// <summary>
    /// 认定批文文件名称（可修改）
    /// </summary>
    public string ApprovalFileName { get; set; } = "";

    /// <summary>
    /// 认定批文文件号（可修改）
    /// </summary>
    public string ApprovalFileNumber { get; set; } = "";

    /// <summary>
    /// 项目内容（可修改）
    /// </summary>
    public string ItemDescription { get; set; } = "";

    /// <summary>
    /// 项目成果（可修改）
    /// </summary>
    public string ProjectOutcome { get; set; } = "";

    /// <summary>
    /// 最终处理意见（可修改，（0未审核，1拟同意，2拟不同意，3不同意））
    /// 默认值为0
    /// </summary>
    public int Decision { get; set; }

    /// <summary>
    /// 审核部门（最终输入）
    /// </summary>
    public string AuditDepartment { get; set; } = "";

    /// <summary>
    /// 原因（最终输入）
    /// </summary>
    public string Comments { get; set; } = "";

    /// <summary>
    /// 认定项目等级
    /// </summary>
    public string RecognitionProjectLevel { get; set; }
    
    /// <summary>
    /// 认定奖项级别
    /// </summary>
    public string RecognitionAwardLevel { get; set; }

    /// <summary>
    /// 认定金额（最终输入）
    /// </summary>
    public decimal DeemedAmount { get; set; }

    /// <summary>
    /// 备注（最终输入）
    /// </summary>
    public string Remarks { get; set; } = "";

    /// <summary>
    /// 用户ID（关联到User表中）
    /// </summary>
    public int UserID { get; set; }


    /// <summary>
    /// 申请时间（一次记录）
    /// </summary>
    public DateTime ApprovalDate { get; set; }

    /// <summary>
    /// 审核状态（0未审核，1预审已完成，2初审完成）完成代表通过或不通过。
    /// </summary>
    public int States { get; set; }

    /// <summary>
    /// 导航属性
    /// </summary>
    // 导航属性
    // [JsonIgnore]//防止Json一直在里面循环
    [ForeignKey("UserID")]
    public User User { get; set; }

    /// <summary>
    /// 当前表单经历的审核历程
    /// </summary>
    public ICollection<ApprovalRecord> ApprovalRecords { get; set; } = new List<ApprovalRecord>();
}

/// <summary>
/// 项目类型
/// </summary>
public enum ProjectCategory
{
    专业建设类,
    课程建设类,
    师资建设类,
    教学竞赛类,
    教材建设类,
    教学成果类
}