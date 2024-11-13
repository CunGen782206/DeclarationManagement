using System.IO.Compression;
using DeclarationManagement.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;

namespace DeclarationManagement.Controller;

[ApiController]
[Route("api/[controller]")]
public class FilesController : ControllerBase
{
    public static string _uploadFolder;
    public static string ApprovalFileAttachmentDirectory = "ApprovalFileAttachment";
    public static string combineDirectory = "Combine";
    public static string cacheDirectory = "Cache";
    public static string endCombine = "EndCombine";

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

        // 可选：为文件名生成唯一标识，防止冲突
        var uniqueFileName = $"{Guid.NewGuid()}_{fileName}";
        var directory = Path.Combine(_uploadFolder, ApprovalFileAttachmentDirectory);

        // 创建新文件夹
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
        var filePath = Path.Combine(directory, uniqueFileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        return Ok(new { message = "文件上传成功", fileName = uniqueFileName });
    }

    /// <summary>
    /// 下载文件
    /// </summary>
    /// <param name="filename">文件名</param>
    /// <returns></returns>
    [HttpGet("download")]
    public async Task<IActionResult> Download([FromQuery] string filename)
    {
        if (string.IsNullOrEmpty(filename))
            return BadRequest("文件名不能为空");

        var filePath = Path.Combine(_uploadFolder, ApprovalFileAttachmentDirectory, filename);

        if (!System.IO.File.Exists(filePath))
            return NotFound("文件未找到");

        var contentType = GetContentType(filePath);
        var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);

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
    /// 下载大文件（分段下载）
    /// </summary>
    /// <returns></returns>
    [HttpGet("largeFile")]
    public async Task<IActionResult> DownloadLargeFile()
    {
        var filePath = await CreateZipAsync(); // 异步生成大文件
        if (!System.IO.File.Exists(filePath))
        {
            return NotFound();
        }

        var fileSize = new FileInfo(filePath).Length;
        var rangeHeader = Request.Headers["Range"].ToString();

        if (string.IsNullOrEmpty(rangeHeader))
        {
            // 未提供 Range 请求头，返回整个文件
            var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            return File(stream, "application/zip", Path.GetFileName(filePath), enableRangeProcessing: true);
        }
        else
        {
            // 解析 Range 请求头
            var range = RangeHeaderValue.Parse(rangeHeader);
            var from = range.Ranges.First().From ?? 0;
            var to = range.Ranges.First().To ?? (fileSize - 1);

            if (from >= fileSize || to >= fileSize)
            {
                return StatusCode(416); // Range Not Satisfiable
            }

            var contentLength = to - from + 1;

            // 设置响应头
            Response.StatusCode = StatusCodes.Status206PartialContent;
            Response.Headers["Content-Range"] = $"bytes {from}-{to}/{fileSize}";
            Response.Headers["Accept-Ranges"] = "bytes";
            Response.Headers["Content-Length"] = contentLength.ToString();

            // 返回文件流
            var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            stream.Seek(from, SeekOrigin.Begin);

            return new FileStreamResult(stream, "application/zip")
            {
                EnableRangeProcessing = true,
                FileDownloadName = Path.GetFileName(filePath)
            };
        }
    }

    /// <summary>
    /// 创建归档Zip（异步）
    /// </summary>
    /// <returns></returns>
    private async Task<string> CreateZipAsync()
    {
        var pathCombine = Path.Combine(_uploadFolder, combineDirectory); //最大号生成
        var zipName = DateTime.Now.ToString("yyyy-MM-dd-HH");
        var zipDirectory = Path.Combine(_uploadFolder, endCombine);

        // 创建新文件夹
        if (!Directory.Exists(zipDirectory))
        {
            Directory.CreateDirectory(zipDirectory);
        }

        var zipPath = Path.Combine(zipDirectory, zipName) + ".zip";
        await ZipFolderAsync(pathCombine, zipPath); // 异步压缩文件夹
        return zipPath;
    }

    /// <summary>
    /// 异步压缩文件夹
    /// </summary>
    /// <param name="folderPath">压缩文件夹路径</param>
    /// <param name="zipFilePath">压缩文件的路径</param>
    public static async Task ZipFolderAsync(string folderPath, string zipFilePath)
    {
        // 如果压缩包已存在，删除它
        if (System.IO.File.Exists(zipFilePath))
        {
            System.IO.File.Delete(zipFilePath);
        }

        // 异步压缩文件夹
        await Task.Run(() => ZipFile.CreateFromDirectory(folderPath, zipFilePath));
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
