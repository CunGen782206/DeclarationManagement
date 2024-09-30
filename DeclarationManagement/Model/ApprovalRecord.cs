using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace DeclarationManagement.Model;

/// <summary> 审核表单 </summary>
public class ApprovalRecord
{
    /// <summary>
    /// 审批记录ID（自增）
    /// </summary>
    public int ApprovalRecordID { get; set; }

    /// <summary>
    /// 申请表单ID (外键，关联到FormID)
    /// </summary>
    public int ApplicationFormID { get; set; }

    /// <summary>
    /// 审批人ID (外键，关联到UserID)
    /// </summary>
    public int UserID { get; set; }

    /// <summary> 审批时间 </summary>
    public DateTime ApprovalDate { get; set; }

    /// <summary>
    /// 审批决定（0未审核，1拟同意，2拟不同意，3不同意）
    /// </summary>
    public int Decision { get; set; }

    /// <summary>
    /// 审批意见（拒绝或者同意的意见）
    /// </summary>
    public string Comments { get; set; }

    // 导航属性
    [JsonIgnore]
    [ForeignKey("ApplicationFormID")]
    public ApplicationForm ApplicationForm { get; set; }

    [JsonIgnore] [ForeignKey("UserID")] public User User { get; set; }
}