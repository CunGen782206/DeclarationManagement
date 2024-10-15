using DeclarationManagement.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Options;
namespace DeclarationManagement.Controller;


/// <summary> 文件控制 </summary>

[ApiController]
[Route("api/[controller]")]
public class FilesController : ControllerBase
{
    private readonly string _uploadFolder;

    
    /// <summary> 创建文件夹 </summary>
    /// <param name="fileSettings"></param>
    public FilesController(IOptions<FileSettings> fileSettings)
    {
        // 获取配置的上传文件夹路径
        _uploadFolder = fileSettings.Value.UploadFolder;

        // 如果路径是相对路径，相对于应用程序根目录
        if (!Path.IsPathRooted(_uploadFolder))
        {
            _uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), _uploadFolder);
        }

        // 确保上传文件夹存在
        if (!Directory.Exists(_uploadFolder))
        {
            Directory.CreateDirectory(_uploadFolder);
        }
    }

    /// <summary>
    /// 上传文件
    /// </summary>
    /// <param name="file">上传的文件</param>
    /// <returns></returns>
    [HttpPost("upload")]
    public async Task<IActionResult> Upload([FromForm] IFormFile file)
    {
        if (file == null || file.Length == 0)
            return BadRequest("没有选择文件上传");

        // 安全处理文件名，移除路径信息
        var fileName = Path.GetFileName(file.FileName);
        var filePath = Path.Combine(_uploadFolder, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        return Ok(new { message = "文件上传成功", fileName });
    }

    /// <summary>
    /// 下载文件
    /// </summary>
    /// <param name="filename">文件名</param>
    /// <returns></returns>
    [HttpGet("download")]
    public IActionResult Download([FromQuery] string filename)
    {
        if (string.IsNullOrEmpty(filename))
            return BadRequest("文件名不能为空");

        var filePath = Path.Combine(_uploadFolder, filename);

        if (!System.IO.File.Exists(filePath))
            return NotFound("文件未找到");

        var contentType = GetContentType(filePath);
        var fileBytes = System.IO.File.ReadAllBytes(filePath);

        // 如果是 PDF，设置为 inline 以在浏览器中直接查看
        if (contentType == "application/pdf")
        {
            return File(fileBytes, contentType, filename, enableRangeProcessing: true);
        }
        else
        {
            // 其他类型文件，设置为 attachment 以触发下载
            return File(fileBytes, contentType, filename);
        }
    }

    /// <summary>
    /// 根据文件扩展名获取 MIME 类型
    /// </summary>
    /// <param name="path">文件路径</param>
    /// <returns>MIME 类型字符串</returns>
    private string GetContentType(string path)
    {
        var provider = new FileExtensionContentTypeProvider();
        if (!provider.TryGetContentType(path, out string contentType))
        {
            contentType = "application/octet-stream";
        }

        return contentType;
    }
}