using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static ArticleController;
using static ArticleManager;
using UnityEngine.UIElements;
//显示文章内容通用prefab                                                             

public class ArticleContentScrollView : MonoBehaviour
{
    public NewArticleView newArticleView;
    public ArticleView articleView;
    public GameObject contentViewGO;
    public InputField paliContentText;
    public RectTransform paliScrollContent;
    public RectTransform nextAndPrevGroup;
    public NextPrevGroupView nextAndPrevGroupView;
    //public RegexHypertext contentText;
    public TextMeshProUGUI contentTextTMP;
    public TextMeshProUGUI textRulerTMP;
    // Start is called before the first frame update
    void Start()
    {

    }
    #region 显示文章内容部分
    public void InitPaliScroll()
    {
        //初始化文章位置为原点
        paliScrollContent.localPosition = Vector3.zero;
        //清理content text列表
        DestroyTextList();
    }
    List<GameObject> contentList = new List<GameObject>();
    public List<string> articleContent;
    public ChapterDBData currentChapterData;
    public string currentChannelId;
    public string currentChannelName;
    public Book currentBook;
    //public void ShowPaliContentTrans(Book book, ChapterDBData cNode, bool isTrans)
    //{
    //    //?????????????????????
    //    gameObject.SetActive(true);

    //    InitPaliScroll();
    //    if (isTrans && cNode == null)
    //        Debug.LogError("!!!!");
    //    if (isTrans && cNode.channelData == null)
    //        Debug.LogError("!!!!");
    //    if (isTrans && cNode.channelData != null && cNode.channelData.channel_id == null)
    //        Debug.LogError("!!!!");

    //    //保存上次预览记录
    //    string channel = "";
    //    currentChannelName = "pāli原文";
    //    if (isTrans)
    //    {
    //        if (cNode.channelData == null)
    //        {
    //            channel = cNode.channel_id;
    //            currentChannelName = cNode.title;
    //        }
    //        else
    //        {
    //            channel = cNode.channelData.channel_id;
    //            currentChannelName = cNode.channelData.name;
    //        }
    //    }
    //    //测试数据

    //    currentChannelId = channel;
    //    nextAndPrevGroupView.SetChapter(book, (isTrans ? channel : ""), isTrans);
    //    contentViewGO.SetActive(true);
    //    if (articleView != null)
    //        articleView.listViewGO.SetActive(false);
    //    else if (newArticleView != null)
    //        newArticleView.listViewGO.SetActive(false);
    //    //articleView.listViewGO.SetActive(false);
    //    //每50行新建一个text
    //    List<string> text;
    //    List<string> sentence;
    //    currentChapterData = cNode;
    //    currentBook = book;
    //    (text, sentence) = ArticleController.Instance().GetPaliContentTransText(book, (isTrans ? cNode.channelData : null), isTrans);
    //    CommonTool.DeepCopyStringList(textBackup, text);
    //    CommonTool.DeepCopyStringList(textBackupOrign, text);
    //    if (SettingManager.Instance().GetPaliRemoveBracket() == 1)
    //        textBackup = MarkdownText.RemoveBracketStringList(textBackup);
    //    //textBackup = text;
    //    articleContent = sentence;
    //    if (text == null)
    //    {
    //        Debug.LogError("【预警】book id:" + book.id + "  没有文章内容 text = null");
    //        return;
    //    }
    //    textRulerTMP.gameObject.SetActive(true);
    //    int l = text.Count;
    //    for (int i = 0; i < l; i++)
    //    {
    //        textRulerTMP.text = text[i];
    //        LayoutRebuilder.ForceRebuildLayoutImmediate(textRulerTMP.rectTransform);
    //        GameObject inst = Instantiate(contentTextTMP.gameObject, contentTextTMP.transform.parent);
    //        inst.name = i.ToString();
    //        inst.transform.position = contentTextTMP.transform.position;
    //        TextMeshProUGUI contentTextInst = inst.GetComponent<TextMeshProUGUI>();
    //        //????????兼容RegexHypertext,文字要超过text框大小，不然匹配的位置是乱的，后面考虑换成，TextMeshPro
    //        //text[i] = text[i] + "\r\n \r\n \r\n \r\n \r\n \r\n";
    //        contentTextInst.text = MarkdownText.PreprocessText(text[i]);
    //        inst.SetActive(true);
    //        contentTextInst.rectTransform.sizeDelta = new Vector2(contentTextInst.rectTransform.sizeDelta.x, textRulerTMP.rectTransform.sizeDelta.y);// new Vector2(PaliContentTextRect.sizeDelta.x, PaliContentText.textComponent.fontSize * (lineCount + 1));
    //        contentList.Add(inst);
    //        if (i != 0)
    //            contentTextInst.enabled = false;
    //    }
    //    //下一帧计算位置
    //    StartCoroutine(SetTMPEnabledCtrl());

