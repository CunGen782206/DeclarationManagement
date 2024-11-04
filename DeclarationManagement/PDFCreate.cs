using System;
using System.IO;
using System.Text;
using iTextSharp.text;
using iTextSharp.text.pdf;

public class PDFSet
{

    public void CreatePDFSet()
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

        // FillPdf(@"E:\TestPDF\test.pdf", @"E:\TestPDF\TestPDF1.pdf");
        // CreateEditablePdf(@"E:\TestPDF\test.pdf");
        DrawGrid(@"E:\TestPDF\TestPDF1.pdf");
    }
    /// <summary>
    /// 注册编码
    /// </summary>
    public void SignIn()
    {
        // 注册代码页提供程序以支持更多编码
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
    }

    #region 输入文字

    //
    // public void FillPdf(string pdfPath, string outputPath)
    // {
    //     // 打开PDF文件
    //     PdfReader pdfReader = new PdfReader(pdfPath);
    //     PdfStamper pdfStamper = new PdfStamper(pdfReader, new FileStream(outputPath, FileMode.Create));
    //
    //     // 加载支持中文的字体
    //     BaseFont baseFont =
    //         BaseFont.CreateFont(@"C:\Windows\Fonts\simsun.ttc,0", BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
    //
    //     // 获取表单字段
    //     AcroFields fields = pdfStamper.AcroFields;
    //
    //     // 设置字段字体和大小
    //     foreach (var fieldKey in fields.Fields.Keys)
    //     {
    //         fields.SetFieldProperty(fieldKey, "textfont", baseFont, null);
    //         fields.SetFieldProperty(fieldKey, "textsize", 12f, null);
    //     }
    //
    //     // 设置字段内容
    //     fields.SetField("项目负责人", "张三");
    //     fields.SetField("所属部门", "教务部");
    //     // 其他字段类似处理
    //
    //     // 设置表单为不可编辑
    //     // pdfStamper.FormFlattening = true;
    //
    //     // 关闭文档
    //     pdfStamper.Close();
    //     pdfReader.Close();
    // }

    #endregion

    
    public void CreateEditablePdf(string filePath)
    {
        // 创建 PDF 文档
        Document document = new Document(PageSize.A4);
        PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(filePath, FileMode.Create));
        document.Open();

        // 设置字体路径
        string fontPath = Path.Combine(@"C:\Windows\", "Fonts", "STZHONGS.TTF");
        if (!File.Exists(fontPath))
        {
            throw new FileNotFoundException($"字体文件未找到: {fontPath}");
        }

        // 加载支持中文的字体
        BaseFont baseFont = BaseFont.CreateFont(fontPath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
        Font font = new Font(baseFont, 12);

        // 添加标题
        Paragraph title = new Paragraph("上海交通职业技术学院教学质量工程项目认定申请表", new Font(baseFont, 16, Font.BOLD));
        title.Alignment = Element.ALIGN_CENTER;
        document.Add(title);
        document.Add(Chunk.NEWLINE);

        // 添加表单字段
        AddFormField(document, writer, "项目负责人", 50, 750, font);
        AddFormField(document, writer, "所属部门", 50, 720, font);
        AddFormField(document, writer, "项目名称", 50, 690, font);
        AddFormField(document, writer, "项目类别", 50, 660, font);
        AddFormField(document, writer, "项目等级", 50, 630, font);
        AddFormField(document, writer, "奖项级别", 50, 600, font);
        AddFormField(document, writer, "参与形式", 50, 570, font);

        // 添加多行字段
        AddMultiLineField(document, writer, "项目内容", 50, 490, 500, 60, font);
        AddMultiLineField(document, writer, "项目成果", 50, 410, 500, 60, font);
        AddMultiLineField(document, writer, "所属部门意见", 50, 330, 500, 60, font);
        AddMultiLineField(document, writer, "责任部门意见", 50, 250, 500, 60, font);
        AddMultiLineField(document, writer, "“质量工程”领导小组认定意见", 50, 170, 500, 60, font);
        AddMultiLineField(document, writer, "学院意见", 50, 90, 500, 60, font);

        // 关闭文档
        document.Close();
    }

    /// <summary>
    /// 添加文本框
    /// </summary>
    /// <param name="document"></param>
    /// <param name="writer"></param>
    /// <param name="fieldName"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="font"></param>
    private void AddFormField(Document document, PdfWriter writer, string fieldName, float x, float y,
        Font font)
    {
        // 添加字段标签
        PdfContentByte cb = writer.DirectContent;
        cb.BeginText();
        cb.SetFontAndSize(font.BaseFont, 12);
        cb.SetTextMatrix(x, y);
        cb.ShowText(fieldName + "：");
        cb.EndText();

        // 创建文本域
        TextField textField = new TextField(writer, new iTextSharp.text.Rectangle(x + 80, y - 5, x + 300, y + 15),
            fieldName);
        textField.FontSize = 12;
        textField.Font = font.BaseFont;
        writer.AddAnnotation(textField.GetTextField());
    }

    private void AddMultiLineField(Document document, PdfWriter writer, string fieldName, float x, float y,
        float width, float height, Font font)
    {
        // 添加字段标签
        PdfContentByte cb = writer.DirectContent;
        cb.BeginText();
        cb.SetFontAndSize(font.BaseFont, 12);
        cb.SetTextMatrix(x, y + height - 15);
        cb.ShowText(fieldName + "：");
        cb.EndText();

        // 创建多行文本域
        TextField textField = new TextField(writer, new iTextSharp.text.Rectangle(x, y, x + width, y + height),
            fieldName);
        textField.Options = TextField.MULTILINE;
        textField.FontSize = 12;
        textField.Font = font.BaseFont;
        writer.AddAnnotation(textField.GetTextField());
    }

   /// <summary>
    /// 画框
    /// </summary>
    /// <param name="outputPath"></param>
    public static void DrawGrid(string outputPath)
    {
        // 创建 PDF 文档
        Document document = new Document(PageSize.A4);
        PdfWriter writer = PdfWriter.GetInstance(document, new FileStream(outputPath, FileMode.Create));
        document.Open();

        // 获取 PdfContentByte 对象，用于绘制内容
        PdfContentByte cb = writer.DirectContent;

        //左下角是0，0点（x=595 y=842）
        
        // 定义网格参数
        float startX = 50f; // 网格起始 X 坐标
        float startY = 842f; // 网格起始 Y 坐标
        float cellWidth = 50f; // 单元格宽度
        float cellHeight = 30f; // 单元格高度
        int rows = 10; // 行数
        int cols = 5; // 列数

        // 设置线条宽度
        cb.SetLineWidth(0.5f);

        // 绘制水平线
        for (int i = 0; i <= rows; i++)
        {
            cb.MoveTo(startX, startY - i * cellHeight);
            cb.LineTo(startX + cols * cellWidth, startY - i * cellHeight);
        }

        // 绘制垂直线
        for (int i = 0; i <= cols; i++)
        {
            cb.MoveTo(startX + i * cellWidth, startY);
            cb.LineTo(startX + i * cellWidth, startY - rows * cellHeight);
        }

        // 完成绘制
        cb.Stroke();

        // 如果需要在单元格中添加文本，您可以执行以下操作
        // 设置字体
        BaseFont baseFont = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.WINANSI, BaseFont.EMBEDDED);
        cb.SetFontAndSize(baseFont, 10);

        // 在单元格中添加文本
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                float x = startX + col * cellWidth;
                float y = startY - row * cellHeight;

                // 计算文本位置（单元格中心）
                float textX = x + cellWidth / 2;
                float textY = y - cellHeight / 2;

                // 添加文本
                ColumnText.ShowTextAligned(cb, Element.ALIGN_CENTER, new Phrase($"({row + 1},{col + 1})"), textX, textY, 0);
            }
        }

        // 关闭文档
        document.Close();
    }
       
}