using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeclarationManagement.Model;

public class ApprovalRecord
{
    public int ApprovalID { get; set; }
    public int ApplicantID { get; set; }
    public int ApproverID { get; set; }
    public DateTime ApprovalDate { get; set; }
    public bool Decision { get; set; }
    public string Comments { get; set; }

    // 导航属性
    public ApplicationForm ApplicationForm { get; set; }
    public User Approver { get; set; }
    
}