    //    textRulerTMP.gameObject.SetActive(false);
    //    nextAndPrevGroup.SetAsLastSibling();

    //    ArticleManager.Instance().SetArticleStar(book.translateName, book.id, book.paragraph, book.chapter_len, channel);
    //    SettingManager.Instance().SaveOpenLastArticle(book.id, book.paragraph, book.chapter_len, channel);
    //    //PaliContentText.lin
    //}
    public Book tempBook;
    public ChapterDBData tempCNode;
    public void ShowPaliContentTransAgent(Book book, ChapterDBData cNode, bool isTrans)
    {
        //有网络的翻译文章全部在线阅读
        if (NetworkMangaer.Instance().CheckIsHaveNetwork() && isTrans)
        {
            BookDBData data = ArticleManager.Instance().GetBookChildrenFromID(book.id, book.paragraph);
            int chapter_len = 0;
            if (data != null)
                chapter_len = data.chapter_len;

            int paraMin = book.paragraph;
            int paraMax = book.paragraph + chapter_len;
            tempBook = book;
            tempCNode = cNode;
            //开始转菊花加载
            if (articleView != null)
                articleView.articleLoadingView.StartLoading(() => { ShowPaliContentTransAgent(book, cNode, isTrans); return null; });
            C2SArticleGetNewDBInfo.GetSentenceData(book.id, cNode.channel_id, paraMin, paraMax, OnLineArticleCallBack);
        }
        else
        {
            ShowPaliContentTransCommon(book, cNode, null, isTrans);
        }

    }
    public object OnLineArticleCallBack(List<SentenceDBData> dl)
    {
        //停止转菊花加载
        articleView.articleLoadingView.StopLoading();
        if (dl != null && dl.Count > 0)
        {
            articleView.contentView.ShowPaliContentTransCommon(tempBook, tempCNode, dl, true);
        }
        return null;
    }
    //去掉括号的
    List<string> textBackup = new List<string>();
    //没去掉括号的
    List<string> textBackupOrign = new List<string>();
    string HIGHLIGHT_COLOR = "#ffa500ff";
    //朗读高亮用接口，重新设置UI的text
    public void SetTextHighLight(int textID, int hlStartID, int hlLength)
    {
        //int l = textBackup.Count;
        //for (int i = 0; i < l; i++)
        //{
        TextMeshProUGUI contentTextInst = contentList[textID].GetComponent<TextMeshProUGUI>();
        string newText = textBackup[textID];
        newText = newText.Substring(0, hlStartID) + "<color=" + HIGHLIGHT_COLOR + ">" +
           newText.Substring(hlStartID, hlLength) + "</color>" + newText.Substring(hlStartID + hlLength);
        contentTextInst.text = MarkdownText.PreprocessText(newText);
        //}
    }
    //还原高亮的文字
    public void RestoreTextHighLight()
    {
        int l = textBackupOrign.Count;
        for (int i = 0; i < l; i++)
        {
            TextMeshProUGUI contentTextInst = contentList[i].GetComponent<TextMeshProUGUI>();
            contentTextInst.text = MarkdownText.PreprocessText(textBackupOrign[i]);
        }
    }
    //收藏夹点开文章，恢复书和文章堆栈路径
    //todo
    public void RepairBookTreeNodeStack()
    {
        //bookTreeNodeStack
    }
    public void ShowPaliContentFromStar(int bookID, int bookParagraph, int bookChapterLen, string channelId, List<SentenceDBData> transOnlineData = null)
    {
        //???ArticleContentScrollView在哪里控制开关？没找到
        this.gameObject.SetActive(true);
        //保存上次预览记录
        SettingManager.Instance().SaveOpenLastArticle(bookID, bookParagraph, bookChapterLen, channelId);
        bool isTrans = !string.IsNullOrEmpty(channelId);
        InitPaliScroll();
        string channel = channelId;
        Book book = new Book();
        book.id = bookID;
        book.paragraph = bookParagraph;
        book.chapter_len = bookChapterLen;
        nextAndPrevGroupView.SetChapter(book, (isTrans ? channel : ""), isTrans);
        contentViewGO.SetActive(true);
        if (articleView != null)
            articleView.listViewGO.SetActive(false);
        //每50行新建一个text
        List<string> starText;
        List<string> sentence;
        ChannelChapterDBData cdata = new ChannelChapterDBData();
        cdata.channel_id = channel;
        (starText, sentence) = ArticleController.Instance().GetPaliContentTransText(book, (isTrans ? cdata : null), isTrans, transOnlineData);
        CommonTool.DeepCopyStringList(textBackup, starText);
        CommonTool.DeepCopyStringList(textBackupOrign, starText);
        if (SettingManager.Instance().GetPaliRemoveBracket() == 1)
            textBackup = MarkdownText.RemoveBracketStringList(textBackup);
        articleContent = sentence;
        if (starText == null)
        {
            Debug.LogError("【预警】book id:" + book.id + "  没有文章内容 text = null");
            return;
        }
        textRulerTMP.gameObject.SetActive(true);
        int l = starText.Count;
        for (int i = 0; i < l; i++)
        {
            textRulerTMP.text = starText[i];
            LayoutRebuilder.ForceRebuildLayoutImmediate(textRulerTMP.rectTransform);
            GameObject inst = Instantiate(contentTextTMP.gameObject, contentTextTMP.transform.parent);
            inst.name = i.ToString();
            inst.transform.position = contentTextTMP.transform.position;
            TextMeshProUGUI contentTextInst = inst.GetComponent<TextMeshProUGUI>();
            //????????兼容RegexHypertext,文字要超过text框大小，不然匹配的位置是乱的，后面考虑换成，TextMeshPro
            starText[i] = starText[i] + "\r\n \r\n \r\n \r\n \r\n \r\n";
            contentTextInst.text = MarkdownText.PreprocessText(starText[i]);
            inst.SetActive(true);
            contentTextInst.rectTransform.sizeDelta = new Vector2(contentTextInst.rectTransform.sizeDelta.x, textRulerTMP.rectTransform.sizeDelta.y);// new Vector2(PaliContentTextRect.sizeDelta.x, PaliContentText.textComponent.fontSize * (lineCount + 1));
            contentList.Add(inst);
            if (i != 0)
                contentTextInst.enabled = false;
        }
        //下一帧计算位置
        StartCoroutine(SetTMPEnabledCtrl());
        textRulerTMP.gameObject.SetActive(false);
        nextAndPrevGroup.SetAsLastSibling();
        //显示root name
        //if (articleView != null)
        //    //articleView.SetTitleRootPath("");
        //    articleView.SetTitleRootPath(book.translateName);
        BookDBData bdd = ArticleManager.Instance().GetBookChildrenFromID(bookID, bookParagraph);
        //todo 翻译名字从标题传入
        if (isTrans)
        {
            ChapterDBData chapter = ArticleManager.Instance().GetChapter(bookID, bookParagraph, channel);
            cdata.name = chapter.title;
        }
        if (bdd != null && articleView != null)
            articleView.SetArticleBookTreeNodeStack(bookID, bookParagraph, bdd.level, isTrans, cdata.name);
    }

