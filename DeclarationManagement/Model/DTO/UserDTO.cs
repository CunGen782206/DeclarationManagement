namespace DeclarationManagement.Model.DTO;

public class UserDTO
{
    public int UserID { get; set; }
    public string Username { get; set; } = "";
    public string Password { get; set; } = "";
    public string Role { get; set; } = "";
    public string Power { get; set; } = "";
    public string JobNumber { get; set; } = "";
    public string Name { get; set; } = "";
}