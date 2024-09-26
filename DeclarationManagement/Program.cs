using DeclarationManagement;
using Microsoft.EntityFrameworkCore;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

// 添加服务到容器中

// 添加 AutoMapper 服务
        builder.Services.AddAutoMapper(typeof(AutoMapperProfile));

// 配置 Entity Framework 和 SQL Server
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(
                builder.Configuration
                    .GetConnectionString(
                        "DefaultConnection"))); //制定要使用的数据库（从配置文件中（appsettings.json） 中获取名为“DefaultConnection”的数据库连接字符串）

// 将对控制器的支持添加到服务容器中
        builder.Services.AddControllers();

// 配置 Swagger/OpenAPI
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

// 可选：添加身份验证和授权服务
// builder.Services.AddAuthentication();
// builder.Services.AddAuthorization();

        var app = builder.Build();

// 配置 HTTP 请求管道
        if (app.Environment.IsDevelopment()) // 仅在开发环境中启用（这些 Swagger 相关的中间件仅在开发环境中启用，避免在生产环境中公开 API 文档。）
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection(); //将 HTTP 请求重定向到 HTTPS，确保应用程序始终通过 HTTPS 传输数据，以提高安全性。

// 可选：使用身份验证和授权
// app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}