    //销毁Text列表GO
    private void DestroyTextList()
    {
        int length = contentList.Count;
        if (length == 0)
            return;
        for (int i = 0; i < length; i++)
        {
            Destroy(contentList[i]);
        }
        contentList.Clear();
    }
    #endregion
    #region 在线阅读
    //离线文章本地获取?
    //翻译内容在线获取
    //public void ShowPaliContentTransOnline(Book book, ChapterDBData cNode, List<SentenceDBData> transOnlineData, bool isTrans)
    //{
    //    //?????????????????????
    //    gameObject.SetActive(true);

    //    InitPaliScroll();
    //    if (isTrans && cNode == null)
    //        Debug.LogError("!!!!");
    //    if (isTrans && cNode.channelData == null)
    //        Debug.LogError("!!!!");
    //    if (isTrans && cNode.channelData != null && cNode.channelData.channel_id == null)
    //        Debug.LogError("!!!!");

    //    //保存上次预览记录
    //    string channel = "";
    //    currentChannelName = "pāli原文";
    //    if (isTrans)
    //    {
    //        if (cNode.channelData == null)
    //        {
    //            channel = cNode.channel_id;
    //            currentChannelName = cNode.title;
    //        }
    //        else
    //        {
    //            channel = cNode.channelData.channel_id;
    //            currentChannelName = cNode.channelData.name;
    //        }
    //    }
    //    //测试数据

