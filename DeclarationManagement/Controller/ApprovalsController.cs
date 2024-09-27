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
    [HttpPost("/approvalForm")] //添加表单
    public async Task<ActionResult> ApprovalForm([FromBody] ApprovalCombineDTO approvalCombineDTO)
    {
        var approvalUser = _context.Users.SingleOrDefault(u =>
            u.UserID == approvalCombineDTO.UserID); //当前审核用户
        switch (approvalUser.Power)
        {
            case nameof(Power.预审用户):
                Audition(approvalCombineDTO);
                break;
            case nameof(Power.初审用户):
                FirstTrial(approvalCombineDTO);
                break;
        }


        return Ok();
    }

    #endregion

    #region 预审用户审核

    private async void Audition(ApprovalCombineDTO approvalCombineDTO)
    {
        var tableSummary =
            _context.TableSummaries.SingleOrDefault(t => t.TableSummaryID == approvalCombineDTO.TableSummaryID);
        tableSummary.Decision = approvalCombineDTO.Decision;
        await _context.SaveChangesAsync();
        var form = _context.ApplicationForms.SingleOrDefault(t =>
            t.ApplicationFormID == approvalCombineDTO.applicationFormID);
        var nextUser = ApprovalTwo(form.ProjectCategory);
        //修改当前审批表
        //生成新的审批记录表
        //生成新的审批汇总表
    }

    /// <summary>
    /// 推送给下一个用户
    /// </summary>
    /// <param name="applicationForm"> 推送表单 </param>
    /// <param name="user"> 推送用户 </param>
    private async void PushNextTableSummary(ApplicationForm applicationForm, User user)
    {
        if (user != null)
        {
            _context.TableSummaries.Add(new TableSummary
            {
                UserID = user.UserID,
                ApplicationFormID = applicationForm.ApplicationFormID,
                User = _context.Users.SingleOrDefault(user => user.UserID == user.UserID), //查找ID
                Decision = 0, //未进行审核(可以放置到表格自动初始化中)
                ApplicationForm = applicationForm
            });
            await _context.SaveChangesAsync();
        }
    }

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

    #endregion

    #region 初审用户审核

    private void FirstTrial(ApprovalCombineDTO approvalCombineDTO)
    {
        //生成新的审批汇总表
        //修改当前审批表
    }

    #endregion

    #region Other

    // GET: api/Approvals
    [HttpGet]
    public IActionResult GetPendingApprovals()
    {
        // int userId = GetCurrentUserId();
        //
        // var approvals = _context.TableSummaries
        //     .Where(ts => ts.UserID == userId)
        //     .OrderBy(ts => ts.SummaryID)
        //     .Select(ts => new
        //     {
        //         ts.ApplicantID,
        //         ButtonLabel = !ts.State ? "审核" : "查看",
        //         Status = !ts.ApprovalEnding && !ts.State ? "已驳回" :
        //             ts.ApprovalEnding ? "已审核" : "待审核"
        //     })
        //     .ToList();

        return Ok();
    }

    // POST: api/Approvals/{id}/review
    [HttpPost("{id}/review")]
    public IActionResult ReviewForm(int id)
    {
        // int userId = GetCurrentUserId();
        //
        // var tableSummary = _context.TableSummaries
        //     .FirstOrDefault(ts => ts.ApplicantID == id && ts.UserID == userId);
        //
        // if (tableSummary == null)
        // {
        //     return NotFound("未找到需要审批的表单。");
        // }
        //
        // var form = _context.ApplicationForms.Find(id);
        // if (form == null)
        // {
        //     return NotFound("申请表单不存在。");
        // }
        //
        // // 添加审批记录
        // var approvalRecord = new ApprovalRecord
        // {
        //     ApplicantID = id,
        //     ApproverID = userId,
        //     ApprovalDate = DateTime.Now,
        //     Decision = model.Decision,
        //     Comments = model.Comments
        // };
        // _context.ApprovalRecords.Add(approvalRecord);
        //
        // if (model.Decision)
        // {
        //     // 审批通过
        //     var nextApprover = GetNextApprover();
        //     if (nextApprover != null)
        //     {
        //         _context.TableSummaries.Add(new TableSummary
        //         {
        //             UserID = nextApprover.UserID,
        //             ApplicantID = id,
        //             State = false,
        //             ApprovalEnding = false
        //         });
        //     }
        //     else
        //     {
        //         form.ApprovalEnding = true; // 标记审批已完成
        //     }
        // }
        // else
        // {
        //     // 审批拒绝
        //     form.ApprovalEnding = false;
        //     form.State = false;
        // }
        //
        // // 更新审批汇总表
        // tableSummary.State = true;
        // tableSummary.ApprovalEnding = model.Decision;
        //
        // _context.SaveChanges();
        return Ok("审批提交成功。");
    }

    private int GetCurrentUserId()
    {
        // 实现获取当前审批人用户 ID 的逻辑
        return 2; // 占位符值
    }

    private User GetNextApprover()
    {
        // 实现获取下一个审批人的逻辑
        return _context.Users.FirstOrDefault(u => u.Power == "审批用户" && u.UserID != GetCurrentUserId());
    }

    #endregion
}