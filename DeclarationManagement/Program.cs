using DeclarationManagement;
using DeclarationManagement.Model;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;

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
                builder.Configuration.GetConnectionString("DefaultConnection"))); // 从配置文件中获取连接字符串

        // 绑定 FileSettings 配置
        builder.Services.Configure<FileSettings>(builder.Configuration.GetSection("FileSettings"));

        // 将对控制器的支持添加到服务容器中
        builder.Services.AddControllers();

        // 配置 Swagger/OpenAPI
        builder.Services.AddEndpointsApiExplorer();
        // builder.Services.AddSwaggerGen();

        #region 支持文件传输的Swag

        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "File Upload API", Version = "v1" });

            // 支持文件上传
            c.OperationFilter<AddFileParamterOperationFilter>();

            // 设定支持的内容类型
            c.AddSecurityDefinition("multipart/form-data", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.ApiKey,
                Name = "Content-Type",
                In = ParameterLocation.Header,
                Description = "Content-Type header"
            });
        });

        // 配置文件大小限制（可选）
        builder.Services.Configure<FormOptions>(options =>
        {
            options.MultipartBodyLengthLimit = 104857600; // 100MB
        });
        
        #endregion

        // 配置 CORS（如果前端与后端在不同域）
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy =>
            {
                policy.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader();
            });
        });

        // 可选：添加身份验证和授权服务
        // builder.Services.AddAuthentication();
        // builder.Services.AddAuthorization();

        var app = builder.Build();

        // 配置 HTTP 请求管道
        if (app.Environment.IsDevelopment()) // 仅在开发环境中启用 Swagger
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection(); // 将 HTTP 请求重定向到 HTTPS

        // 使用 CORS
        app.UseCors("AllowAll");

        // 可选：使用身份验证和授权
        // app.UseAuthentication();
        app.UseAuthorization();

        // 映射控制器
        app.MapControllers();

        // 映射静态文件（允许浏览器直接访问上传的文件，例如在线查看 PDF）
        var fileSettings = app.Services.GetRequiredService<IOptions<FileSettings>>().Value;
        string uploadPath = fileSettings.UploadFolder;

        // 如果是相对路径，则转换为绝对路径
        if (!Path.IsPathRooted(uploadPath))
        {
            uploadPath = Path.Combine(Directory.GetCurrentDirectory(), uploadPath);
        }

        // 确保上传文件夹存在
        if (!Directory.Exists(uploadPath))
        {
            Directory.CreateDirectory(uploadPath);
        }

        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(uploadPath),
            RequestPath = "/Uploads",
            ContentTypeProvider = new FileExtensionContentTypeProvider()
        });

        app.Run();
    }
}