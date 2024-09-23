using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DeclarationManagement.Model;


public class User
{
    /// <summary> 用户ID </summary>
    public int UserID { get; set; }
    
    /// <summary> 用户名称 </summary>
    public string Username { get; set; }
    
    /// <summary> 密码 </summary>
    public byte[] Password { get; set; }
    
    /// <summary> 角色 </summary>
    public string Role { get; set; }
    
    /// <summary> 权限 </summary>
    public string Power { get; set; }

    // 导航属性
    /// <summary> 申请表单（一个用户有多个申请表单） </summary>
    public ICollection<ApplicationForm> ApplicationForms { get; set; }
    public ICollection<ApprovalRecord> ApprovalRecords { get; set; }
    public ICollection<TableSummary> TableSummaries { get; set; }
    
}