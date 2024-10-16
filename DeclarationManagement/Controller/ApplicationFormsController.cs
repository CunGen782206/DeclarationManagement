using AutoMapper;
using DeclarationManagement;
using DeclarationManagement.Controller;
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

        #region 修改文件名

        if (string.IsNullOrEmpty(record.ApprovalFileAttachmentName))
        {
            var newFileName = $"{Guid.NewGuid()}_{user.Role}_{user.Name}_{record.ApprovalFileAttachmentName}";
            RenamingFile(record.ApprovalFileAttachmentName, newFileName);
            record.ApprovalFileAttachmentName = newFileName;
        }

        #endregion

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
    private async Task<User> ApprovalOne(string department, string power)
    {
        return await _context.Users.FirstOrDefaultAsync(f => (f.Role == department) && (f.Power == power));
    }

    /// <summary>
    /// 修改文件名
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="rename"></param>
    private async Task RenamingFile(string fileName, string rename)
    {
        if (string.IsNullOrEmpty(fileName))
            return;
        var filePath = Path.Combine(FilesController._uploadFolder, fileName);
        if (!System.IO.File.Exists(filePath))
            return;
        var filePathRemove = Path.Combine(FilesController._uploadFolder, rename);
        System.IO.File.Move(filePath, filePathRemove);
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
    [HttpPost("/getForm")] //传入表格ID（获得表单）
    public async Task<ActionResult> GetForms([FromBody] GetFormsModel getFormsModel)
    {
        int tableSummaries = 0;
        if (getFormsModel.getCode == 1)
        {
            var table = await _context.TableSummaries.SingleOrDefaultAsync(summary =>
                summary.TableSummaryID == getFormsModel.FormID);
            if (table == null)
            {
                return NotFound("汇总表记录不存在。");
            }

            tableSummaries = table.TableSummaryID;
            getFormsModel.FormID = table.ApplicationFormID; //获取当前表单
        }

        var applicationForm = await _context.ApplicationForms
            .Include(applicationForm => applicationForm.ApprovalRecords)
            .ThenInclude(approvalRecord => approvalRecord.User)
            .FirstOrDefaultAsync(form => form.ApplicationFormID == getFormsModel.FormID); //需要通过Include加载关系
        // var applicationForm = _context.ApplicationForms.FirstOrDefault(form => form.ApplicationFormID == FormId);//如果直接这样写就会出现错误。
        if (applicationForm == null)
        {
            return NotFound("申请表单不存在。");
        }

        var applicationFormDto = _mapper.Map<ApplicationFormDTO>(applicationForm);
        return Ok(new { tableSummaries, applicationFormDto });
    }

    //TODO:需要拆分

    #endregion

    #region 取消单个表单

    [HttpPost("/cancelForm")] //传入表格ID（获得表单）
    public async Task<ActionResult> CancelForms([FromBody] CancelForm fileName)
    {
        if (string.IsNullOrEmpty(fileName.FileName))
            return BadRequest("文件名不能为空");

        var filePath = Path.Combine(FilesController._uploadFolder, fileName.FileName);
        if (!System.IO.File.Exists(filePath))
            return NotFound("文件未找到");
        System.IO.File.Delete(filePath); //删除文件
        return Ok("删除完成");
    }

    #endregion
}

public class GetFormsModel
{
    public int getCode { get; set; }
    public int FormID { get; set; }
}

public class CancelForm
{
    public string FileName { get; set; }
}