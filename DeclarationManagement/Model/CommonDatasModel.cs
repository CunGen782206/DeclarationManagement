using DeclarationManagement.Model;

namespace DeclarationManagement.Model;

/// <summary> 展示页展示数据，返回所需要的展示数据 </summary>
public class CommonDatasModel
{
    /// <summary>
    /// 表单ID 普通用户时ApplicationForm，，初审和终审则是TableSummary
    /// </summary>
    public int FormID { get; set; } //表单ID

    /// <summary>
    /// 项目负责人（可修改）
    /// </summary>
    public string ProjectLeader { get; set; } = "";

    /// <summary>
    /// 所属部门（可修改）
    /// </summary>
    public string Department { get; set; } = "";

    /// <summary>
    /// 项目名称（可修改）
    /// </summary>
    public string ProjectName { get; set; } = "";

    /// <summary>
    /// 项目类别（可修改）
    /// </summary>
    public string ProjectCategory { get; set; } = "";

    /// <summary>
    /// 项目等级（可修改）
    /// </summary>
    public string ProjectLevel { get; set; } = "";

    /// <summary>
    /// 申请时间（一次记录）
    /// </summary>
    public DateTime ApprovalDate { get; set; }

    /// <summary>
    /// 审批决定（0未审核，1拟同意，2拟不同意，3不同意）
    /// </summary>
    public int Decision { get; set; }

    #region 新加字段

    /// <summary>
    /// 奖项级别（可修改）
    /// </summary>
    public string AwardLevel { get; set; } = "";

    /// <summary>
    /// 参与形式（可修改）
    /// </summary>
    public string ParticipationForm { get; set; } = "";

    #endregion

    /// <summary>
    /// 普通用户的展示页数据
    /// </summary>
    /// <param name="applicationForm"></param>
    public CommonDatasModel(ApplicationForm applicationForm)
    {
        FormID = applicationForm.ApplicationFormID;
        ProjectLeader = applicationForm.ProjectLeader;
        Department = applicationForm.Department;
        ProjectName = applicationForm.ProjectName;
        ProjectCategory = applicationForm.ProjectCategory;
        ProjectLevel = applicationForm.ProjectLevel;
        ApprovalDate = applicationForm.ApprovalDate;
        AwardLevel = applicationForm.AwardLevel;
        ParticipationForm = applicationForm.ParticipationForm;
        Decision = applicationForm.Decision;
    }

    /// <summary>
    /// 审核用户的
    /// </summary>
    /// <param name="applicationForm"></param>
    /// <param name="tableSummary"></param>
    public CommonDatasModel(ApplicationForm applicationForm, TableSummary tableSummary)
    {
        FormID = tableSummary.TableSummaryID;
        ProjectLeader = applicationForm.ProjectLeader;
        Department = applicationForm.Department;
        ProjectName = applicationForm.ProjectName;
        ProjectCategory = applicationForm.ProjectCategory;
        ProjectLevel = applicationForm.ProjectLevel;
        ApprovalDate = applicationForm.ApprovalDate;
        AwardLevel = applicationForm.AwardLevel;
        ParticipationForm = applicationForm.ParticipationForm;
        Decision = tableSummary.Decision; //当前操作
    }

    //TODO 修改当前状态 
    //审核状态：

    #region 新加方法

    /// <summary> 设置默认 </summary>
    public void DecisionDefault()
    {
    }

    /// <summary> 设置审核人员 </summary>
    public void DecisionOther()
    {
    }

    #endregion
}