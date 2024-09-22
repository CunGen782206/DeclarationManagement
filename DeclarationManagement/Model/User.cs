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
    public byte[] Password { get; set; }

    [Required]
    [StringLength(50)]
    public string Role { get; set; }    // 理学院/教务处

    [Required]
    [StringLength(50)]
    public string Power { get; set; }   // 普通用户/审批用户
}