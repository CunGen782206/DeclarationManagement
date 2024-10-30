using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace DeclarationManagement.Model;

public class TableSummary
{
    /// <summary>
    /// 表汇总ID
    /// </summary>
    public int TableSummaryID { get; set; }
    
    /// <summary>
    /// 目标审核人ID
    /// </summary>
    public int UserID { get; set; }
    
    /// <summary>
    /// 申请表单ID
    /// </summary>
    public int ApplicationFormID { get; set; }
    
    /// <summary>
    /// 审批表操作情况（0未审核，1拟同意，2拟不同意，3不同意）
    /// </summary>
    public int Decision { get; set; }
    
    //TODO:添加当前审核属于第几个状态

    // 导航属性
    // [JsonIgnore]
    [ForeignKey("UserID")]
    public User User { get; set; }
    
    // [JsonIgnore]
    [ForeignKey("ApplicationFormID")]
    public ApplicationForm ApplicationForm { get; set; }

}