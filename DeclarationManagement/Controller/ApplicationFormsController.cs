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

        return NoContent();//请求成功但不返回内容
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
    // POST: api/ApplicationForms/alterForm
    [HttpPut("/alterForm")] //修改表单
    public async Task<ActionResult> AlterForm([FromBody] ApplicationFormDTO modelDTO)
    {
        
        var record = _mapper.Map<ApplicationForm>(modelDTO);
        var applicationForm = _context.ApplicationForms.FirstOrDefault(form =>  form.ApplicationFormID == modelDTO.ApplicationFormID);
        
        _mapper.Map(modelDTO, applicationForm);//将表单进行数据替换
        
        //TODO:清空其他审核过程中的数据
        
        await _context.SaveChangesAsync();
        
        //获得下一个审批人
        var nextUser = ApprovalOne(record.User.Role, nameof(Power.预审用户));
        
        // 推送到下一个审批人的汇总表
        if (nextUser != null)
            PushNextTableSummary(record, nextUser);
        
        return NoContent();//请求成功但不返回内容
    }
    #endregion

    #region 查看表单
    
    // GET: api/ApplicationForms/getForm
    [HttpGet("/getForm")] //传入表格ID（获得表单）
    public async Task<ActionResult> GetForms(int ApplicationFormID)
    {
        var applicationForm = _context.ApplicationForms.FirstOrDefault(form =>  form.ApplicationFormID == ApplicationFormID);
 
        return Ok(applicationForm);
    }

    #endregion
}