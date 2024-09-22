using DeclarationManagement;
using DeclarationManagement.Model;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ApprovalsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    public ApprovalsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // 获取需要审批的表单
    [HttpGet]
    public IActionResult GetPendingApprovals()
    {
        int userId = GetCurrentUserId();

        var approvals = _context.TableSummaries
            .Where(ts => ts.UserID == userId)
            .OrderBy(ts => ts.SummaryID)
            .Select(ts => new
            {
                ts.ApplicantID,
                ButtonLabel = ts.State ? "查看" : "审核",
                Status = ts.ApprovalEnding == false && ts.State == false ? "已驳回" :
                         ts.ApprovalEnding == true ? "已审核" : "待审核"
            })
            .ToList();

        return Ok(approvals);
    }

    // 审核表单
    [HttpPost("{id}/review")]
    public IActionResult ReviewForm(int id, [FromBody] ReviewViewModel model)
    {
        int userId = GetCurrentUserId();

        var tableSummary = _context.TableSummaries
            .FirstOrDefault(ts => ts.ApplicantID == id && ts.UserID == userId);

        if (tableSummary == null)
        {
            return NotFound("未找到需要审核的表单");
        }

        var form = _context.ApplicationForms.Find(id);

        // 添加审批记录
        _context.ApprovalRecords.Add(new ApprovalRecord
        {
            ApplicantID = id,
            ApproverID = userId,
            ApprovalDate = DateTime.Now,
            Decision = model.Decision,
            Comments = model.Comments
        });

        // 更新表单状态
        if (model.Decision)
        {
            // 审批通过，推送到下一个审批人或标记为审批完成
            var nextApprover = GetNextApprover();
            if (nextApprover != null)
            {
                _context.TableSummaries.Add(new TableSummary
                {
                    UserID = nextApprover.UserID,
                    ApplicantID = id,
                    State = false,
                    ApprovalEnding = false
                });
            }
            else
            {
                form.ApprovalEnding = true;
            }
        }
        else
        {
            // 审批拒绝，标记表单为已驳回
            form.ApprovalEnding = false;
            form.State = false;
        }

        // 更新审批汇总表
        tableSummary.State = true;
        tableSummary.ApprovalEnding = model.Decision;

        _context.SaveChanges();
        return Ok();
    }

    private int GetCurrentUserId()
    {
        // 获取当前登录用户的ID（此处省略具体实现）
        return 2;
    }

    private User GetNextApprover()
    {
        // 获取下一个审批用户（此处需要根据审批流程表实现）
        return _context.Users.FirstOrDefault(u => u.Power == "审批用户" && u.UserID != GetCurrentUserId());
    }
}

public class ReviewViewModel
{
    public bool Decision { get; set; }
    public string Comments { get; set; }
}
