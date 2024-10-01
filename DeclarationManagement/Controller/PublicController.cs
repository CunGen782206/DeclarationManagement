using AutoMapper;
using DeclarationManagement.Model;
using DeclarationManagement.View;
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

        return Ok( GetStatesPrivate(user.UserID));//返回所需要的值
    }

    #endregion

    #region 登陆后查看当前用户的所有表单
    
    //TODO:登录做在这里

    [HttpGet("/getUserStates/{UserID}")] //查找当前用户的表单
    public async Task<ActionResult> GetStates(int UserID)
    {
        return Ok(GetStatesPrivate(UserID));
    }

    private List<CommonDatas> GetStatesPrivate(int UserID)
    {
        var user = _context.Users.SingleOrDefault(u => u.UserID == UserID);
        List<CommonDatas> listDate;
        if (user.Power == nameof(Power.普通用户))
        {
            listDate = GetCommonDatas(UserID);
        }
        else
        {
            listDate = GetApprovalDatas(UserID);
        }

        return listDate;
    }

    /// <summary> 普通用户的返回数据 </summary>
    /// <returns></returns>
    private List<CommonDatas> GetCommonDatas(int userID)
    {
        var applicationForms = _context.ApplicationForms.Where(form => form.UserID == userID);
        if (applicationForms.Any()) //判断是有元素
        {
            return applicationForms.Select(form => new CommonDatas(form)).ToList();
        }
        else
        {
            return null;
        }
    }


    /// <summary> 审核用户的返回数据 </summary>
    /// <returns></returns>
    private List<CommonDatas> GetApprovalDatas(int userID)
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
    private static CommonDatas CreateCommonData(TableSummary tableSummary,ApplicationDbContext _context)
    {
        var applicationForm = _context.ApplicationForms.SingleOrDefault(form => form.ApplicationFormID == tableSummary.ApplicationFormID);
        return new CommonDatas(applicationForm, tableSummary);
    }

    #endregion
}


public class LoginViewModel
{
    public string Username { get; set; }
    public string Password { get; set; }
}