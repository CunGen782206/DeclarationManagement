using DeclarationManagement.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DeclarationManagement.Controller;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    // 构造函数
    public UserController(ApplicationDbContext context)
    {
        _context = context;
    }

    // 用户登录
    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == model.Username);

        if (user != null && VerifyPassword(model.Password, user.Password))
        {
            // 登录成功，生成Token等
            return Ok(new { message = "登录成功", userId = user.UserID });
        }

        return Unauthorized(new { message = "用户名或密码错误" });
    }

    private bool VerifyPassword(string inputPassword, string storedHashedPassword)
    {
        // 实现密码验证逻辑，如哈希比较
        // 这里只是简单比较，实际中应使用哈希和盐值
        return inputPassword == storedHashedPassword;
    }

    // 用户注册（如果需要）
    [HttpPost("Register")]
    public async Task<IActionResult> Register([FromBody] User model)
    {
        // 检查用户名是否已存在
        if (await _context.Users.AnyAsync(u => u.Username == model.Username))
        {
            return BadRequest(new { message = "用户名已存在" });
        }

        // 保存新用户
        _context.Users.Add(model);
        await _context.SaveChangesAsync();

        return Ok(new { message = "注册成功" });
    }
}

// 登录模型
public class LoginModel
{
    public string Username { get; set; }
    public string Password { get; set; }
}