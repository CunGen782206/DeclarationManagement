using AutoMapper;
using DeclarationManagement;
using DeclarationManagement.Model;
using DeclarationManagement.Model.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
    public async Task<ActionResult> ApprovalForm([FromBody] ApprovalCombineModel approvalCombineModel)
    {
        if (approvalCombineModel == null)
        {
            return BadRequest("请求数据无效。");
        }

        var approvalUser = await _context.Users.SingleOrDefaultAsync(u =>
            u.UserID == approvalCombineModel.UserID); //当前审核用户

        if (approvalUser == null)
        {
            return NotFound("审核用户不存在。");
        }

        var applicationForm =
            await _context.ApplicationForms.SingleOrDefaultAsync(
                a => a.ApplicationFormID == approvalCombineModel.applicationFormID);

        if (applicationForm == null)
        {
            return NotFound("申请表单不存在。");
        }

        switch (applicationForm.States)
        {
            case 0: //进入预审
                await Audition(approvalCombineModel, approvalUser, applicationForm);
                break;
            case 1: //进入初审
                await FirstTrial(approvalCombineModel, approvalUser, applicationForm);
                break;
            default:
                return BadRequest("无效的申请表单状态。");
        }

        //返回所有审核表单
        return Ok();
    }

    #endregion

    #region 预审用户审核

    /// <summary>
    /// 预审用户审核
    /// </summary>
    /// <param name="approvalCombineModel">  </param>
    /// <param name="approvalUser"> 审核的用户 </param>
    private async Task Audition(ApprovalCombineModel approvalCombineModel, User approvalUser,
        ApplicationForm applicationForm)
    {
        //查找对应的审核表单
        //修改当前审批表（相当于修改状态）
        var result = await AmendTableSummary(approvalCombineModel);

        if (!result) return;

        //创建新的审批记录表
        await CreateApprovalRecords(approvalCombineModel, applicationForm, approvalUser);
        if (approvalCombineModel.Decision == 2)
        {
            applicationForm.Decision = 2; //打回给原客户
        }
        else if (approvalCombineModel.Decision == 3)
        {
            applicationForm.Decision = 3; //审批不通过
            applicationForm.AuditDepartment = approvalUser.Role; //审核人
            applicationForm.Comments = approvalCombineModel.Comments;
            applicationForm.States = 1;
        }
        else
        {
            //查找下一级User
            applicationForm.Decision = 0;
            applicationForm.States = 1;
            var nextUser = ApprovalTwo(applicationForm.ProjectCategory); //查找下一个层级
            // 推送到下一个审批人的汇总表
            if (nextUser != null)
            {
                await PushNextTableSummary(applicationForm, nextUser);
            }
        }

        await _context.SaveChangesAsync();
    }


    /// <summary>
    /// 修改当前审批表汇总的表格
    /// </summary>
    private async Task<bool> AmendTableSummary(ApprovalCombineModel approvalCombineModel)
    {
        //查找审核表
        var currentTableSummary = await
            _context.TableSummaries.SingleOrDefaultAsync(t => t.TableSummaryID == approvalCombineModel.TableSummaryID);
        if (currentTableSummary == null)
        {
            ModelState.AddModelError(string.Empty, "未找到对应的汇总表记录。");
            return false;
        }

        //修改审核表内容
        currentTableSummary.Decision = approvalCombineModel.Decision;
        await _context.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// 创建新的审批记录表
    /// </summary>
    /// <param name="approvalCombineModel"></param>
    private async Task CreateApprovalRecords(ApprovalCombineModel approvalCombineModel,
        ApplicationForm applicationForm,
        User approvalUser)
    {
        var newApprovalRecord = new ApprovalRecord()
        {
            ApplicationFormID = applicationForm.ApplicationFormID,
            UserID = approvalUser.UserID,
            Decision = approvalCombineModel.Decision,
            Comments = approvalCombineModel.Comments,
            ApprovalDate = DateTime.Now,
            ApplicationForm = applicationForm,
            User = approvalUser
        };
        await _context.ApprovalRecords.AddAsync(newApprovalRecord);
        await _context.SaveChangesAsync();
        applicationForm.ApprovalRecords.Add(newApprovalRecord);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// 推送给下一个用户
    /// </summary>
    /// <param name="applicationForm"> 推送表单 </param>
    /// <param name="nextUser"> 推送用户 </param>
    private async Task PushNextTableSummary(ApplicationForm applicationForm, User nextUser)
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

    #endregion

    #region 初审用户审核

    /// <summary>
    /// 初审用户
    /// </summary>
    /// <param name="approvalCombineModel">  </param>
    /// <param name="approvalUser">  </param>
    private async Task FirstTrial(ApprovalCombineModel approvalCombineModel, User approvalUser,
        ApplicationForm applicationForm)
    {
        //修改当前审批表（相当于修改状态）
        var result = await AmendTableSummary(approvalCombineModel);
        if (!result) return;

        //创建新的审批记录表
        await CreateApprovalRecords(approvalCombineModel, applicationForm, approvalUser);
        applicationForm.Decision = approvalCombineModel.Decision;
        applicationForm.States = 2;

        if (approvalCombineModel.Decision == 3 || approvalCombineModel.Decision == 1) //通过或者不通过进行的操作
        {
            await AmendApplicationFormDTO(approvalCombineModel, applicationForm, approvalUser);
        }

        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// 修改申请表单
    /// </summary>
    private async Task AmendApplicationFormDTO(ApprovalCombineModel approvalCombineModel,
        ApplicationForm applicationForm,
        User approvalUser)
    {
        applicationForm.AuditDepartment = approvalUser.Role;
        applicationForm.Comments = approvalCombineModel.Comments;
        applicationForm.RecognitionProjectLevel = approvalCombineModel.RecognitionProjectLevel;
        applicationForm.RecognitionAwardLevel = approvalCombineModel.RecognitionAwardLevel;
        applicationForm.DeemedAmount = approvalCombineModel.DeemedAmount;
        applicationForm.Remarks = approvalCombineModel.Remarks;
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
                return _context.Users.FirstOrDefault(f => (f.Role == "组织人事处") && (f.Power == nameof(Power.审核用户)));
            case nameof(ProjectCategory.教学成果类):
                return _context.Users.FirstOrDefault(f => (f.Role == "科研处") && (f.Power == nameof(Power.审核用户)));
            // case nameof(ProjectCategory.专业建设类):
            // case nameof(ProjectCategory.课程建设类):
            // case nameof(ProjectCategory.教学竞赛类):
            // case nameof(ProjectCategory.教材建设类):
            default:
                return _context.Users.FirstOrDefault(f => (f.Role == "教务处") && (f.Power == nameof(Power.审核用户)));
        }
    }
}