namespace DeclarationManagement.Model.DTO;

/// <summary> 审核表单DTO </summary>
public class ApprovalRecordDTO
{
    public int ApprovalRecordID { get; set; }
    public int ApplicationFormID { get; set; }
    public int UserID { get; set; }
    public DateTime ApprovalDate { get; set; }
    public int Decision { get; set; }
    public string Comments { get; set; } = "";
}