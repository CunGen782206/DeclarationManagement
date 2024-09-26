using AutoMapper;
using DeclarationManagement;
using DeclarationManagement.Model;
using DeclarationManagement.Model.DTO;
using Microsoft.AspNetCore.Mvc;

/// <summary> 申请表单控制 </summary>
[ApiController]
[Route("api/[controller]")] //api/ApplicationForms
public class ApplicationFormsController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public ApplicationFormsController(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    #region 首次添加表单
    // POST: api/ApplicationForms/addForm
    [HttpPost("/addForm")] //添加表单
    public async Task<ActionResult> CreateForm([FromBody] ApplicationFormDTO modelDTO)
    {
        var record = _mapper.Map<ApplicationForm>(modelDTO);
        var user = _context.Users.FirstOrDefault(user => user.UserID == modelDTO.UserID);
        //相互绑定
        user.ApplicationForms.Add(record);
        record.User = user;
        _context.ApplicationForms.Add(record); //向表中自动进行推送。
        await _context.SaveChangesAsync();

        //获得下一个审批人
        var nextUser = ApprovalOne(record.User.Role, nameof(Power.预审用户));
        
        // 推送到下一个审批人的汇总表
        if (nextUser != null)
            PushNextTableSummary(record, nextUser);

        return Ok(user);
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
                User = _context.Users.FirstOrDefault(user => user.UserID == user.UserID), //查找ID
                Decision = 0, //未进行审核(可以放置到表格自动初始化中)
                ApplicationForm = applicationForm
            });
            await _context.SaveChangesAsync();
        }
    }

    /// <summary> 预审核 </summary>
    private User ApprovalOne(string role, string power)
    {
        return _context.Users.FirstOrDefault(f => (f.Role == role) && (f.Power == power));
    }

    #endregion

    #region 修改表单

    #endregion

    #region 查看表单

    #endregion


    // GET: api/ApplicationForms
    [HttpGet]
    public IActionResult GetUserForms()
    {
        int userId = GetCurrentUserId();

        // var forms = _context.ApplicationForms
        //     .Where(f => f.UserID == userId)
        //     .OrderBy(f => f.FormID)
        //     .Select(f => new
        //     {
        //         f.FormID,
        //         f.ProjectName,
        //         ButtonLabel = !f.State ? "修改" : "查看",
        //         Status = !f.ApprovalEnding && !f.State ? "已驳回" :
        //             f.ApprovalEnding ? "审批通过" : "待审批"
        //     })
        //     .ToList();

        return Ok();
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
                    // UserID = nextApprover.UserID,
                    // ApplicantID = form.FormID,
                    // State = false,
                    // ApprovalEnding = false
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