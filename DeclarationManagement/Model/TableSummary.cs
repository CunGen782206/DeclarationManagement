using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeclarationManagement.Model;

public class TableSummary
{
    [Key]
    public int SummaryID { get; set; }

    [Required]
    public int UserID { get; set; }

    [Required]
    public int ApplicantID { get; set; }

    [Required]
    public bool State { get; set; }

    [Required]
    public bool ApprovalEnding { get; set; }

    // 导航属性
    [ForeignKey("UserID")]
    public virtual User User { get; set; }

    [ForeignKey("ApplicantID")]
    public virtual ApplicationForm ApplicationForm { get; set; }

}