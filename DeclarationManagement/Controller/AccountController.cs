using DeclarationManagement;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    public AccountController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginViewModel model)
    {
        var user = _context.Users.SingleOrDefault(u => u.Username == model.Username);
        if (user == null || !VerifyPassword(model.Password, user.Password))
        {
            return Unauthorized("用户名或密码错误");
        }

        // 生成 JWT 或其他身份验证令牌（此处省略）

        return Ok(new { user.UserID, user.Username, user.Role, user.Power });
    }

    private bool VerifyPassword(string inputPassword, byte[] storedHash)
    {
        // 实现密码验证逻辑，例如使用哈希算法（此处省略具体实现）
        return true;
    }
}

public class LoginViewModel
{
    public string Username { get; set; }
    public string Password { get; set; }
}