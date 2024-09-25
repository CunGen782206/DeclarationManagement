namespace DeclarationManagement.Model.DTO;

public class TableSummaryDTO
{
    public int SummaryID { get; set; }
    public int UserID { get; set; }
    public int FormID { get; set; }
    public bool HasOperated { get; set; }
    public bool? ApprovalResult { get; set; }
    public bool ApprovalEnding { get; set; }
    
    // 可选：包含用户和表单的信息
    public UserDTO User { get; set; }
    public ApplicationFormDTO ApplicationForm { get; set; }
}