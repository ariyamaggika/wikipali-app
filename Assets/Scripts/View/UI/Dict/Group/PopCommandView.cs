using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static ArticleController;
using static ArticleManager;
using static DictManager;

public class PopCommandView : MonoBehaviour
{
    public Button cancelBtn;
    public Button closeBackGroupBtn;
    public Button okBtn;
    public Text contentText;
    public ArticleView articleView;
    string dicWord;
    bool isDic = false;
    int artBookID;
    int artBookParagraph;
    int artBookChapterLen;
    string artChannelId;
    string artTitle;

    // Start is called before the first frame update
    void Start()
    {
        cancelBtn.onClick.AddListener(OnCloseBackBtnClick);
        closeBackGroupBtn.onClick.AddListener(OnCloseBackBtnClick);
        okBtn.onClick.AddListener(OnOkBtnClick);

    }
    public void Init(int bookID, int bookParagraph, int bookChapterLen, string channelId, string title)
    {
        artBookID = bookID;
        artBookParagraph = bookParagraph;
        artBookChapterLen = bookChapterLen;
        artChannelId = channelId;
        artTitle = title;
        isDic = false;
        contentText.text = title;
    }
    public void Init(string word)
    {
        dicWord = word;
        isDic = true;
    }
    public void OnCloseBackBtnClick()
    {
        this.gameObject.SetActive(false);
    }
    public void OnOkBtnClick()
    {
        if (isDic)
        {
            GameManager.Instance().ShowDicWord(dicWord);
        }
        else
        {
            GameManager.Instance().ShowArticle(artBookID, artBookParagraph,
                artBookChapterLen, artChannelId);
        }
        this.gameObject.SetActive(false);
    }
    void Update()
    {

    }
}
