using DeclarationManagement.Model;

namespace DeclarationManagement.View;

/// <summary> 展示页展示数据，返回所需要的展示数据 </summary>
public class CommonDatas
{
    /// <summary>
    /// 表单ID 普通用户时ApplicationForm，，初审和终审则是TableSummary
    /// </summary>
    public int FormID { get; set; } //表单ID

    /// <summary>
    /// 项目负责人（可修改）
    /// </summary>
    public string ProjectLeader { get; set; }

    /// <summary>
    /// 所属部门（可修改）
    /// </summary>
    public string Department { get; set; }

    /// <summary>
    /// 项目名称（可修改）
    /// </summary>
    public string ProjectName { get; set; }

    /// <summary>
    /// 项目类别（可修改）
    /// </summary>
    public string ProjectCategory { get; set; }

    /// <summary>
    /// 项目等级（可修改）
    /// </summary>
    public string ProjectLevel { get; set; }

    /// <summary>
    /// 申请时间（一次记录）
    /// </summary>
    public DateTime ApprovalDate { get; set; }

    /// <summary>
    /// 审批决定（0未审核，1拟同意，2拟不同意，3不同意）
    /// </summary>
    public int Decision { get; set; }

    /// <summary>
    /// 普通用户的展示页数据
    /// </summary>
    /// <param name="applicationForm"></param>
    public CommonDatas(ApplicationForm applicationForm)
    {
        FormID = applicationForm.ApplicationFormID;
        ProjectLeader = applicationForm.ProjectLeader;
        Department = applicationForm.Department;
        ProjectName = applicationForm.ProjectName;
        ProjectCategory = applicationForm.ProjectCategory;
        ProjectLevel = applicationForm.ProjectLevel;
        ApprovalDate = applicationForm.ApprovalDate;
        Decision = applicationForm.Decision;
    }
    
    /// <summary>
    /// 审核用户的
    /// </summary>
    /// <param name="applicationForm"></param>
    /// <param name="tableSummary"></param>
    public CommonDatas(ApplicationForm applicationForm,TableSummary tableSummary)
    {
        FormID = tableSummary.TableSummaryID;
        ProjectLeader = applicationForm.ProjectLeader;
        Department = applicationForm.Department;
        ProjectName = applicationForm.ProjectName;
        ProjectCategory = applicationForm.ProjectCategory;
        ProjectLevel = applicationForm.ProjectLevel;
        ApprovalDate = applicationForm.ApprovalDate;
        Decision = tableSummary.Decision;//当前操作
    }
}