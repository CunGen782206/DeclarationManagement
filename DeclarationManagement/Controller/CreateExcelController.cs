using DeclarationManagement.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace DeclarationManagement.Controller;

/// <summary>
/// 生成Excel
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class CreateExcelController : ControllerBase
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
        var applicationForms = await _context.ApplicationForms.Where(form => form.Decision == 1 || form.Decision == 3)
            .ToListAsync();

        if (applicationForms == null) applicationForms = new();

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
            worksheet.Cells[1, 15].Value = "认定项目等级";
            worksheet.Cells[1, 16].Value = "认定奖项级别";
            worksheet.Cells[1, 17].Value = "认定金额";
            worksheet.Cells[1, 18].Value = "备注";

            // 添加数据
            int row = 2;
            foreach (var applicationForm in applicationForms)
            {
                worksheet.Cells[row, 1].Value = (row - 1).ToString();
                worksheet.Cells[row, 2].Value = applicationForm.Department;
                worksheet.Cells[row, 3].Value = applicationForm.ProjectName;
                worksheet.Cells[row, 5].Value = applicationForm.ProjectCategory;
                worksheet.Cells[row, 6].Value = applicationForm.ProjectLevel;
                worksheet.Cells[row, 7].Value = applicationForm.AwardLevel;
                worksheet.Cells[row, 8].Value = applicationForm.ParticipationForm;
                worksheet.Cells[row, 9].Value = applicationForm.ProjectLeader;
                worksheet.Cells[row, 10].Value = applicationForm.ContactWay;
                worksheet.Cells[row, 11].Value = "盖章单位及时间";
                worksheet.Cells[row, 12].Value = applicationForm.AuditDepartment;
                string decision = "";
                switch (applicationForm.Decision)
                {
                    case 1:
                        decision = "拟同意";
                        break;
                    case 3:
                        decision = "不同意";
                        break;
                    default:
                        decision = "";
                        break;
                }
                worksheet.Cells[row, 13].Value = decision;
                worksheet.Cells[row, 14].Value = applicationForm.Comments;
                worksheet.Cells[row, 15].Value = applicationForm.RecognitionProjectLevel;
                worksheet.Cells[row, 16].Value = applicationForm.RecognitionAwardLevel;
                worksheet.Cells[row, 17].Value = applicationForm.DeemedAmount;
                worksheet.Cells[row, 18].Value = applicationForm.Remarks;
                row++;
            }

            // 自动调整列宽
            worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

            // 将 Excel 包保存到内存流中
            var stream = new MemoryStream();
            package.SaveAs(stream);
            stream.Position = 0;

            // 返回文件流
            string excelName = $"Products-{DateTime.Now.ToString("yyyy-MM-dd-HH")}.xlsx";

            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
        }
    }
}