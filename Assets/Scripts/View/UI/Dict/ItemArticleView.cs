using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static ArticleController;
using static ArticleManager;
using static DictManager;

public class ItemArticleView : MonoBehaviour
{
    //单词列表面板
    public RectTransform SummaryScrollView;
    //单词详情面板
    public RectTransform DetailScrollView;
    public Button btn;
    public Text nameTxt;
    public Text detailTxt;
    public DicView dicView;
    SentenceDBData sentenceInfo;
    BookDBData bookData;
    ChapterDBData chapterInfo;
    public void SetArticle(SentenceDBData _sentenceInfo, BookDBData _bookData)
    {
        sentenceInfo = _sentenceInfo;
        detailTxt.text = sentenceInfo.content;
        bookData = _bookData;// ArticleManager.Instance().GetBookChildrenFromID(sentenceInfo.bookID, sentenceInfo.paragraph);
        //bookData = ArticleManager.Instance().GetBookChildrenFromID(sentenceInfo.bookID, bookData.parent);
        string ts = "";
        bool isHaveTs = ArticleController.Instance().tsBookDic.TryGetValue(bookData.id, out ts);
        if (isHaveTs)
            bookData.toc = ts;
        if (string.IsNullOrEmpty(sentenceInfo.channel_id))
        {
        }
        else
        {
            // bookData = ArticleManager.Instance().GetBookChildrenFromID(sentenceInfo.bookID, sentenceInfo.paragraph);
            //ChannelChapterDBData channelData = ArticleManager.Instance().GetChannelDataByID(sentenceInfo.channel_id);
            ChapterDBData chapter = ArticleManager.Instance().GetChapter(bookData.id, sentenceInfo.paragraph, sentenceInfo.channel_id);
            if (chapter != null)
                bookData.toc = chapter.title;
        }
        nameTxt.text = bookData.toc;

        //int chapter_len = 0;
        //if (data != null)
        //    chapter_len = data.chapter_len;
        //bookChapterLen = chapter_len;
        //mainView.articleView.contentView.ShowPaliContentFromStar(bookID, bookParagraph, bookChapterLen, channelId);

    }
    public void SetArticle(ChapterDBData _chapterInfo, BookDBData _bookData)
    {
        chapterInfo = _chapterInfo;
        detailTxt.text = "";
        bookData = _bookData;// ArticleManager.Instance().GetBookChildrenFromID(chapterInfo.bookID, chapterInfo.paragraph);
        //bookData = ArticleManager.Instance().GetBookChildrenFromID(chapterInfo.bookID, bookData.parent);
        //string ts = "";
        //bool isHaveTs = ArticleController.Instance().tsBookDic.TryGetValue(bookData.id, out ts);
        //if (isHaveTs)
        //    bookData.toc = ts;
        //if (string.IsNullOrEmpty(chapterInfo.channel_id))
        //{
        //}
        //else
        //{
        //    // bookData = ArticleManager.Instance().GetBookChildrenFromID(sentenceInfo.bookID, sentenceInfo.paragraph);
        //    //ChannelChapterDBData channelData = ArticleManager.Instance().GetChannelDataByID(sentenceInfo.channel_id);
        //    ChapterDBData chapter = ArticleManager.Instance().GetChapter(bookData.id, chapterInfo.paragraph, chapterInfo.channel_id);
        //    if (chapter != null)
        //        bookData.toc = chapter.title;
        //}
        nameTxt.text = chapterInfo.title;

        //int chapter_len = 0;
        //if (data != null)
        //    chapter_len = data.chapter_len;
        //bookChapterLen = chapter_len;
        //mainView.articleView.contentView.ShowPaliContentFromStar(bookID, bookParagraph, bookChapterLen, channelId);

    }
    // Start is called before the first frame update
    void Start()
    {
        //btn = this.GetComponent<Button>();
        btn.onClick.AddListener(OnBtnClick);

    }
    public void OnBtnClick()
    {
        GameManager.Instance().ShowArticle(bookData.id, bookData.paragraph, bookData.chapter_len, sentenceInfo == null ? chapterInfo.channel_id : sentenceInfo.channel_id);
    }
    //void SetSummaryOff()
    //{
    //    DetailScrollView.gameObject.SetActive(true);
    //    SummaryScrollView.gameObject.SetActive(false);
    //}
    // Update is called once per frame
    void Update()
    {

    }
}
