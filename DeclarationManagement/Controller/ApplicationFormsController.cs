using AutoMapper;
using DeclarationManagement;
using DeclarationManagement.Model;
using DeclarationManagement.Model.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        if (modelDTO == null)
        {
            return BadRequest("表单数据无效。");
        }

        var record = _mapper.Map<ApplicationForm>(modelDTO); //将DTO数据转化为非DTO数据库数据
        var user = await _context.Users.SingleOrDefaultAsync(user => user.UserID == modelDTO.UserID); //当前表单用户
        if (user == null)
        {
            return NotFound("用户不存在。");
        }

        // 绑定用户和表单
        user.ApplicationForms.Add(record);
        record.User = user;
        record.ApprovalDate = DateTime.Now;
        record.States = 0;

        // 添加表单到数据库
        await _context.ApplicationForms.AddAsync(record); //向表中自动进行推送。
        await _context.SaveChangesAsync();

        //获得下一个审批人
        var nextUser = await ApprovalOne(record.Department, nameof(Power.审核用户));

        // 推送到下一个审批人的汇总表
        if (nextUser != null)
            await PushNextTableSummary(record, nextUser);

        return Ok(); //请求成功但不返回内容 
    }

    /// <summary>
    /// 推送给下一个用户
    /// </summary>
    /// <param name="applicationForm"> 推送表单 </param>
    /// <param name="nextUser"> 推送用户 </param>
    private async Task PushNextTableSummary(ApplicationForm applicationForm, User nextUser)
    {
        await _context.TableSummaries.AddAsync(new TableSummary
        {
            UserID = nextUser.UserID,
            ApplicationFormID = applicationForm.ApplicationFormID,
            User = _context.Users.SingleOrDefault(user => user.UserID == nextUser.UserID), //查找ID(绑定关系)
            Decision = 0, //未进行审核(可以放置到表格自动初始化中)
            ApplicationForm = applicationForm
        });
        await _context.SaveChangesAsync();
    }

    /// <summary> 预审核 </summary>
    private async Task<User> ApprovalOne(string Department, string power)
    {
        return await _context.Users.FirstOrDefaultAsync(f => (f.Role == Department) && (f.Power == power));
    }

    #endregion

    #region 修改表单 //TODO:要进行修改的部分

    // POST: api/ApplicationForms/alterForm
    [HttpPut("/alterForm")] //修改表单
    public async Task<ActionResult> AlterForm([FromBody] ApplicationFormDTO modelDTO)
    {
        if (modelDTO == null)
        {
            return BadRequest("表单数据无效。");
        }

        var applicationForm =
            await _context.ApplicationForms.FirstOrDefaultAsync(form =>
                form.ApplicationFormID == modelDTO.ApplicationFormID);

        if (applicationForm == null)
        {
            return NotFound("申请表单不存在。");
        }

        _mapper.Map(modelDTO, applicationForm); //将表单进行数据替换

        //TODO:清空其他审核过程中的数据
        applicationForm.States = 0;
        await _context.SaveChangesAsync();

        //获得下一个审批人
        var nextUser = await ApprovalOne(applicationForm.Department, nameof(Power.审核用户));

        // 推送到下一个审批人的汇总表
        if (nextUser != null)
            await PushNextTableSummary(applicationForm, nextUser);

        return Ok("修改成功"); //请求成功但不返回内容
    }

    #endregion

    #region 查看单个表单

    //如果是普通用户，则Form传入的是表单ID，如果是审核用户，传入的则是TableId
    //getCode=0 则是直接查看表单 getCode=1 则是审核界面查看表单
    [HttpGet("/getForm/{getCode:int}/{FormId:int}")] //传入表格ID（获得表单）
    public async Task<ActionResult> GetForms(int getCode, int FormId)
    {
        if (getCode == 1)
        {
            var table = await _context.TableSummaries.SingleOrDefaultAsync(summary => summary.TableSummaryID == FormId);
            if (table == null)
            {
                return NotFound("汇总表记录不存在。");
            }

            FormId = table.ApplicationFormID; //获取当前表单
        }

        var applicationForm = await _context.ApplicationForms
            .Include(applicationForm => applicationForm.ApprovalRecords)
            .FirstOrDefaultAsync(form => form.ApplicationFormID == FormId); //需要通过Include加载关系
        // var applicationForm = _context.ApplicationForms.FirstOrDefault(form => form.ApplicationFormID == FormId);//如果直接这样写就会出现错误。
        if (applicationForm == null)
        {
            return NotFound("申请表单不存在。");
        }

        var applicationFormDto = _mapper.Map<ApplicationFormDTO>(applicationForm);
        return Ok(applicationFormDto);
    }

    //TODO:需要拆分

    #endregion
}