    //    currentChannelId = channel;
    //    nextAndPrevGroupView.SetChapter(book, (isTrans ? channel : ""), isTrans);
    //    contentViewGO.SetActive(true);
    //    if (articleView != null)
    //        articleView.listViewGO.SetActive(false);
    //    else if (newArticleView != null)
    //        newArticleView.listViewGO.SetActive(false);
    //    //每50行新建一个text
    //    List<string> text;
    //    List<string> sentence;
    //    currentChapterData = cNode;
    //    currentBook = book;
    //    (text, sentence) = ArticleController.Instance().GetPaliContentTransText(book, (isTrans ? cNode.channelData : null), isTrans, transOnlineData);
    //    CommonTool.DeepCopyStringList(textBackup, text);
    //    CommonTool.DeepCopyStringList(textBackupOrign, text);
    //    if (SettingManager.Instance().GetPaliRemoveBracket() == 1)
    //        textBackup = MarkdownText.RemoveBracketStringList(textBackup);
    //    //textBackup = text;
    //    articleContent = sentence;
    //    if (text == null)
    //    {
    //        Debug.LogError("【预警】book id:" + book.id + "  没有文章内容 text = null");
    //        return;
    //    }
    //    textRulerTMP.gameObject.SetActive(true);
    //    int l = text.Count;
    //    for (int i = 0; i < l; i++)
    //    {
    //        textRulerTMP.text = text[i];
    //        LayoutRebuilder.ForceRebuildLayoutImmediate(textRulerTMP.rectTransform);
    //        GameObject inst = Instantiate(contentTextTMP.gameObject, contentTextTMP.transform.parent);
    //        inst.name = i.ToString();
    //        inst.transform.position = contentTextTMP.transform.position;
    //        TextMeshProUGUI contentTextInst = inst.GetComponent<TextMeshProUGUI>();
    //        //????????兼容RegexHypertext,文字要超过text框大小，不然匹配的位置是乱的，后面考虑换成，TextMeshPro
    //        //text[i] = text[i] + "\r\n \r\n \r\n \r\n \r\n \r\n";
    //        contentTextInst.text = MarkdownText.PreprocessText(text[i]);
    //        inst.SetActive(true);
    //        contentTextInst.rectTransform.sizeDelta = new Vector2(contentTextInst.rectTransform.sizeDelta.x, textRulerTMP.rectTransform.sizeDelta.y);// new Vector2(PaliContentTextRect.sizeDelta.x, PaliContentText.textComponent.fontSize * (lineCount + 1));
    //        contentList.Add(inst);
    //        if (i != 0)
    //            contentTextInst.enabled = false;
    //        //TMPScrollEnableSelf tmpEnabledCtrl = contentTextInst.gameObject.AddComponent<TMPScrollEnableSelf>();
    //        //tmpEnabledCtrl.OnInit(paliScrollContent,contentTextInst);
    //    }
    //    //下一帧计算位置
    //    StartCoroutine(SetTMPEnabledCtrl());

    //    textRulerTMP.gameObject.SetActive(false);
    //    nextAndPrevGroup.SetAsLastSibling();

    //    ArticleManager.Instance().SetArticleStar(book.translateName, book.id, book.paragraph, book.chapter_len, channel);
    //    SettingManager.Instance().SaveOpenLastArticle(book.id, book.paragraph, book.chapter_len, channel);
    //    //PaliContentText.lin
    //}

