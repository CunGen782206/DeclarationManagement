namespace DeclarationManagement.Model.DTO;

public class TableSummaryDTO
{
    /// <summary>
    /// 表汇总
    /// </summary>
    public int TableSummaryID { get; set; }
    
    /// <summary>
    /// 用户ID
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
    
}