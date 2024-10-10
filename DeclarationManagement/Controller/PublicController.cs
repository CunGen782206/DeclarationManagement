using AutoMapper;
using DeclarationManagement.Model;
using DeclarationManagement.Model.DTO;
using Microsoft.AspNetCore.Mvc;

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
        var user = _context.Users.SingleOrDefault(u => u.Username == model.Username);
        if (user == null || !model.Password.Equals(user.Password))
        {
            return Unauthorized("用户名或密码错误");
        }

        // 如果需要，在此生成身份验证令牌（例如 JWT）

        return Ok(user); //返回所需要的值
    }

    //TODO：修改密码
    // POST: api/Account/login
    [HttpPost("ChangePassword")]
    public async Task<ActionResult> ChangePassword([FromBody] LoginViewModel model)
    {
        var user = _context.Users.SingleOrDefault(u => u.Username == model.Username);
        user.Password = model.Password;
        await _context.SaveChangesAsync();
        return Ok(); //返回所需要的值
    }

    #endregion

    #region 登陆后查看当前用户的所有表单

    /// <summary>
    /// 审核表单
    /// </summary>
    /// <param name="UserID"></param>
    /// <returns></returns>
    [HttpGet("/getUserStates/approval/{UserID}")]
    public async Task<ActionResult> GetStatesApproval(int UserID)
    {
        var user = _context.Users.SingleOrDefault(u => u.UserID == UserID);
        List<CommonDatasModel> listDate = GetApprovalDatas(UserID);
        return Ok(listDate);
    }

    /// <summary>
    /// 普通表单
    /// </summary>
    /// <param name="UserID"></param>
    /// <returns></returns>
    [HttpGet("/getUserStates/default/{UserID}")]
    public async Task<ActionResult> GetStatesDe(int UserID)
    {
        var user = _context.Users.SingleOrDefault(u => u.UserID == UserID);
        List<CommonDatasModel> listDate = GetCommonDatas(UserID);
        return Ok(listDate);
    }

    /// <summary> 普通用户的返回数据 </summary>
    /// <returns></returns>
    private List<CommonDatasModel> GetCommonDatas(int userID)
    {
        var applicationForms = _context.ApplicationForms.Where(form => form.UserID == userID);
        if (applicationForms.Any()) //判断是有元素
        {
            return applicationForms.Select(form => new CommonDatasModel(form)).ToList();
        }
        else
        {
            return null;
        }
    }


    /// <summary> 审核用户的返回数据 </summary>
    /// <returns></returns>
    private List<CommonDatasModel> GetApprovalDatas(int userID)
    {
        var tableSummaries = _context.TableSummaries.Where(summaries => summaries.UserID == userID);
        if (tableSummaries.Any())
        {
            return tableSummaries.Select(table => CreateCommonData(table, _context)).ToList();
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// 创建CreateCommonData
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
    public async Task<ActionResult> GetAllForm(int UserID)
    {
        var user = _context.Users.SingleOrDefault(u => u.UserID == UserID);
        List<CommonDatasModel> listDate = GetAllFormDatas();
        return Ok(listDate);
    }

    /// <summary> 普通用户的返回数据 </summary>
    /// <returns></returns>
    private List<CommonDatasModel> GetAllFormDatas()
    {
        var applicationForms = _context.ApplicationForms.Where(form => form.Decision == 1 || form.Decision == 2);
        if (applicationForms.Any()) //判断是有元素
        {
            return applicationForms.Select(form => new CommonDatasModel(form)).ToList();
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

public class ChangePasswordModel
{
    public string UserID { get; set; }
    public string NewPassword { get; set; }
}