    //在线离线包通用方法
    public void ShowPaliContentTransCommon(Book book, ChapterDBData cNode, List<SentenceDBData> transOnlineData, bool isTrans)
    {
        //?????????????????????
        gameObject.SetActive(true);

        InitPaliScroll();
        if (isTrans && cNode == null)
            Debug.LogError("!!!!");
        if (isTrans && cNode.channelData == null)
            Debug.LogError("!!!!");
        if (isTrans && cNode.channelData != null && cNode.channelData.channel_id == null)
            Debug.LogError("!!!!");

        //保存上次预览记录
        string channel = "";
        currentChannelName = "pāli原文";
        if (isTrans)
        {
            if (cNode.channelData == null)
            {
                channel = cNode.channel_id;
                currentChannelName = cNode.title;
            }
            else
            {
                channel = cNode.channelData.channel_id;
                currentChannelName = cNode.channelData.name;
            }
        }
        //测试数据

        currentChannelId = channel;
        nextAndPrevGroupView.SetChapter(book, (isTrans ? channel : ""), isTrans);
        contentViewGO.SetActive(true);
        if (articleView != null)
            articleView.listViewGO.SetActive(false);
        else if (newArticleView != null)
            newArticleView.listViewGO.SetActive(false);
        //每50行新建一个text
        List<string> text;
        List<string> sentence;
        currentChapterData = cNode;
        currentBook = book;
        (text, sentence) = ArticleController.Instance().GetPaliContentTransText(book, (isTrans ? cNode.channelData : null), isTrans, transOnlineData);
        CommonTool.DeepCopyStringList(textBackup, text);
        CommonTool.DeepCopyStringList(textBackupOrign, text);
        if (SettingManager.Instance().GetPaliRemoveBracket() == 1)
            textBackup = MarkdownText.RemoveBracketStringList(textBackup);
        //textBackup = text;
        articleContent = sentence;
        if (text == null)
        {
            Debug.LogError("【预警】book id:" + book.id + "  没有文章内容 text = null");
            return;
        }
        textRulerTMP.gameObject.SetActive(true);
        int l = text.Count;
        for (int i = 0; i < l; i++)
        {
            textRulerTMP.text = text[i];
            LayoutRebuilder.ForceRebuildLayoutImmediate(textRulerTMP.rectTransform);
            GameObject inst = Instantiate(contentTextTMP.gameObject, contentTextTMP.transform.parent);
            inst.name = i.ToString();
            inst.transform.position = contentTextTMP.transform.position;
            TextMeshProUGUI contentTextInst = inst.GetComponent<TextMeshProUGUI>();
            //????????兼容RegexHypertext,文字要超过text框大小，不然匹配的位置是乱的，后面考虑换成，TextMeshPro
            //text[i] = text[i] + "\r\n \r\n \r\n \r\n \r\n \r\n";
            contentTextInst.text = MarkdownText.PreprocessText(text[i]);
            inst.SetActive(true);
            contentTextInst.rectTransform.sizeDelta = new Vector2(contentTextInst.rectTransform.sizeDelta.x, textRulerTMP.rectTransform.sizeDelta.y);// new Vector2(PaliContentTextRect.sizeDelta.x, PaliContentText.textComponent.fontSize * (lineCount + 1));
            contentList.Add(inst);
            if (i != 0)
                contentTextInst.enabled = false;
            //TMPScrollEnableSelf tmpEnabledCtrl = contentTextInst.gameObject.AddComponent<TMPScrollEnableSelf>();
            //tmpEnabledCtrl.OnInit(paliScrollContent,contentTextInst);
        }
        //下一帧计算位置
        StartCoroutine(SetTMPEnabledCtrl());

        textRulerTMP.gameObject.SetActive(false);
        nextAndPrevGroup.SetAsLastSibling();

        ArticleManager.Instance().SetArticleStar(book.translateName, book.id, book.paragraph, book.chapter_len, channel);
        SettingManager.Instance().SaveOpenLastArticle(book.id, book.paragraph, book.chapter_len, channel);
        //PaliContentText.lin
    }
    IEnumerator SetTMPEnabledCtrl()
    {
        yield return null;
        int c = contentList.Count;
        for (int i = 0; i < c; i++)
        {
            TMPScrollEnableSelf tmpEnabledCtrl = contentList[i].AddComponent<TMPScrollEnableSelf>();
            tmpEnabledCtrl.OnInit(paliScrollContent, contentList[i].GetComponent<TextMeshProUGUI>());
        }
    }
    #endregion
}
