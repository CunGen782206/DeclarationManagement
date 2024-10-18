using AutoMapper;
using DeclarationManagement.Model;
using DeclarationManagement.Model.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DeclarationManagement.Controller;

/// <summary>
/// 通用控制
/// </summary>
[ApiController]
[Route("api/[controller]")] //api/ApplicationForms
public class PublicController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public PublicController(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    #region 登录部分

    // POST: api/Account/login
    [HttpPost("login")]
    public async Task<ActionResult> Login([FromBody] LoginViewModel model)
    {
        var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == model.Username);
        if (user == null || !model.Password.Equals(user.Password))
        {
            return Unauthorized("用户名或密码错误");
        }

        // 如果需要，在此生成身份验证令牌（例如 JWT）

        var userDto = _mapper.Map<UserDTO>(user);
        return Ok(userDto); //返回所需要的值
    }

    //TODO：修改密码
    // POST: api/Account/login
    [HttpPost("changePassword")]
    public async Task<ActionResult> ChangePassword([FromBody] LoginViewModel model)
    {
        var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == model.Username);
        if (user == null)
        {
            return Unauthorized("用户名错误"); //返回所需要的值
        }
        else
        {
            user.Password = model.Password;
            await _context.SaveChangesAsync();
            return Ok("修改密码成功"); //返回所需要的值
        }
    }

    #endregion

    #region 登陆后查看当前用户的所有表单

    /// <summary>
    /// 审核表单
    /// </summary>
    /// <param name="UserID"></param>
    /// <returns></returns>
    [HttpPost("/getUserStates/approval")] //查看审核表单
    public async Task<ActionResult> GetStatesApproval([FromBody] UserDTO user)
    {
        var userExists = await _context.Users.AnyAsync(u => u.UserID == user.UserID);
        if (!userExists)
        {
            return NotFound("用户不存在");
        }

        List<CommonDatasModel> listDate = await GetApprovalDatas(user.UserID);
        if (listDate == null) listDate = new();
        return Ok(listDate);
    }

    /// <summary>
    /// 普通表单
    /// </summary>
    /// <param name="UserID"></param>
    /// <returns> </returns>
    [HttpPost("/getUserStates/default")] //查看普通表单
    public async Task<ActionResult> GetStatesDe([FromBody] UserDTO user)
    {
        var userExists = await _context.Users.AnyAsync(u => u.UserID == user.UserID);
        if (!userExists)
        {
            return NotFound("用户不存在");
        }

        List<CommonDatasModel> listDate = await GetCommonDatas(user.UserID);
        if (listDate == null) listDate = new();
        return Ok(listDate);
    }

    /// <summary> 普通用户的返回数据 </summary>
    /// <returns></returns>
    private async Task<List<CommonDatasModel>> GetCommonDatas(int userID)
    {
        var applicationForms = _context.ApplicationForms.Where(form => form.UserID == userID);
        if (await applicationForms.AnyAsync()) //判断是有元素
        {
            return await applicationForms.Select(form => new CommonDatasModel(form)).ToListAsync();
        }
        else
        {
            return null;
        }
    }


    /// <summary> 审核用户的返回数据 </summary>
    /// <returns></returns>
    private async Task<List<CommonDatasModel>> GetApprovalDatas(int userID)
    {
        var tableSummaries = _context.TableSummaries.Where(summaries => summaries.UserID == userID); //查找当前Id下所有的审核表单
        if (await tableSummaries.AnyAsync())
        {
            return await tableSummaries.Select(table => CreateCommonData(table, _context)).ToListAsync();
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// 创建审核表单的信息
    /// </summary>
    /// <param name="tableSummary"></param>
    /// <returns></returns>
    private static CommonDatasModel CreateCommonData(TableSummary tableSummary, ApplicationDbContext _context)
    {
        var applicationForm =
            _context.ApplicationForms.SingleOrDefault(form => form.ApplicationFormID == tableSummary.ApplicationFormID);
        return new CommonDatasModel(applicationForm, tableSummary);
    }

    #endregion

    #region 查看所有已经完成审核的表格

    /// <summary>
    /// 查找所有表单（教务处汇总使用）
    /// </summary>
    /// <param name="UserID"></param>
    /// <returns></returns>
    [HttpGet("/getUserStates/allForm")]
    public async Task<ActionResult> GetAllForm()
    {
        List<CommonDatasModel> listDate = await GetAllFormDatas();
        if (listDate == null) listDate = new();
        return Ok(listDate);
    }

    /// <summary> 普通用户的返回数据 </summary>
    /// <returns></returns>
    private async Task<List<CommonDatasModel>> GetAllFormDatas()
    {
        var applicationForms = _context.ApplicationForms.Where(form => form.Decision == 1 || form.Decision == 3);
        if (await applicationForms.AnyAsync()) //判断是有元素
        {
            return await applicationForms.Select(form => new CommonDatasModel(form)).ToListAsync();
        }
        else
        {
            return null;
        }
    }

    #endregion
}

public class LoginViewModel
{
    public string Username { get; set; }
    public string Password { get; set; }
}