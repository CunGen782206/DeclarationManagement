using DeclarationManagement;
using DeclarationManagement.Model;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ApplicationFormsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    public ApplicationFormsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // 获取当前用户的所有表单
    [HttpGet]
    public IActionResult GetUserForms()
    {
        int userId = GetCurrentUserId();

        var forms = _context.ApplicationForms
            .Where(f => f.UserID == userId)
            .OrderBy(f => f.FormID)
            .Select(f => new
            {
                f.FormID,
                f.ProjectName,
                ButtonLabel = f.State ? "查看" : "修改",
                Status = f.ApprovalEnding == false && f.State == false ? "已驳回" :
                         f.ApprovalEnding == true ? "审批通过" : "待审批"
            })
            .ToList();

        return Ok(forms);
    }

    // 查看表单详情
    [HttpGet("{id}")]
    public IActionResult GetForm(int id)
    {
        var form = _context.ApplicationForms.Find(id);
        if (form == null)
        {
            return NotFound();
        }
        return Ok(form);
    }

    // 修改表单
    [HttpPut("{id}")]
    public IActionResult UpdateForm(int id, [FromBody] ApplicationForm model)
    {
        var form = _context.ApplicationForms.Find(id);
        if (form == null)
        {
            return NotFound();
        }

        if (form.State == false)
        {
            // 更新表单内容
            form.ProjectName = model.ProjectName;
            // 其他字段更新
            form.State = true; // 修改后设置为可查看状态

            // 推送到下一个审核用户的审批表汇总
            var nextApprover = GetNextApprover();
            _context.TableSummaries.Add(new TableSummary
            {
                UserID = nextApprover.UserID,
                ApplicantID = form.FormID,
                State = false,
                ApprovalEnding = false
            });

            _context.SaveChanges();
            return Ok();
        }
        else
        {
            return BadRequest("表单当前不可修改");
        }
    }

    // 新建表单申请
    [HttpPost]
    public IActionResult CreateForm([FromBody] ApplicationForm model)
    {
        int userId = GetCurrentUserId();
        model.UserID = userId;
        model.State = false;
        model.ApprovalEnding = false;
        model.ApprovalDate = DateTime.Now;

        _context.ApplicationForms.Add(model);
        _context.SaveChanges();

        // 推送到下一个审核用户的审批表汇总
        var nextApprover = GetNextApprover();
        _context.TableSummaries.Add(new TableSummary
        {
            UserID = nextApprover.UserID,
            ApplicantID = model.FormID,
            State = false,
            ApprovalEnding = false
        });

        _context.SaveChanges();
        return Ok();
    }

    private int GetCurrentUserId()
    {
        // 获取当前登录用户的ID（此处省略具体实现）
        return 1;
    }

    private User GetNextApprover()
    {
        // 获取下一个审批用户（此处需要根据审批流程表实现）
        return _context.Users.FirstOrDefault(u => u.Power == "审批用户");
    }
}
