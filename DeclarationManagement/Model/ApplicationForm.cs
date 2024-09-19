using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeclarationManagement.Model;

public class ApplicationForm
{
    [Key]
    public int FormID { get; set; }

    [Required]
    [StringLength(50)]
    public string ProjectLeader { get; set; }

    [StringLength(50)]
    public string ContactWay { get; set; }

    [StringLength(50)]
    public string DepartmentID { get; set; } // 修改为字符串类型，且不作为外键

    [Required]
    [StringLength(150)]
    public string ProjectName { get; set; }

    [Required]
    [StringLength(50)]
    public string ProjectCategory { get; set; }

    [Required]
    [StringLength(50)]
    public string ProjectLevel { get; set; }

    [Required]
    [StringLength(50)]
    public string AwardLevel { get; set; }

    [Required]
    [StringLength(50)]
    public string ParticipationForm { get; set; }

    [Required]
    [StringLength(500)]
    public string ApprovalFileName { get; set; }

    [Required]
    [StringLength(500)]
    public string ApprovalFileNumber { get; set; }

    [Required]
    public string ItemDescription { get; set; }

    [Required]
    public string ProjectOutcome { get; set; }

    public bool? Decision { get; set; }

    [StringLength(50)]
    public string AuditDepartment { get; set; }

    public string Comments { get; set; }

    [StringLength(50)]
    public string RecognitionLevel { get; set; }

    public decimal? DeemedAmount { get; set; }

    public string Remarks { get; set; }

    [Required]
    public int UserID { get; set; }

    [ForeignKey("UserID")]
    public User User { get; set; }

    [Required]
    public bool State { get; set; } = false;

    [Required]
    public DateTime ApplicationDate { get; set; } = DateTime.Now;

    [Required]
    public bool ApprovalEnding { get; set; } = false;
}