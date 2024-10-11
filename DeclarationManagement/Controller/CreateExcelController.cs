using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace DeclarationManagement.Controller;

/// <summary>
/// 生成Excel
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class CreateExcelController: ControllerBase
{
    private readonly ApplicationDbContext _context;

    public CreateExcelController(ApplicationDbContext context)
    {
        _context = context;
    }
    
    [HttpGet("export")]
    public async Task<IActionResult> Export()
    {
        // 获取数据
        var products = await _context.ApplicationForms.ToListAsync();

        // 使用 EPPlus 生成 Excel 文件
        using (var package = new ExcelPackage())
        {
            // 添加工作表
            var worksheet = package.Workbook.Worksheets.Add("Products");

            // 添加表头
            worksheet.Cells[1, 1].Value = "序号";
            worksheet.Cells[1, 2].Value = "部门";
            worksheet.Cells[1, 3].Value = "项目名称";
            worksheet.Cells[1, 5].Value = "项目类别";
            worksheet.Cells[1, 6].Value = "项目等级";
            worksheet.Cells[1, 7].Value = "奖项级别";
            worksheet.Cells[1, 8].Value = "参与形式";
            worksheet.Cells[1, 9].Value = "负责人";
            worksheet.Cells[1, 10].Value = "联系方式";
            worksheet.Cells[1, 11].Value = "盖章单位及时间";
            worksheet.Cells[1, 12].Value = "审核部门";
            worksheet.Cells[1, 13].Value = "处理意见";
            worksheet.Cells[1, 14].Value = "原因";
            worksheet.Cells[1, 15].Value = "认定等级";
            worksheet.Cells[1, 16].Value = "认定金额";
            worksheet.Cells[1, 17].Value = "备注";

            // 添加数据
            int row = 2;
            foreach (var product in products)
            {
                worksheet.Cells[row, 1].Value = "序号";
                worksheet.Cells[row, 2].Value = "部门";
                worksheet.Cells[row, 3].Value = "项目名称";
                worksheet.Cells[row, 5].Value = "项目类别";
                worksheet.Cells[row, 6].Value = "项目等级";
                worksheet.Cells[row, 7].Value = "奖项级别";
                worksheet.Cells[row, 8].Value = "参与形式";
                worksheet.Cells[row, 9].Value = "负责人";
                worksheet.Cells[row, 10].Value = "联系方式";
                worksheet.Cells[row, 11].Value = "盖章单位及时间";
                worksheet.Cells[row, 12].Value = "审核部门";
                worksheet.Cells[row, 13].Value = "处理意见";
                worksheet.Cells[row, 14].Value = "原因";
                worksheet.Cells[row, 15].Value = "认定等级";
                worksheet.Cells[row, 16].Value = "认定金额";
                worksheet.Cells[row, 17].Value = "备注";
                row++;
            }

            // 自动调整列宽
            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

            // 将 Excel 包保存到内存流中
            var stream = new MemoryStream();
            package.SaveAs(stream);
            stream.Position = 0;

            // 返回文件流
            string excelName = $"Products-{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.xlsx";

            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
        }
    }
}