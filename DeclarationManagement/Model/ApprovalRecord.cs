using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeclarationManagement.Model;

public class ApprovalRecord
{
    [Key]
    public int ApprovalID { get; set; }

    [Required]
    public int ApplicantID { get; set; }

    [Required]
    public int ApproverID { get; set; }

    [Required]
    public DateTime ApprovalDate { get; set; }

    [Required]
    public bool Decision { get; set; }

    [StringLength(8000)]
    public string Comments { get; set; }

    // 导航属性
    [ForeignKey("ApplicantID")]
    public virtual ApplicationForm ApplicationForm { get; set; }

    [ForeignKey("ApproverID")]
    public virtual User Approver { get; set; }
    
}