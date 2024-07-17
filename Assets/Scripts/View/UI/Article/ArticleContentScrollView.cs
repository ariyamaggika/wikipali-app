using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static ArticleController;
using static ArticleManager;
using UnityEngine.UIElements;
//��ʾ��������ͨ��prefab                                                             

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
    #region ��ʾ�������ݲ���
    public void InitPaliScroll()
    {
        //��ʼ������λ��Ϊԭ��
        paliScrollContent.localPosition = Vector3.zero;
        //����content text�б�
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

    //    //�����ϴ�Ԥ����¼
    //    string channel = "";
    //    currentChannelName = "p��liԭ��";
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
    //    //��������

    //    currentChannelId = channel;
    //    nextAndPrevGroupView.SetChapter(book, (isTrans ? channel : ""), isTrans);
    //    contentViewGO.SetActive(true);
    //    if (articleView != null)
    //        articleView.listViewGO.SetActive(false);
    //    else if (newArticleView != null)
    //        newArticleView.listViewGO.SetActive(false);
    //    //articleView.listViewGO.SetActive(false);
    //    //ÿ50���½�һ��text
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
    //        Debug.LogError("��Ԥ����book id:" + book.id + "  û���������� text = null");
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
    //        //????????����RegexHypertext,����Ҫ����text���С����Ȼƥ���λ�����ҵģ����濼�ǻ��ɣ�TextMeshPro
    //        //text[i] = text[i] + "\r\n \r\n \r\n \r\n \r\n \r\n";
    //        contentTextInst.text = MarkdownText.PreprocessText(text[i]);
    //        inst.SetActive(true);
    //        contentTextInst.rectTransform.sizeDelta = new Vector2(contentTextInst.rectTransform.sizeDelta.x, textRulerTMP.rectTransform.sizeDelta.y);// new Vector2(PaliContentTextRect.sizeDelta.x, PaliContentText.textComponent.fontSize * (lineCount + 1));
    //        contentList.Add(inst);
    //        if (i != 0)
    //            contentTextInst.enabled = false;
    //    }
    //    //��һ֡����λ��
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
        //������ķ�������ȫ�������Ķ�
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
            //��ʼת�ջ�����
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
        //ֹͣת�ջ�����
        articleView.articleLoadingView.StopLoading();
        if (dl != null && dl.Count > 0)
        {
            articleView.contentView.ShowPaliContentTransCommon(tempBook, tempCNode, dl, true);
        }
        return null;
    }
    //ȥ�����ŵ�
    List<string> textBackup = new List<string>();
    //ûȥ�����ŵ�
    List<string> textBackupOrign = new List<string>();
    string HIGHLIGHT_COLOR = "#ffa500ff";
    //�ʶ������ýӿڣ���������UI��text
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
    //��ԭ����������
    public void RestoreTextHighLight()
    {
        int l = textBackupOrign.Count;
        for (int i = 0; i < l; i++)
        {
            TextMeshProUGUI contentTextInst = contentList[i].GetComponent<TextMeshProUGUI>();
            contentTextInst.text = MarkdownText.PreprocessText(textBackupOrign[i]);
        }
    }
    //�ղؼе㿪���£��ָ�������¶�ջ·��
    //todo
    public void RepairBookTreeNodeStack()
    {
        //bookTreeNodeStack
    }
    public void ShowPaliContentFromStar(int bookID, int bookParagraph, int bookChapterLen, string channelId, List<SentenceDBData> transOnlineData = null)
    {
        //???ArticleContentScrollView��������ƿ��أ�û�ҵ�
        this.gameObject.SetActive(true);
        //�����ϴ�Ԥ����¼
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
        //ÿ50���½�һ��text
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
            Debug.LogError("��Ԥ����book id:" + book.id + "  û���������� text = null");
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
            //????????����RegexHypertext,����Ҫ����text���С����Ȼƥ���λ�����ҵģ����濼�ǻ��ɣ�TextMeshPro
            starText[i] = starText[i] + "\r\n \r\n \r\n \r\n \r\n \r\n";
            contentTextInst.text = MarkdownText.PreprocessText(starText[i]);
            inst.SetActive(true);
            contentTextInst.rectTransform.sizeDelta = new Vector2(contentTextInst.rectTransform.sizeDelta.x, textRulerTMP.rectTransform.sizeDelta.y);// new Vector2(PaliContentTextRect.sizeDelta.x, PaliContentText.textComponent.fontSize * (lineCount + 1));
            contentList.Add(inst);
            if (i != 0)
                contentTextInst.enabled = false;
        }
        //��һ֡����λ��
        StartCoroutine(SetTMPEnabledCtrl());
        textRulerTMP.gameObject.SetActive(false);
        nextAndPrevGroup.SetAsLastSibling();
        //��ʾroot name
        //if (articleView != null)
        //    //articleView.SetTitleRootPath("");
        //    articleView.SetTitleRootPath(book.translateName);
        BookDBData bdd = ArticleManager.Instance().GetBookChildrenFromID(bookID, bookParagraph);
        //todo �������ִӱ��⴫��
        if (isTrans)
        {
            ChapterDBData chapter = ArticleManager.Instance().GetChapter(bookID, bookParagraph, channel);
            cdata.name = chapter.title;
        }
        if (bdd != null && articleView != null)
            articleView.SetArticleBookTreeNodeStack(bookID, bookParagraph, bdd.level, isTrans, cdata.name);
    }

    //����Text�б�GO
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
    #region �����Ķ�
    //�������±��ػ�ȡ?
    //�����������߻�ȡ
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

    //    //�����ϴ�Ԥ����¼
    //    string channel = "";
    //    currentChannelName = "p��liԭ��";
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
    //    //��������

    //    currentChannelId = channel;
    //    nextAndPrevGroupView.SetChapter(book, (isTrans ? channel : ""), isTrans);
    //    contentViewGO.SetActive(true);
    //    if (articleView != null)
    //        articleView.listViewGO.SetActive(false);
    //    else if (newArticleView != null)
    //        newArticleView.listViewGO.SetActive(false);
    //    //ÿ50���½�һ��text
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
    //        Debug.LogError("��Ԥ����book id:" + book.id + "  û���������� text = null");
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
    //        //????????����RegexHypertext,����Ҫ����text���С����Ȼƥ���λ�����ҵģ����濼�ǻ��ɣ�TextMeshPro
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
    //    //��һ֡����λ��
    //    StartCoroutine(SetTMPEnabledCtrl());

    //    textRulerTMP.gameObject.SetActive(false);
    //    nextAndPrevGroup.SetAsLastSibling();

    //    ArticleManager.Instance().SetArticleStar(book.translateName, book.id, book.paragraph, book.chapter_len, channel);
    //    SettingManager.Instance().SaveOpenLastArticle(book.id, book.paragraph, book.chapter_len, channel);
    //    //PaliContentText.lin
    //}

    //�������߰�ͨ�÷���
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

        //�����ϴ�Ԥ����¼
        string channel = "";
        currentChannelName = "p��liԭ��";
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
        //��������

        currentChannelId = channel;
        nextAndPrevGroupView.SetChapter(book, (isTrans ? channel : ""), isTrans);
        contentViewGO.SetActive(true);
        if (articleView != null)
            articleView.listViewGO.SetActive(false);
        else if (newArticleView != null)
            newArticleView.listViewGO.SetActive(false);
        //ÿ50���½�һ��text
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
            Debug.LogError("��Ԥ����book id:" + book.id + "  û���������� text = null");
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
            //????????����RegexHypertext,����Ҫ����text���С����Ȼƥ���λ�����ҵģ����濼�ǻ��ɣ�TextMeshPro
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
        //��һ֡����λ��
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
