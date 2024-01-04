using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static ArticleController;
using static ArticleManager;
using static C2SArticleGetNewDBInfo;
using static SettingManager;

public class NewArticleNodeItemView : MonoBehaviour
{
    public Button inArticleBtn;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI subText;
    public TextMeshProUGUI contentText;

    public TextMeshProUGUI progressText;
    public Image progressImg;

    public TextMeshProUGUI userText;
    public TextMeshProUGUI timeText;
    public NewArticleView articleView;

    public Book book;
    public ChapterDBData channel;
    // Start is called before the first frame update
    void Start()
    {
        inArticleBtn.onClick.AddListener(OnInArticleBtnClick);

    }
    public void OnInArticleBtnClick()
    {
        BookDBData data = ArticleManager.Instance().GetBookChildrenFromID(book.id, book.paragraph);
        if (data == null)
            return;
        int paraMin = data.paragraph;
        int paraMax = data.paragraph + data.chapter_len;
        C2SArticleGetNewDBInfo.GetSentenceData(book.id, channel.channel_id, paraMin, paraMax, OnLineArticleCallBack);
        articleView.ArticleNodeBtnClick();
    }
    public object OnLineArticleCallBack(List<SentenceDBData> dl)
    {
        if (dl != null && dl.Count > 0)
        {
            articleView.contentView.ShowPaliContentTransOnline(book, channel, dl, true);
        }
        return null;
    }
    public void Init(NewArticleData data, NewArticleView _articleView)
    {
        articleView = _articleView;
        titleText.text = data.title.Replace("\n","");
        subText.text = data.toc;
        contentText.text = data.summary;
        data.progress = Mathf.Clamp01(data.progress);
        progressText.text = (int)(data.progress * 100) + "%";
        progressImg.fillAmount = data.progress;
        userText.text = data.channel.name;
        timeText.text = CommonTool.FormatDate(DateTime.Parse(data.updated_at));
        book = new Book()
        {
            id = data.book,
            toc = data.toc,
            //chapter_len = data.l
            //level = data.level,
            paragraph = data.para,
            //parentP = data.parent,
            translateName = data.toc,
        };

        channel = new ChapterDBData()
        {
            id = data.channel_id,
            bookID = data.book,
            paragraph = data.para,
            //language = data.la,
            title = data.channel.name,
            channel_id = data.channel_id,
            progress = data.progress,
        };
    }
    // Update is called once per frame
    void Update()
    {

    }
}
