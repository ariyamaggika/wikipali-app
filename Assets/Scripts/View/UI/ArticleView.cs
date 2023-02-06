﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static ArticleController;
using static ArticleManager;

public class ArticleView : MonoBehaviour
{
    ArticleController controller = ArticleController.Instance();
    public ArticleNodeItemView nodeItem;
    public ArticleTitleReturnBtn returnBtn;
    //文章标题树
    public GameObject ListViewGO;
    //文章内容 pali原文和翻译
    public GameObject ContentViewGO;
    public InputField PaliContentText;
    public RectTransform PaliContentTextRect;
    public Text contentText;
    public Text textRuler;
    Stack<ArticleTreeNode> articleTreeNodeStack;
    Stack<Book> bookTreeNodeStack;
    // Start is called before the first frame update
    void Start()
    {
        articleTreeNodeStack = new Stack<ArticleTreeNode>();
        bookTreeNodeStack = new Stack<Book>();
        //TODO?:controller在此处初始化不太合理
        controller.Init();
        InitNodeItem(controller.articleTreeNodes.info);
    }
    List<GameObject> nodeList = new List<GameObject>();

    void InitNodeItem(List<ArticleTreeNode> info)
    {
        DestroyNodeList();
        int length = info.Count;
        float height = nodeItem.GetComponent<RectTransform>().sizeDelta.y;
        for (int i = 0; i < length; i++)
        {
            GameObject inst = Instantiate(nodeItem.gameObject, nodeItem.transform.parent);
            inst.transform.position = nodeItem.transform.position;
            inst.GetComponent<RectTransform>().position -= Vector3.up * height * i;

            inst.GetComponent<ArticleNodeItemView>().Init(info[i]);
            inst.SetActive(true);
            nodeList.Add(inst);
        }
    }
    void InitNodeItem(List<Book> info)
    {
        DestroyNodeList();
        int length = info.Count;
        float height = nodeItem.GetComponent<RectTransform>().sizeDelta.y;
        for (int i = 0; i < length; i++)
        {
            GameObject inst = Instantiate(nodeItem.gameObject, nodeItem.transform.parent);
            inst.transform.position = nodeItem.transform.position;
            inst.GetComponent<RectTransform>().position -= Vector3.up * height * i;

            inst.GetComponent<ArticleNodeItemView>().Init(info[i]);
            inst.SetActive(true);
            nodeList.Add(inst);
        }
    }
    //chapter
    void InitNodeItem(Book book)
    {
        List<ChapterDBData> info = book.chapterDBDatas;
        DestroyNodeList();
        int length = info.Count;
        float height = nodeItem.GetComponent<RectTransform>().sizeDelta.y;

        //todo 优化代码
        //第一个按钮是显示原文
        GameObject inst0 = Instantiate(nodeItem.gameObject, nodeItem.transform.parent);
        inst0.transform.position = nodeItem.transform.position;
        inst0.GetComponent<RectTransform>().position -= Vector3.up;
        inst0.GetComponent<ArticleNodeItemView>().InitPali(book);
        inst0.SetActive(true);
        nodeList.Add(inst0);


        for (int i = 0; i < length; i++)
        {
            if (info[i].channelData == null)
                continue;
            GameObject inst = Instantiate(nodeItem.gameObject, nodeItem.transform.parent);
            inst.transform.position = nodeItem.transform.position;
            inst.GetComponent<RectTransform>().position -= Vector3.up * height * (i + 1);

            inst.GetComponent<ArticleNodeItemView>().Init(book, info[i]);
            inst.SetActive(true);
            nodeList.Add(inst);
        }
    }
    //销毁下拉列表GO
    private void DestroyNodeList()
    {
        int length = nodeList.Count;
        if (length == 0)
            return;
        for (int i = 0; i < length; i++)
        {
            Destroy(nodeList[i]);
        }
        nodeList.Clear();
    }
    // Update is called once per frame
    void Update()
    {

    }
    //点击article节点
    public void ArticleNodeBtnClick(ArticleTreeNode info, bool isReturn = false)
    {
        if (!isReturn)
            articleTreeNodeStack.Push(info);
        string aPath = "";
        int length = articleTreeNodeStack.Count;
        //设置标题路径
        ArticleTreeNode[] arr = articleTreeNodeStack.ToArray();
        for (int i = length - 1; i > -1; i--)
        {
            aPath += arr[i].name + "/";
        }
        returnBtn.SetPath(aPath);
        if (info.children != null && info.children.Count > 0)
        {
            InitNodeItem(info.children);
        }
        else
        {
#if DEBUG_PERFORMANCE
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
#endif
            List<Book> book = controller.GetBooks(info);
#if DEBUG_PERFORMANCE
            sw.Stop();
            Debug.LogError("【性能】查询文章耗时：" + sw.ElapsedMilliseconds);
#endif
            InitNodeItem(book);
        }
    }
    //点击细化到book的节点
    public void BookNodeBtnClick(Book info, bool isReturn = false, bool isChild = false)
    {
        if (info.children != null && info.children.Count > 0)
        {
            InitNodeItem(info.children);
            if (!isReturn)
                bookTreeNodeStack.Push(info);
            //TODO?:点进书本只显示书本路径，前面是否需要显示Article路径？
            string aPath = "";
            int length = bookTreeNodeStack.Count;
            Book[] arr = bookTreeNodeStack.ToArray();
            for (int i = length - 1; i > -1; i--)
            {
                aPath += arr[i].toc + "/";
            }
            returnBtn.SetPath(aPath);
        }
        else
        {
            if (!isChild)
            {
                //再次获取子章节
                controller.GetBooksChildrenLevel(info);
                //获取版本风格
                controller.GetChapterChannelData(info);
                BookNodeBtnClick(info, isReturn, true);
            }
            else
            {
                //显示版本风格
                InitNodeItem(info);

                bookTreeNodeStack.Push(info);
                //TODO?:点进书本只显示书本路径，前面是否需要显示Article路径？
                string aPath = "";
                int length = bookTreeNodeStack.Count;
                Book[] arr = bookTreeNodeStack.ToArray();
                for (int i = length - 1; i > -1; i--)
                {
                    aPath += arr[i].toc + "/";
                }
                aPath += "译文版本";
                returnBtn.SetPath(aPath);
            }
        }
    }
    //点击显示channel的节点
    public void ChannelBtnClick()
    {

    }
    public void ReturnBtnClick()
    {
        ContentViewGO.SetActive(false);
        ListViewGO.SetActive(true);

        if (bookTreeNodeStack.Count != 0)
        {
            bookTreeNodeStack.Pop();
            if (bookTreeNodeStack.Count > 0)
            {
                BookNodeBtnClick(bookTreeNodeStack.Peek(), true);
            }
            else
            {
                //returnBtn.Init();
                //设置标题路径
                string aPath = "";
                int length = articleTreeNodeStack.Count;
                ArticleTreeNode[] arr = articleTreeNodeStack.ToArray();
                for (int i = length - 1; i > -1; i--)
                {
                    aPath += arr[i].name + "/";
                }
                returnBtn.SetPath(aPath);
                InitNodeItem(controller.currentBookList);
            }
            return;
        }
        if (articleTreeNodeStack.Count == 0)
            return;
        articleTreeNodeStack.Pop();
        if (articleTreeNodeStack.Count > 0)
        {
            ArticleNodeBtnClick(articleTreeNodeStack.Peek(), true);
        }
        else
        {
            returnBtn.Init();
            InitNodeItem(controller.articleTreeNodes.info);
        }
    }
    #region 显示文章内容部分
    public void ShowPaliContent(Book book)
    {
        ContentViewGO.SetActive(true);
        ListViewGO.SetActive(false);
        string text = controller.GetPaliContentText(book);
        textRuler.gameObject.SetActive(true);
        textRuler.text = text;
        //PaliContentText.text = text;
        contentText.text = text;
        LayoutRebuilder.ForceRebuildLayoutImmediate(textRuler.rectTransform);

        //int lineCount = textRuler.cachedTextGenerator.lineCount;// PaliContentText.textComponent.cachedTextGenerator.lineCount;
        //Debug.LogError(textRuler.rectTransform.sizeDelta.y);
        //Debug.LogError(PaliContentText.textComponent.cachedTextGenerator.lineCount);
        PaliContentTextRect.sizeDelta = new Vector2(PaliContentTextRect.sizeDelta.x, textRuler.rectTransform.sizeDelta.y);// new Vector2(PaliContentTextRect.sizeDelta.x, PaliContentText.textComponent.fontSize * (lineCount + 1));
        textRuler.gameObject.SetActive(false);

        //PaliContentText.lin
    }
    public void ShowPaliContentTrans(Book book, ChapterDBData cNode)
    {
        ContentViewGO.SetActive(true);
        ListViewGO.SetActive(false);
        string text = controller.GetPaliContentTransText(book, cNode.channelData);
        textRuler.gameObject.SetActive(true);
        textRuler.text = text;
        //PaliContentText.text = text;
        contentText.text = text;
        LayoutRebuilder.ForceRebuildLayoutImmediate(textRuler.rectTransform);

        //int lineCount = textRuler.cachedTextGenerator.lineCount;// PaliContentText.textComponent.cachedTextGenerator.lineCount;
        //Debug.LogError(textRuler.rectTransform.sizeDelta.y);
        //Debug.LogError(PaliContentText.textComponent.cachedTextGenerator.lineCount);
        PaliContentTextRect.sizeDelta = new Vector2(PaliContentTextRect.sizeDelta.x, textRuler.rectTransform.sizeDelta.y);// new Vector2(PaliContentTextRect.sizeDelta.x, PaliContentText.textComponent.fontSize * (lineCount + 1));
        textRuler.gameObject.SetActive(false);

        //PaliContentText.lin
    }

    #endregion
}