using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace DeclarationManagement.Model;

public class User
{
    /// <summary> 用户ID </summary>
    public int UserID { get; set; }

    /// <summary> 用户登录名称 </summary>
    public string Username { get; set; } = "";

    /// <summary> 密码 </summary>
    public string Password { get; set; } = "";
    
    /// <summary>
    /// Email地址
    /// </summary>
    public string Email { get; set; } = "";
    
    /// <summary>
    /// 用户名
    /// </summary>
    public string Name { get; set; } = "";

    /// <summary> 角色 </summary>
    public string Role { get; set; } = "";

    /// <summary> 权限 </summary>
    public string Power { get; set; } = "";

    // 导航属性
    // /// <summary> 申请表单（一个用户有多个申请表单） </summary>
    public ICollection<ApplicationForm> ApplicationForms { get; set; } = new List<ApplicationForm>();
    public ICollection<ApprovalRecord> ApprovalRecords { get; set; } = new List<ApprovalRecord>();
    public ICollection<TableSummary> TableSummaries { get; set; } = new List<TableSummary>();
}

/// <summary>
/// 用户权限
/// </summary>
public enum Power
{
    普通用户,
    预审用户,
    初审用户
}