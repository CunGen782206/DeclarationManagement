using System.ComponentModel.DataAnnotations;

namespace DeclarationManagement.Model;

public class User
{
    public int UserID { get; set; }
    public string Username { get; set; }
    public byte[] Password { get; set; }
    public string Role { get; set; }
    public string Power { get; set; }

    // 导航属性
    public ICollection<ApplicationForm> ApplicationForms { get; set; }
    public ICollection<ApprovalRecord> ApprovalRecords { get; set; }
    public ICollection<TableSummary> TableSummaries { get; set; }
    
}