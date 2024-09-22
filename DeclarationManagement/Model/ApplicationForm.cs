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
        public string Department { get; set; }

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
        [StringLength(8000)]
        public string ItemDescription { get; set; }

        [Required]
        [StringLength(8000)]
        public string ProjectOutcome { get; set; }

        [Required]
        public bool Decision { get; set; }

        [Required]
        [StringLength(50)]
        public string AuditDepartment { get; set; }

        [Required]
        [StringLength(8000)]
        public string Comments { get; set; }

        [Required]
        [StringLength(50)]
        public string RecognitionLevel { get; set; }

        [Required]
        public decimal DeemedAmount { get; set; }

        [Required]
        [StringLength(8000)]
        public string Remarks { get; set; }

        [Required]
        public int UserID { get; set; }

        [Required]
        public bool State { get; set; } = false;    // 0表示查看，1表示修改

        [Required]
        public DateTime ApprovalDate { get; set; }

        [Required]
        public bool ApprovalEnding { get; set; } = false;   // 0表示审核未完成，1表示完成

        // 导航属性
        [ForeignKey("UserID")]
        public virtual User User { get; set; }
}