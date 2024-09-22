using DeclarationManagement;
using Microsoft.EntityFrameworkCore;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

// 添加服务到容器中

// 配置 Entity Framework 和 SQL Server
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 添加控制器
        builder.Services.AddControllers();

// 配置 Swagger/OpenAPI
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

// 可选：添加身份验证和授权服务
// builder.Services.AddAuthentication();
// builder.Services.AddAuthorization();

        var app = builder.Build();

// 配置 HTTP 请求管道
        if (app.Environment.IsDevelopment()) // 仅在开发环境中启用
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

// 可选：使用身份验证和授权
// app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}