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
    //����һ �ĵ������    ������ ֽ�Ŵ�С
    public GenPDFTool(string _fileName,string _title)
    {
        document = new Document();
        //����
        //Document document = new Document(PageSize.A4.rotate());
        // ����
        document.AddAuthor("wikipali app");
        // ��������
        document.AddCreationDate();
        // �����ؼ���
        //document.addKeywords("����");
        // ���������̣��Զ�ʹ��iText
        document.AddProducer();
        // ��������
        document.AddCreator("wikipali app");
        // ����
        document.AddTitle(_title);
        // ����
        //document.addSubject("����PDF����������");


        fileName = _fileName;
        //document = _document;
        Init();
    }

    BaseFont heiBaseFont;//����
    public Font titleFont;//����������ʽ
    public Font firstTitleFont;//�����������ʽ
    public Font secondTitleFont;//С����������ʽ
    public Font contentFont;//����������ʽ
    Document document;//�ĵ�
    PdfPTable table;//���
    string imageName;//����ͼƬ������
    int tableColumn = 4;
    string fileName;
    /// <summary>
    /// ��������ĳ�ʼ��
    /// </summary>
    public void Init()
    {
        //��������
        heiBaseFont = BaseFont.CreateFont(Application.streamingAssetsPath + "/Res/Zawgyi-One.ttf", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
        titleFont = new Font(heiBaseFont, 26, 1);
        firstTitleFont = new Font(heiBaseFont, 20, 1);
        secondTitleFont = new Font(heiBaseFont, 13, 1);
        contentFont = new Font(heiBaseFont, 11, Font.NORMAL);
        //Document:���ĵ�������pdf�ر���һ������,����һ��Documentʾ��

        //Ϊ��Document����һ��Writerʵ���� 
        FileStream os = new FileStream(Application.streamingAssetsPath + "/" + fileName + ".pdf", FileMode.Create);
        PdfWriter.GetInstance(document, os);
        StartPDF();
    }
    /// <summary>
    /// ���ĵ�
    /// </summary>
    void StartPDF()
    {
        document.Open();
    }
    /// <summary>
    /// ���ӱ��
    /// </summary>
    /// <param name="column">����</param>
    /// <param name="content">����</param>
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
    /// ʵѵһ �б�����
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

                cell.Rowspan = 2;//�ϲ���

            }
            if (i == 1 || i == 2)
            {
                cell.Colspan = 2;//�ϲ���

            }
            if (i == 3)
            {
                cell.Colspan = 3;//�ϲ���
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
    /// ʵѵ�� �б�����
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

                cell.Colspan = 19;//�ϲ���

            }
            if (i == 1 || i == 2)
            {
                cell.Rowspan = 2;//�ϲ���

            }
            if (i == 3)
            {
                cell.Colspan = 6;//�ϲ���
            }
            if (i == 4 || i == 7)
            {
                cell.Colspan = 3;//�ϲ���
            }
            if (i == 5)
            {
                cell.Rowspan = 2;//�ϲ���
            }
            if (i == 6 || i == 8)
            {
                cell.Colspan = 2;//�ϲ���
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
    /// ʵѵ�� �б�����
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

                cell.Colspan = 21;//�ϲ���

            }
            if (i == 1 || i == 2)
            {
                cell.Rowspan = 2;//�ϲ���

            }
            if (i == 3)
            {
                cell.Colspan = 2;//�ϲ���
            }
            if (i == 4 || i == 5 || i == 7)
            {
                cell.Colspan = 3;//�ϲ���
            }
            if (i == 6)
            {
                cell.Colspan = 2;//�ϲ���
            }
            if (i == 8)
            {
                cell.Rowspan = 2;//�ϲ���
            }
            if (i == 9)
            {
                cell.Colspan = 5;//�ϲ���
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

                cell.Rowspan = 2;//�ϲ���

            }
            if (i == 1 || i == 2)
            {
                cell.Colspan = 2;//�ϲ���

            }
            if (i == 3)
            {
                cell.Colspan = 3;//�ϲ���
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
    /// �ո�
    /// </summary>
    public void Space()
    {
        Paragraph content = new Paragraph(new Chunk(" ", secondTitleFont));
        document.Add(content);
    }
    /// <summary>
    /// ������������
    /// </summary>
    /// <param name="content">����</param>
    /// <param name="font">����</param>
    /// <param name="type">��ʽ,1Ϊ����</param>
    public void AddContent(string content, Font font, int type = 0)
    {
        Paragraph contentP = new Paragraph(new Chunk(content, font));
        contentP.Alignment = type;
        document.Add(contentP);
    }
    /// <summary>
    /// ����ͼƬ
    /// </summary>
    /// <param name="imageName"></param>
    /// <param name="scale"></param>
    public void AddImage(string imageName)
    {
        string path = Application.streamingAssetsPath + "/ReportRes/" + imageName;
        if (!File.Exists(path)) return;
        Image image = Image.GetInstance(Application.streamingAssetsPath + "/ReportRes/" + imageName);

        //Image image = Resources.Load<Image>(imageName);
        //���ﶼ��ͼƬ��ԭʼ�Ŀ����߶�
        float resizedWidht = image.Width;
        float resizedHeight = image.Height;
        image.ScaleToFit(475, 325);
        image.Alignment = Element.ALIGN_JUSTIFIED;
        document.Add(image);
    }
    /// <summary>
    /// �µ�һҳ
    /// </summary>
    public void NewPage()
    {
        document.NewPage();
    }
    /// <summary>
    /// �ر��ĵ�
    /// </summary>
    public void ClosePDF()
    {
        document.Close();
    }
}





