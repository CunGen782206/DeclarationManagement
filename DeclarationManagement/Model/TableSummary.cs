using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeclarationManagement.Model;

public class TableSummary
{
    public int SummaryID { get; set; }
    public int UserID { get; set; }
    public int ApplicantID { get; set; }
    public bool State { get; set; }
    public bool ApprovalEnding { get; set; }

    // 导航属性
    [ForeignKey("UserID")]
    public User User { get; set; }
    
    [ForeignKey("ApplicantID")]
    public ApplicationForm ApplicationForm { get; set; }

}