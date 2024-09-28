namespace DeclarationManagement.Model.DTO;

/// <summary> 审核组合代码 </summary>
public class ApprovalCombineDTO
{
    /// <summary>
    /// 所审核的表单ID
    /// </summary>
    public int applicationFormID { get; set; }
    
    /// <summary>
    /// 当前的审批用户的表汇总ID
    /// </summary>
    public int TableSummaryID { get; set; }

    #region 审批记录部分

    /// <summary>
    /// 审批人ID
    /// </summary>
    public int UserID { get; set; }

    /// <summary>
    /// 当前审批决定
    /// </summary>
    public int Decision { get; set; }

    /// <summary>
    /// 审批意见（原因）
    /// </summary>
    public string Comments { get; set; }
    
    /// <summary>
    /// 认定等级
    /// </summary>
    public string RecognitionLevel { get; set; }
    
    
    /// <summary>
    /// 认定金额
    /// </summary>
    public int DeemedAmount { get; set; }
    
    /// <summary>
    /// 备注（选填）
    /// </summary>
    public string Remarks { get; set; }

    #endregion
    
}