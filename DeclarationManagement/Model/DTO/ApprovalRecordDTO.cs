namespace DeclarationManagement.Model.DTO;

public class ApprovalRecordDTO
{
    public int ApprovalID { get; set; }
    public int FormID { get; set; }
    public int ApproverID { get; set; }
    public DateTime ApprovalDate { get; set; }
    public bool Decision { get; set; }
    public string Comments { get; set; }
}