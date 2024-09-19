using System.ComponentModel.DataAnnotations;

namespace DeclarationManagement.Model;

public class User
{
    [Key]
    public int UserID { get; set; }

    [Required]
    [StringLength(50)]
    public string Username { get; set; }

    [Required]
    [StringLength(255)]
    public string Password { get; set; } // Hashed Password

    [Required]
    [StringLength(50)]
    public string Role { get; set; } // e.g., "理学院", "教务处"

    [Required]
    [StringLength(50)]
    public string Power { get; set; } // e.g., "普通用户", "审批用户"
}