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
    [HttpPost("/getUserStates/approval")]
    public async Task<ActionResult> GetStatesApproval(
        [FromQuery] int yearDate,
        [FromBody] UserDTO user)
    {
        var userExists = await _context.Users
            .AsNoTracking()
            .AnyAsync(u => u.UserID == user.UserID);

        if (!userExists)
        {
            return NotFound("用户不存在");
        }

        var listDate = await GetApprovalDatas(user.UserID,yearDate) ?? new List<CommonDatasModel>();

        return Ok(listDate);
    }

    /// <summary>
    /// 普通表单
    /// </summary>
    /// <param name="UserID"></param>
    /// <returns> </returns>
    [HttpPost("/getUserStates/default")] //查看普通表单
    public async Task<ActionResult> GetStatesDe([FromQuery] int yearDate, [FromBody] UserDTO user)
    {
        // 1. 校验用户
        var userExists = await _context.Users
            .AnyAsync(u => u.UserID == user.UserID);

        if (!userExists)
        {
            return NotFound("用户不存在");
        }

        // 2. 获取数据（直接带年份过滤）
        var listDate = await GetCommonDatas(user.UserID, yearDate);

        return Ok(listDate);
    }
    
    /// <summary> 普通用户的返回数据 </summary>
    /// <returns></returns>
    private async Task<List<CommonDatasModel>> GetCommonDatas(int userID, int yearDate)
    {
        var query = _context.ApplicationForms
            .Where(form => form.UserID == userID);

        // 年份过滤（推荐：范围查询）
        if (yearDate != 0)
        {
            var start = new DateTime(yearDate, 1, 1);
            var end = start.AddYears(1);

            query = query.Where(form =>
                form.ApprovalDate >= start &&
                form.ApprovalDate < end);
        }

        // 一次性查询 + 映射
        return await query
            .Select(form => new CommonDatasModel(form))
            .ToListAsync();
    }


    /// <summary> 审核用户的返回数据 </summary>
    /// <returns></returns>
    private async Task<List<CommonDatasModel>> GetApprovalDatas(int userID,int yearDate)
    {
        // 1️⃣ 先按 ApplicationFormID 分组，只取最新的一条
        var latestSummaries = await _context.TableSummaries
            .Where(s => s.UserID == userID)
            .GroupBy(s => s.ApplicationFormID)
            .Select(g => g.OrderByDescending(x => x.TableSummaryID).First())
            .AsNoTracking()
            .ToListAsync();

        if (!latestSummaries.Any())
            return new List<CommonDatasModel>();

        // 2️⃣ 一次性查所有 ApplicationForms（解决 N+1）
        var formIds = latestSummaries.Select(x => x.ApplicationFormID).ToList();

        var query = _context.ApplicationForms
            .Where(f => formIds.Contains(f.ApplicationFormID));

        if (yearDate != 0)
        {
            var start = new DateTime(yearDate, 1, 1);
            var end = start.AddYears(1);

            query = query.Where(f => f.ApprovalDate >= start && f.ApprovalDate < end);
        }

        var applicationForms = await query
            .AsNoTracking()
            .ToDictionaryAsync(f => f.ApplicationFormID);


        // 3️⃣ 内存组装
        var result = latestSummaries
            .Where(s => applicationForms.ContainsKey(s.ApplicationFormID))
            .Select(s => new CommonDatasModel(
                applicationForms[s.ApplicationFormID],
                s))
            .ToList();

        return result;
    }

    #endregion

    #region 查看所有已经完成审核的表格

    /// <summary>
    /// 查找所有表单（教务处汇总使用）
    /// </summary>
    /// <param name="UserID"></param>
    /// <returns></returns>
    [HttpGet("/getUserStates/allForm")]
    public async Task<ActionResult> GetAllForm([FromQuery] int yearDate)
    {
        var listDate = await GetAllFormDatas(yearDate);
        return Ok(listDate);
    }

    /// <summary> 普通用户的返回数据 </summary>
    /// <returns></returns>
    private async Task<List<CommonDatasModel>> GetAllFormDatas(int yearDate)
    {
        var query = _context.ApplicationForms
            .AsNoTracking() // ⭐ 关键优化
            .Where(form => form.Decision == 1 || form.Decision == 3);

        // 年份过滤（数据库层）
        if (yearDate != 0)
        {
            var start = new DateTime(yearDate, 1, 1);
            var end = start.AddYears(1);

            query = query.Where(form =>
                form.ApprovalDate >= start &&
                form.ApprovalDate < end);
        }

        return await query
            .Select(form => new CommonDatasModel(form))
            .ToListAsync();
    }

    #endregion
}

public class LoginViewModel
{
    public string Username { get; set; }
    public string Password { get; set; }
}