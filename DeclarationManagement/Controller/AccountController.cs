using AutoMapper;
using DeclarationManagement;
using Microsoft.AspNetCore.Mvc;

/// <summary> 登录 </summary>
[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IMapper _mapper;

    public AccountController(ApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    // POST: api/Account/login
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginViewModel model)
    {
        var user = _context.Users.SingleOrDefault(u => u.Username == model.Username);
        if (user == null || !VerifyPassword(model.Password, user.Password))
        {
            return Unauthorized("用户名或密码错误");
        }

        // 如果需要，在此生成身份验证令牌（例如 JWT）

        return Ok(new { user.UserID, user.Username, user.Role, user.Power });
    }

    private bool VerifyPassword(string inputPassword, string storedHash)
    {
        // 实现密码验证逻辑，例如哈希并比较
        return true;
    }
}

public class LoginViewModel
{
    public string Username { get; set; }
    public string Password { get; set; }
}