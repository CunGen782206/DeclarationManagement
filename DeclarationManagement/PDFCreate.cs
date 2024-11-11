using System;
using System.IO;
using System.Text;
using DeclarationManagement.Model;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace DeclarationManagement;

public class PDFCreate
{
    
    /// <summary>
    /// 画框
    /// </summary>
    /// <param name="outputPath"></param>
    public void DrawGridT(string outputPath,ApplicationForm applicationForm)
    {
        try
        {
            // 注册代码页提供程序以支持更多编码
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            Console.WriteLine($"PDF 已成功生成: ");
        }
        catch (Exception ex)
        {
            Console.WriteLine("发生错误: " + ex.Message);
        }
        // 创建 PDF 文档
        Document document = new Document(PageSize.A4);
        PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(outputPath, FileMode.Create));
        document.Open();


        // 获取 PdfContentByte 对象，用于绘制内容
        PdfContentByte cb = writer.DirectContent;

        // 设置字体路径
        string fontPath = Path.Combine(@"C:\Windows\", "Fonts", "STFANGSO.TTF");
        if (!File.Exists(fontPath))
        {
            throw new FileNotFoundException($"字体文件未找到: {fontPath}");
        }

        // 加载支持中文的字体
        BaseFont baseFont = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
        Font font = new Font(baseFont, 14);
        Font fontSmall = new Font(baseFont, 11);

        // 设置字体路径
        string fontPathNew = Path.Combine(@"C:\Windows\", "Fonts", "STXIHEI.TTF");
        if (!File.Exists(fontPathNew))
        {
            throw new FileNotFoundException($"字体文件未找到: {fontPathNew}");
        }

        // 加载支持中文的字体
        BaseFont baseFontNew = BaseFont.CreateFont(fontPathNew, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
        // 添加标题
        Paragraph title = new Paragraph("上海交通职业技术学院教学质量工程项目认定表", new Font(baseFontNew, 16, Font.BOLD));
        title.Alignment = Element.ALIGN_CENTER;
        document.Add(title);
        document.Add(Chunk.NEWLINE);
        // 定义网格参数
        float fullLengthX = 595f; // 网格起始 X 坐标
        float fullLengthY = 842f; // 网格起始 Y 坐标

        #region 横向比例

        float oneH = 0.12f;
        float twoH = 0.16f;
        float threeH = 0.20f;
        float fourH = 0.24f;
        float fiveH = 0.28f;
        float sixH = 0.32f;
        float sevenH = 0.36f;
        float eightH = 0.44f;
        float nineH = 0.52f;
        float tenH = 0.67f;
        float elevenH = 0.82f;

        #endregion

        #region 纵向比例

        float oneV = 0.10f;
        float twoV = 0.30f;
        float threeV = 0.50f;
        float fourV = 0.70f;
        float fiveV = 0.90f;

        #endregion

        #region 各种顶点

        AddFormField1(document, writer, font, fullLengthX * oneV, fullLengthY * (1 - twoH), fullLengthX * twoV,
            fullLengthY * (1 - oneH), "项目负责人");
        AddFormField1(document, writer, font, fullLengthX * twoV, fullLengthY * (1 - twoH), fullLengthX * threeV,
            fullLengthY * (1 - oneH), applicationForm.ProjectLeader);
        AddFormField1(document, writer, font, fullLengthX * threeV, fullLengthY * (1 - twoH), fullLengthX * fourV,
            fullLengthY * (1 - oneH), "所属部门");
        AddFormField1(document, writer, font, fullLengthX * fourV, fullLengthY * (1 - twoH), fullLengthX * fiveV,
            fullLengthY * (1 - oneH), applicationForm.Department);

        AddFormField1(document, writer, font, fullLengthX * oneV, fullLengthY * (1 - threeH), fullLengthX * twoV,
            fullLengthY * (1 - twoH), "项目名称");
        AddFormField1(document, writer, font, fullLengthX * twoV, fullLengthY * (1 - threeH), fullLengthX * fiveV,
            fullLengthY * (1 - twoH), applicationForm.ProjectName);

        AddFormField1(document, writer, font, fullLengthX * oneV, fullLengthY * (1 - fourH), fullLengthX * twoV,
            fullLengthY * (1 - threeH), "项目类别");
        AddFormField1(document, writer, font, fullLengthX * twoV, fullLengthY * (1 - fourH), fullLengthX * fiveV,
            fullLengthY * (1 - threeH), applicationForm.ProjectCategory);

        AddFormField1(document, writer, font, fullLengthX * oneV, fullLengthY * (1 - fiveH), fullLengthX * twoV,
            fullLengthY * (1 - fourH), "项目等级");
        AddFormField1(document, writer, font, fullLengthX * twoV, fullLengthY * (1 - fiveH), fullLengthX * fiveV,
            fullLengthY * (1 - fourH), applicationForm.ProjectLevel);

        AddFormField1(document, writer, font, fullLengthX * oneV, fullLengthY * (1 - sixH), fullLengthX * twoV,
            fullLengthY * (1 - fiveH), "奖项级别");
        AddFormField1(document, writer, font, fullLengthX * twoV, fullLengthY * (1 - sixH), fullLengthX * fiveV,
            fullLengthY * (1 - fiveH), applicationForm.AwardLevel);

        AddFormField1(document, writer, font, fullLengthX * oneV, fullLengthY * (1 - sevenH), fullLengthX * twoV,
            fullLengthY * (1 - sixH), "参与形式");
        AddFormField1(document, writer, font, fullLengthX * twoV, fullLengthY * (1 - sevenH), fullLengthX * fiveV,
            fullLengthY * (1 - sixH), applicationForm.ParticipationForm);


        AddFormField1(document, writer, font, fullLengthX * oneV, fullLengthY * (1 - eightH), fullLengthX * twoV,
            fullLengthY * (1 - sevenH), "认定批文名称\r\n认定批文文件号");
        AddFormField1(document, writer, font, fullLengthX * twoV, fullLengthY * (1 - eightH), fullLengthX * fiveV,
            fullLengthY * (1 - sevenH), applicationForm.ApprovalFileName, Element.ALIGN_LEFT);

        AddFormField1(document, writer, font, fullLengthX * oneV, fullLengthY * (1 - nineH), fullLengthX * twoV,
            fullLengthY * (1 - eightH), "认定批文\r\n盖章单位及时间");
        AddFormField1(document, writer, fontSmall, fullLengthX * twoV, fullLengthY * (1 - nineH), fullLengthX * fiveV,
            fullLengthY * (1 - eightH), applicationForm.ApprovalFileNumber,
            Element.ALIGN_LEFT);

        AddFormField1(document, writer, font, fullLengthX * oneV, fullLengthY * (1 - tenH), fullLengthX * twoV,
            fullLengthY * (1 - nineH), "项目内容");
        AddFormField1(document, writer, fontSmall, fullLengthX * twoV, fullLengthY * (1 - tenH), fullLengthX * fiveV,
            fullLengthY * (1 - nineH),
            applicationForm.ItemDescription,
            Element.ALIGN_LEFT, Element.ALIGN_TOP);

        AddFormField1(document, writer, font, fullLengthX * oneV, fullLengthY * (1 - elevenH), fullLengthX * twoV,
            fullLengthY * (1 - tenH), "项目成果");
        AddFormField1(document, writer, fontSmall, fullLengthX * twoV, fullLengthY * (1 - elevenH), fullLengthX * fiveV,
            fullLengthY * (1 - tenH),
            applicationForm.ProjectOutcome,
            Element.ALIGN_LEFT, Element.ALIGN_TOP);

        #endregion


        // 关闭文档
        document.Close();
    }

    /// <summary>
    /// 添加字符Field
    /// </summary>
    /// <param name="document"></param>
    /// <param name="writer"></param>
    /// <param name="font"></param>
    /// <param name="llx1">Lower Left X (左下角 X 坐标)</param>
    /// <param name="lly1">Lower Left Y (左下角 Y 坐标)</param>
    /// <param name="urx1">Upper Right X (右上角 X 坐标)</param>
    /// <param name="ury1">Upper Right Y (右上角 Y 坐标)</param>
    private void AddFormField1(Document document, PdfWriter writer, Font font, float llx, float lly,
        float urx, float ury, string data, int elementHorizontal = Element.ALIGN_CENTER,
        int elementVertical = Element.ALIGN_MIDDLE)
    {
        // 2. 定义文本区域
        float width = urx - llx; // 区域宽度
        float height = ury - lly; // 区域高度


        // 3. 创建 PdfPTable 和 PdfPCell
        PdfPTable table = new PdfPTable(1); // 1 列的表格
        table.TotalWidth = width; // 设置表格总宽度

        PdfPCell cell = new PdfPCell();
        cell.FixedHeight = height; // 设置单元格的固定高度

        // 4. 移除单元格的默认内边距
        cell.Padding = 5f;
        cell.PaddingTop = 5f;
        cell.PaddingBottom = 5f;

        // 5. 设置单元格对齐方式
        cell.HorizontalAlignment = elementHorizontal; // 水平居中
        cell.VerticalAlignment = elementVertical; // 垂直居中  

        // 6. 设置 UseAscender 和 UseDescender
        cell.UseAscender = true;
        cell.UseDescender = true;

        // 7. 添加文本内容并自动换行
        Paragraph phrase = new Paragraph(data, font);
        // 5. 设置单元格对齐方式
        phrase.Alignment = elementHorizontal; // 确保短语内容居中

        // 8. 将短语添加到单元格中
        cell.AddElement(phrase);

        // 9. 将单元格添加到表格
        table.AddCell(cell);

        // 10. 将表格放置在指定位置
        table.WriteSelectedRows(0, -1, llx, lly + height, writer.DirectContent); // 绘制表格

        // 11. 绘制矩形边框（可选）
        PdfContentByte cb = writer.DirectContent;
        cb.SetLineWidth(0.3f);
        cb.SetColorStroke(BaseColor.BLACK);
        cb.Rectangle(llx, lly, width, height);
        cb.Stroke();
    }
}