using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeclarationManagement.Model;

public class ApprovalFlow
{
    public int RequestID { get; set; }
    public int StepNumber { get; set; }
    public int UserID { get; set; }

    // 导航属性
    public User User { get; set; }
}