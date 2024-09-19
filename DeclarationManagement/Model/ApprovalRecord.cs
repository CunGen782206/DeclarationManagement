using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeclarationManagement.Model;

public class ApprovalRecord
{
    [Key]
    public int ApprovalID { get; set; }

    [Required]
    public int FormID { get; set; }

    [ForeignKey("FormID")]
    public ApplicationForm ApplicationForm { get; set; }

    [Required]
    public int ApproverID { get; set; }

    [ForeignKey("ApproverID")]
    public User Approver { get; set; }

    [Required]
    public DateTime ApprovalDate { get; set; } = DateTime.Now;

    [Required]
    public bool Decision { get; set; } // true: 同意，false: 拒绝

    public string Comments { get; set; }
    
}