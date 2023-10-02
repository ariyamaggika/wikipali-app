using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using UnityEditor;
//using UnityEditor.iOS;
using UnityEngine;
using Font = iTextSharp.text.Font;
using Image = iTextSharp.text.Image;
using Rectangle = iTextSharp.text.Rectangle;

public class GenPDFTool
{
    //参数一 文档大标题    参数二 纸张大小
    public GenPDFTool(string _fileName,string _title)
    {
        document = new Document();
        //横向
        //Document document = new Document(PageSize.A4.rotate());
        // 作者
        document.AddAuthor("wikipali app");
        // 创建日期
        document.AddCreationDate();
        // 创建关键字
        //document.addKeywords("测试");
        // 创建生产商，自动使用iText
        document.AddProducer();
        // 创建程序
        document.AddCreator("wikipali app");
        // 标题
        document.AddTitle(_title);
        // 主题
        //document.addSubject("测试PDF创建的主题");


        fileName = _fileName;
        //document = _document;
        Init();
    }

    BaseFont heiBaseFont;//字体
    public Font titleFont;//报告字体样式
    public Font firstTitleFont;//大标题字体样式
    public Font secondTitleFont;//小标题字体样式
    public Font contentFont;//内容字体样式
    Document document;//文档
    PdfPTable table;//表格
    string imageName;//插入图片的名字
    int tableColumn = 4;
    string fileName;
    /// <summary>
    /// 创建报告的初始化
    /// </summary>
    public void Init()
    {
        //创建字体
        heiBaseFont = BaseFont.CreateFont(Application.streamingAssetsPath + "/Res/Zawgyi-One.ttf", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
        titleFont = new Font(heiBaseFont, 26, 1);
        firstTitleFont = new Font(heiBaseFont, 20, 1);
        secondTitleFont = new Font(heiBaseFont, 13, 1);
        contentFont = new Font(heiBaseFont, 11, Font.NORMAL);
        //Document:（文档）生成pdf必备的一个对象,生成一个Document示例

        //为该Document创建一个Writer实例： 
        FileStream os = new FileStream(Application.streamingAssetsPath + "/" + fileName + ".pdf", FileMode.Create);
        PdfWriter.GetInstance(document, os);
        StartPDF();
    }
    /// <summary>
    /// 打开文档
    /// </summary>
    void StartPDF()
    {
        document.Open();
    }
    /// <summary>
    /// 增加表格
    /// </summary>
    /// <param name="column">列数</param>
    /// <param name="content">内容</param>
    public void AddTable(int column, string[] content)
    {
        table = new PdfPTable(column);
        table.TotalWidth = 520f;

        table.LockedWidth = true;

        table.HorizontalAlignment = 1;
        for (int i = 0; i < content.Length; i++)
        {
            PdfPCell cell = new PdfPCell(new Phrase(content[i], contentFont));
            // cell.FixedHeight = 20;

            cell.PaddingBottom = 5f;
            cell.PaddingTop = 5f;
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            table.AddCell(cell);
        }
        document.Add(table);
    }
    /// <summary>
    /// 实训一 列表数据
    /// </summary>
    /// <param name="column"></param>
    /// <param name="content"></param>
    public void AddTableTraining(int column, string[] content)
    {
        table = new PdfPTable(column);
        table.TotalWidth = 520f;

        table.LockedWidth = true;

        table.HorizontalAlignment = 1;

        for (int i = 0; i < content.Length; i++)
        {

            PdfPCell cell = new PdfPCell(new Phrase(content[i], contentFont));

            if (i == 0)
            {

                cell.Rowspan = 2;//合并行

            }
            if (i == 1 || i == 2)
            {
                cell.Colspan = 2;//合并列

            }
            if (i == 3)
            {
                cell.Colspan = 3;//合并列
            }
            cell.PaddingBottom = 5f;
            cell.PaddingTop = 5f;
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            table.AddCell(cell);
        }
        document.Add(table);
    }
    /// <summary>
    /// 实训三 列表数据
    /// </summary>
    /// <param name="column"></param>
    /// <param name="content"></param>
    public void AddTableTrainingThree(int column, string[] content)
    {
        table = new PdfPTable(column);
        table.TotalWidth = 800f;

        table.LockedWidth = true;

        table.HorizontalAlignment = 1;

        for (int i = 0; i < content.Length; i++)
        {

            PdfPCell cell = new PdfPCell(new Phrase(content[i], contentFont));


            if (i == 0)
            {

                cell.Colspan = 19;//合并列

            }
            if (i == 1 || i == 2)
            {
                cell.Rowspan = 2;//合并行

            }
            if (i == 3)
            {
                cell.Colspan = 6;//合并列
            }
            if (i == 4 || i == 7)
            {
                cell.Colspan = 3;//合并列
            }
            if (i == 5)
            {
                cell.Rowspan = 2;//合并行
            }
            if (i == 6 || i == 8)
            {
                cell.Colspan = 2;//合并列
            }
            cell.PaddingBottom = 5f;
            cell.PaddingTop = 5f;
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            table.AddCell(cell);
        }
        document.Add(table);
    }
    /// <summary>
    /// 实训三 列表数据
    /// </summary>
    /// <param name="column"></param>
    /// <param name="content"></param>
    public void AddTableTrainingThree2(int column, string[] content)
    {
        table = new PdfPTable(column);
        table.TotalWidth = 800f;

        table.LockedWidth = true;

        table.HorizontalAlignment = 1;

        for (int i = 0; i < content.Length; i++)
        {

            PdfPCell cell = new PdfPCell(new Phrase(content[i], contentFont));

            if (i == 0)
            {

                cell.Colspan = 21;//合并列

            }
            if (i == 1 || i == 2)
            {
                cell.Rowspan = 2;//合并行

            }
            if (i == 3)
            {
                cell.Colspan = 2;//合并列
            }
            if (i == 4 || i == 5 || i == 7)
            {
                cell.Colspan = 3;//合并列
            }
            if (i == 6)
            {
                cell.Colspan = 2;//合并列
            }
            if (i == 8)
            {
                cell.Rowspan = 2;//合并行
            }
            if (i == 9)
            {
                cell.Colspan = 5;//合并列
            }
            cell.PaddingBottom = 5f;
            cell.PaddingTop = 5f;
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            table.AddCell(cell);
        }
        document.Add(table);
    }
    public void AddTableTraining1(int column, string[] content)
    {
        table = new PdfPTable(column);
        table.TotalWidth = 520;
        table.LockedWidth = true;

        table.HorizontalAlignment = 1;

        for (int i = 0; i < content.Length; i++)
        {

            PdfPCell cell = new PdfPCell(new Phrase(content[i], contentFont));

            if (i == 0)
            {

                cell.Rowspan = 2;//合并行

            }
            if (i == 1 || i == 2)
            {
                cell.Colspan = 2;//合并列

            }
            if (i == 3)
            {
                cell.Colspan = 3;//合并列
            }
            cell.PaddingBottom = 5f;
            cell.PaddingTop = 5f;
            cell.HorizontalAlignment = 1;
            cell.VerticalAlignment = 1;
            table.AddCell(cell);
        }
        document.Add(table);
    }
    /// <summary>
    /// 空格
    /// </summary>
    public void Space()
    {
        Paragraph content = new Paragraph(new Chunk(" ", secondTitleFont));
        document.Add(content);
    }
    /// <summary>
    /// 插入文字内容
    /// </summary>
    /// <param name="content">内容</param>
    /// <param name="font">字体</param>
    /// <param name="type">格式,1为居中</param>
    public void AddContent(string content, Font font, int type = 0)
    {
        Paragraph contentP = new Paragraph(new Chunk(content, font));
        contentP.Alignment = type;
        document.Add(contentP);
    }
    /// <summary>
    /// 插入图片
    /// </summary>
    /// <param name="imageName"></param>
    /// <param name="scale"></param>
    public void AddImage(string imageName)
    {
        string path = Application.streamingAssetsPath + "/ReportRes/" + imageName;
        if (!File.Exists(path)) return;
        Image image = Image.GetInstance(Application.streamingAssetsPath + "/ReportRes/" + imageName);

        //Image image = Resources.Load<Image>(imageName);
        //这里都是图片最原始的宽度与高度
        float resizedWidht = image.Width;
        float resizedHeight = image.Height;
        image.ScaleToFit(475, 325);
        image.Alignment = Element.ALIGN_JUSTIFIED;
        document.Add(image);
    }
    /// <summary>
    /// 新的一页
    /// </summary>
    public void NewPage()
    {
        document.NewPage();
    }
    /// <summary>
    /// 关闭文档
    /// </summary>
    public void ClosePDF()
    {
        document.Close();
    }
}





