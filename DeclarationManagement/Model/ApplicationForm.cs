using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeclarationManagement.Model;

public class ApplicationForm
{
    public int FormID { get; set; }
    public string ProjectLeader { get; set; }
    public string ContactWay { get; set; }
    public string Department { get; set; }
    public string ProjectName { get; set; }
    public string ProjectCategory { get; set; }
    public string ProjectLevel { get; set; }
    public string AwardLevel { get; set; }
    public string ParticipationForm { get; set; }
    public string ApprovalFileName { get; set; }
    public string ApprovalFileNumber { get; set; }
    public string ItemDescription { get; set; }
    public string ProjectOutcome { get; set; }
    public bool Decision { get; set; }
    public string AuditDepartment { get; set; }
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