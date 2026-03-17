namespace DeclarationManagement.Model.DTO;

/// <summary>
/// 用户管理部分的DTO
/// </summary>
public class UserManageDTO
{
    public int UserID { get; set; }
    public string Role { get; set; } = "";
    public string Power { get; set; } = "";
    public string JobNumber { get; set; } = "";
    public string Name { get; set; } = "";
}