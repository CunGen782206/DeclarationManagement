using AutoMapper;
using DeclarationManagement;
using DeclarationManagement.Model;
using DeclarationManagement.Model.DTO;
using Microsoft.AspNetCore.Mvc;

/// <summary> 申请控制 </summary>
[ApiController]
[Route("api/[controller]")]
public class ApprovalsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public ApprovalsController(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    #region 用户审核

    /// <summary> 用户审核（生成新的审批记录表，推送新的审批表汇总） </summary>
    /// <param name="id"> 审核人id（根据审核人判断是预审还是终审状态） </param>
    /// <param name="modelDTO"> 审核组合表单 </param>
    /// <returns> 返回当前所有的TableSummaries表单 </returns> //TODO:是否需要改成单个表单返回，他直接添加刷新
    [HttpPost("/approvalForm")] //审核表单
    public async Task<ActionResult> ApprovalForm([FromBody] ApprovalCombineDTO approvalCombineDTO)
    {
        var approvalUser = _context.Users.SingleOrDefault(u =>
            u.UserID == approvalCombineDTO.UserID); //当前审核用户
        switch (approvalUser.Power)
        {
            case nameof(Power.预审用户):
                await Audition(approvalCombineDTO, approvalUser);
                break;
            case nameof(Power.初审用户):
                await FirstTrial(approvalCombineDTO, approvalUser);
                break;
        }

        //返回所有审核表单
        return Ok();
    }

    #endregion

    #region 预审用户审核

    /// <summary>
    /// 预审用户审核
    /// </summary>
    /// <param name="approvalCombineDTO"></param>
    /// <param name="approvalUser"> 审核的用户 </param>
    private async Task Audition(ApprovalCombineDTO approvalCombineDTO, User approvalUser)
    {
        //查找对应的审核表单
        var applicationForm = _context.ApplicationForms.SingleOrDefault(t =>
            t.ApplicationFormID == approvalCombineDTO.applicationFormID);

        //修改当前审批表（相当于修改状态）
        await AmendTableSummary(approvalCombineDTO);
        
        //创建新的审批记录表
        await CreateApprovalRecords(approvalCombineDTO, applicationForm, approvalUser);

        if (approvalCombineDTO.Decision == 2)
        {
            applicationForm.Decision = 2; //打回给原客户
            await _context.SaveChangesAsync();
        }
        else if (approvalCombineDTO.Decision == 3)
        {
            applicationForm.Decision = 3; //审批不通过
            applicationForm.AuditDepartment = approvalUser.Role;
            applicationForm.Comments = approvalCombineDTO.Comments;
            await _context.SaveChangesAsync();
        }
        else
        {
            //查找下一级User
            applicationForm.Decision = 0; 
            await _context.SaveChangesAsync();
            var nextUser = ApprovalTwo(applicationForm.ProjectCategory); //查找下一个层级
            await PushNextTableSummary(applicationForm, nextUser);
        }
    }


    /// <summary>
    /// 修改当前审批表汇总的表格
    /// </summary>
    private async Task AmendTableSummary(ApprovalCombineDTO approvalCombineDTO)
    {
        //查找审核表
        var currentTableSummary =
            _context.TableSummaries.SingleOrDefault(t => t.TableSummaryID == approvalCombineDTO.TableSummaryID);
        //修改审核表内容
        currentTableSummary.Decision = approvalCombineDTO.Decision;
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// 创建新的审批记录表
    /// </summary>
    /// <param name="approvalCombineDTO"></param>
    private async Task CreateApprovalRecords(ApprovalCombineDTO approvalCombineDTO, ApplicationForm applicationForm,
        User approvalUser)
    {
        var newApprovalRecord = new ApprovalRecord()
        {
            ApplicationFormID = applicationForm.ApplicationFormID,
            UserID = approvalUser.UserID,
            Decision = approvalCombineDTO.Decision,
            Comments = approvalCombineDTO.Comments,
            ApprovalDate = DateTime.Now,
            ApplicationForm = applicationForm,
            User = approvalUser
        };
        _context.ApprovalRecords.Add(newApprovalRecord);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// 推送给下一个用户
    /// </summary>
    /// <param name="applicationForm"> 推送表单 </param>
    /// <param name="nextUser"> 推送用户 </param>
    private async Task PushNextTableSummary(ApplicationForm applicationForm, User nextUser)
    {
        if (nextUser != null)
        {
            _context.TableSummaries.Add(new TableSummary
            {
                UserID = nextUser.UserID,
                ApplicationFormID = applicationForm.ApplicationFormID,
                User = _context.Users.SingleOrDefault(user => user.UserID == nextUser.UserID), //查找ID(绑定关系)
                Decision = 0, //未进行审核(可以放置到表格自动初始化中)
                ApplicationForm = applicationForm
            });
            await _context.SaveChangesAsync();
        }
    }

    #endregion

    #region 初审用户审核

    /// <summary>
    /// 初审用户
    /// </summary>
    /// <param name="approvalCombineDTO">  </param>
    /// <param name="approvalUser">  </param>
    private async Task FirstTrial(ApprovalCombineDTO approvalCombineDTO, User approvalUser)
    {
        //查找对应的审核表单
        var applicationForm = _context.ApplicationForms.SingleOrDefault(t =>
            t.ApplicationFormID == approvalCombineDTO.applicationFormID);

        //修改当前审批表（相当于修改状态）
        await AmendTableSummary(approvalCombineDTO);
        //创建新的审批记录表
        await CreateApprovalRecords(approvalCombineDTO, applicationForm, approvalUser);
        applicationForm.Decision = approvalCombineDTO.Decision;
        await _context.SaveChangesAsync();
        if (approvalCombineDTO.Decision == 3 || approvalCombineDTO.Decision == 1) //通过或者不通过进行的操作
        {
            await AmendApplicationFormDTO(approvalCombineDTO, applicationForm, approvalUser);
        }
    }

    /// <summary>
    /// 修改申请表单
    /// </summary>
    private async Task AmendApplicationFormDTO(ApprovalCombineDTO approvalCombineDTO, ApplicationForm applicationForm,
        User approvalUser)
    {
        applicationForm.AuditDepartment = approvalUser.Role;
        applicationForm.Comments = approvalCombineDTO.Comments;
        applicationForm.RecognitionLevel = approvalCombineDTO.RecognitionLevel;
        applicationForm.DeemedAmount = approvalCombineDTO.DeemedAmount;
        applicationForm.Remarks = approvalCombineDTO.Remarks;
        await _context.SaveChangesAsync();
    }
    
    #endregion


    
    /// <summary>
    /// 查找下一个需要推送的表单
    /// </summary>
    /// <param name="projectCategory"> 项目类别 </param>
    /// <param name="role"> 当前用户的角色 </param>
    /// <param name="power"> 终审用户 </param>
    /// <returns></returns>
    private User ApprovalTwo(string projectCategory)
    {
        switch (projectCategory)
        {
            case nameof(ProjectCategory.师资建设类):
                //TODO:组织人事处
                return _context.Users.FirstOrDefault(f => (f.Role == "组织人事处") && (f.Power == nameof(Power.初审用户)));
            case nameof(ProjectCategory.教学成果类):
                //TODO:科研处
                return _context.Users.FirstOrDefault(f => (f.Role == "科研处") && (f.Power == nameof(Power.初审用户)));
            // case nameof(ProjectCategory.专业建设类):
            // case nameof(ProjectCategory.课程建设类):
            // case nameof(ProjectCategory.教学竞赛类):
            // case nameof(ProjectCategory.教材建设类):
            default:
                //TODO:教务处
                return _context.Users.FirstOrDefault(f => (f.Role == "教务处") && (f.Power == nameof(Power.初审用户)));
        }
    }
    
}