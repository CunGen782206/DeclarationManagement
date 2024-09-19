using System.Security.Claims;
using DeclarationManagement.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DeclarationManagement.Controller;

 [ApiController]
    [Route("api/[controller]")]
    public class ApplicationFormController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ApplicationFormController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 获取当前用户的所有表单
        [HttpGet("MyForms")]
        public async Task<IActionResult> GetMyForms()
        {
            var userId = GetCurrentUserId();
            var forms = await _context.ApplicationForms
                .Where(f => f.UserID == userId)
                .OrderBy(f => f.FormID)
                .ToListAsync();

            return Ok(forms);
        }

        // 获取特定表单详情
        [HttpGet("Form/{id}")]
        public async Task<IActionResult> GetForm(int id)
        {
            var userId = GetCurrentUserId();
            var form = await _context.ApplicationForms
                .FirstOrDefaultAsync(f => f.FormID == id && f.UserID == userId);

            if (form == null)
                return NotFound(new { message = "表单不存在" });

            return Ok(form);
        }

        // 提交新的申请表单
        [HttpPost("Apply")]
        public async Task<IActionResult> ApplyForm([FromBody] ApplicationForm model)
        {
            model.UserID = GetCurrentUserId();
            model.ApplicationDate = DateTime.Now;
            model.State = false;
            model.ApprovalEnding = false;

            _context.ApplicationForms.Add(model);
            await _context.SaveChangesAsync();

            // 推送到下一个审核用户的审批表汇总
            var nextApprover = await GetNextApproverAsync(null);
            if (nextApprover != null)
            {
                var summary = new TableSummary
                {
                    UserID = nextApprover.UserID,
                    FormID = model.FormID,
                    HasOperated = false,
                    ApprovalEnding = false
                };
                _context.TableSummaries.Add(summary);
                await _context.SaveChangesAsync();
            }

            return Ok(new { message = "申请成功" });
        }

        // 修改表单（如果State为false）
        [HttpPut("Edit/{id}")]
        public async Task<IActionResult> EditForm(int id, [FromBody] ApplicationForm model)
        {
            var userId = GetCurrentUserId();
            var form = await _context.ApplicationForms
                .FirstOrDefaultAsync(f => f.FormID == id && f.UserID == userId);

            if (form == null)
                return NotFound(new { message = "表单不存在" });

            if (form.State == true)
                return BadRequest(new { message = "表单已提交，无法修改" });

            // 更新表单信息
            form.ProjectLeader = model.ProjectLeader;
            form.ContactWay = model.ContactWay;
            // ... 更新其他字段

            await _context.SaveChangesAsync();

            return Ok(new { message = "修改成功" });
        }

        private int GetCurrentUserId()
        {
            // 实现获取当前登录用户ID的逻辑
            // 这里假设用户ID保存在Claims中
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            return int.Parse(userIdClaim.Value);
        }

        private async Task<User> GetNextApproverAsync(int? currentApproverId)
        {
            // 实现获取下一个审批人的逻辑，根据审批流程表
            // 如果currentApproverId为null，表示获取第一个审批人
            ApprovalFlow nextFlow;

            if (currentApproverId == null)
            {
                nextFlow = await _context.ApprovalFlows.OrderBy(f => f.StepNumber).FirstOrDefaultAsync();
            }
            else
            {
                var currentUser = await _context.Users.FindAsync(currentApproverId);
                var currentFlow = await _context.ApprovalFlows.FirstOrDefaultAsync(f => f.Role == currentUser.Role);
                // nextFlow = await _context.ApprovalFlows.FirstOrDefaultAsync(f => f.StepNumber == currentFlow.NextStepNumber);
            }
            //
            // if (nextFlow != null)
            // {
            //     return await _context.Users.FirstOrDefaultAsync(u => u.Role == nextFlow.Role);
            // }

            return null;
        }
    }