using DeclarationManagement;
using DeclarationManagement.Model;
using Microsoft.AspNetCore.Mvc;

/// <summary> 申请控制 </summary>
[ApiController]
[Route("api/[controller]")]
public class ApprovalsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    
    public ApprovalsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: api/Approvals
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
                ButtonLabel = !ts.State ? "审核" : "查看",
                Status = !ts.ApprovalEnding && !ts.State ? "已驳回" :
                    ts.ApprovalEnding ? "已审核" : "待审核"
            })
            .ToList();

        return Ok(approvals);
    }

    // POST: api/Approvals/{id}/review
    [HttpPost("{id}/review")]
    public IActionResult ReviewForm(int id, [FromBody] ReviewViewModel model)
    {
        int userId = GetCurrentUserId();

        var tableSummary = _context.TableSummaries
            .FirstOrDefault(ts => ts.ApplicantID == id && ts.UserID == userId);

        if (tableSummary == null)
        {
            return NotFound("未找到需要审批的表单。");
        }

        var form = _context.ApplicationForms.Find(id);
        if (form == null)
        {
            return NotFound("申请表单不存在。");
        }

        // 添加审批记录
        var approvalRecord = new ApprovalRecord
        {
            ApplicantID = id,
            ApproverID = userId,
            ApprovalDate = DateTime.Now,
            Decision = model.Decision,
            Comments = model.Comments
        };
        _context.ApprovalRecords.Add(approvalRecord);

        if (model.Decision)
        {
            // 审批通过
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
                form.ApprovalEnding = true; // 标记审批已完成
            }
        }
        else
        {
            // 审批拒绝
            form.ApprovalEnding = false;
            form.State = false;
        }

        // 更新审批汇总表
        tableSummary.State = true;
        tableSummary.ApprovalEnding = model.Decision;

        _context.SaveChanges();
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
}

public class ReviewViewModel
{
    public bool Decision { get; set; }
    public string Comments { get; set; }
}