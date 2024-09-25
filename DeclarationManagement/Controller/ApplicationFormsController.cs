using DeclarationManagement;
using DeclarationManagement.Model;
using Microsoft.AspNetCore.Mvc;

/// <summary> 申请表单控制 </summary>
[ApiController]
[Route("api/[controller]")]
public class ApplicationFormsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public ApplicationFormsController(ApplicationDbContext context)
    {
        _context = context;
    }

    #region 添加表单
    // POST: api/ApplicationForms
    [HttpPost]
    public IActionResult CreateForm([FromBody] ApplicationForm model)
    {
        // int userId = GetCurrentUserId();
        // model.UserID = userId;
        //用户ID通过包体传送给我
        model.State = false;
        model.ApprovalEnding = false;
        model.ApprovalDate = DateTime.Now;

        _context.ApplicationForms.Add(model);//向表中自动进行推送。
        _context.SaveChanges();

        // 推送到下一个审批人的汇总表
        var nextApprover = GetNextApprover();
        if (nextApprover != null)
        {
            _context.TableSummaries.Add(new TableSummary
            {
                UserID = nextApprover.UserID,
                ApplicantID = model.FormID,
                State = false,
                ApprovalEnding = false
            });
            _context.SaveChanges();
        }

        return Ok(model);
    }
    #endregion
    
    // GET: api/ApplicationForms
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
                ButtonLabel = !f.State ? "修改" : "查看",
                Status = !f.ApprovalEnding && !f.State ? "已驳回" :
                    f.ApprovalEnding ? "审批通过" : "待审批"
            })
            .ToList();

        return Ok(forms);
    }

 

    // PUT: api/ApplicationForms/{id}
    [HttpPut("{id}")]
    public IActionResult UpdateForm(int id, [FromBody] ApplicationForm model)
    {
        var form = _context.ApplicationForms.Find(id);
        if (form == null)
        {
            return NotFound();
        }

        if (!form.State)
        {
            // 根据需要更新表单字段
            form.ProjectName = model.ProjectName;
            // ... 更新其他字段

            form.State = true; // 修改后设置为“查看”状态

            // 推送到下一个审批人的汇总表
            var nextApprover = GetNextApprover();
            if (nextApprover != null)
            {
                _context.TableSummaries.Add(new TableSummary
                {
                    UserID = nextApprover.UserID,
                    ApplicantID = form.FormID,
                    State = false,
                    ApprovalEnding = false
                });
            }

            _context.SaveChanges();
            return Ok(form);
        }
        else
        {
            return BadRequest("当前状态下无法修改表单。");
        }
    }

    private int GetCurrentUserId()
    {
        // 实现获取当前用户 ID 的逻辑
        return 1; // 占位符值
    }

    /// <summary> 下一个层级进行审批 </summary>
    /// <returns>  </returns>
    /// string role,string power
    private User GetNextApprover()
    {
        // 根据您的审批流程实现获取下一个审批人的逻辑
        return _context.Users.FirstOrDefault(u => u.Power == "审批用户");
    }
}