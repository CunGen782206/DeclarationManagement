using System.Security.Claims;
using DeclarationManagement.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DeclarationManagement.Controller;

   [ApiController]
    [Route("api/[controller]")]
    public class ApprovalController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ApprovalController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 获取当前审批用户的所有待审批表单
        [HttpGet("PendingForms")]
        public async Task<IActionResult> GetPendingForms()
        {
            var userId = GetCurrentUserId();
            var summaries = await _context.TableSummaries
                .Include(s => s.ApplicationForm)
                .Where(s => s.UserID == userId && s.HasOperated == false)
                .OrderBy(s => s.SummaryID)
                .ToListAsync();

            return Ok(summaries);
        }

        // 审批表单
        [HttpPost("Approve/{formId}")]
        public async Task<IActionResult> ApproveForm(int formId, [FromBody] ApprovalModel model)
        {
            var userId = GetCurrentUserId();
            var form = await _context.ApplicationForms.FindAsync(formId);

            if (form == null)
                return NotFound(new { message = "表单不存在" });

            // 添加审批记录
            var record = new ApprovalRecord
            {
                FormID = formId,
                ApproverID = userId,
                ApprovalDate = DateTime.Now,
                Decision = model.Decision,
                Comments = model.Comments
            };
            _context.ApprovalRecords.Add(record);

            // 更新审批表汇总
            var summary = await _context.TableSummaries
                .FirstOrDefaultAsync(s => s.FormID == formId && s.UserID == userId);

            if (summary != null)
            {
                summary.HasOperated = true;
                summary.ApprovalResult = model.Decision;
                summary.ApprovalEnding = !model.Decision;
            }

            if (model.Decision)
            {
                // 审批通过，推送到下一个审批人
                var nextApprover = await GetNextApproverAsync(userId);
                if (nextApprover != null)
                {
                    var nextSummary = new TableSummary
                    {
                        UserID = nextApprover.UserID,
                        FormID = formId,
                        HasOperated = false,
                        ApprovalEnding = false
                    };
                    _context.TableSummaries.Add(nextSummary);
                }
                else
                {
                    // 审批流程结束
                    form.ApprovalEnding = true;
                }
            }
            else
            {
                // 审批未通过，更新表单状态
                form.State = false;
                form.ApprovalEnding = false;
            }

            await _context.SaveChangesAsync();

            return Ok(new { message = "审批完成" });
        }

        private int GetCurrentUserId()
        {
            // 实现获取当前登录用户ID的逻辑
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return int.Parse(userIdClaim.Value);
        }

        private async Task<User> GetNextApproverAsync(int currentApproverId)
        {
            // 实现获取下一个审批人的逻辑
            var currentUser = await _context.Users.FindAsync(currentApproverId);
            var currentFlow = await _context.ApprovalFlows.FirstOrDefaultAsync(f => f.Role == currentUser.Role);
            // var nextFlow = await _context.ApprovalFlows.FirstOrDefaultAsync(f => f.StepNumber == currentFlow.NextStepNumber);
            //
            // if (nextFlow != null)
            // {
            //     return await _context.Users.FirstOrDefaultAsync(u => u.Role == nextFlow.Role);
            // }

            return null;
        }
    }

    // 审批模型
    public class ApprovalModel
    {
        public bool Decision { get; set; } // true: 同意，false: 拒绝
        public string Comments { get; set; }
    }