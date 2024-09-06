﻿using I2.Loc;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static ArticleController;

public class StarGroupArticleView : MonoBehaviour
{
    public Toggle starToggle;
    public Button shareBtn;
    public Button shareCommandBtn;
    public Button voiceBtn;
    public AudioSource voiceSource;
    public PopArticleSentenceSelectView popSentenceSelectView;
    public PopView popView;
    public ArticleView articleView;
    public NewArticleView newArticleView;
    //public ShareView shareView;
    // Start is called before the first frame update
    void Awake()
    {
        starToggle.onValueChanged.AddListener(OnToggleValueChanged);
        shareBtn.onClick.AddListener(OnShareBtnClick);
        shareCommandBtn.onClick.AddListener(OnShareCommandBtnClick);
        voiceBtn.onClick.AddListener(OnVoiceBtnClick);

    }
    public static string currVoiceArticle;
    //public void OnVoiceBtnClick()
    //{
    //    string readWord = SpeechGeneration.Instance().ReplaceWord(ArticleController.Instance().testPl);
    //    if (voiceSource.clip != null && currVoiceArticle == readWord)
    //    {
    //        voiceSource.Play();
    //        return;
    //    }
    //    AudioClip ac = SpeechGeneration.Instance().Speek(readWord);
    //    if (ac != null)
    //    {
    //        currVoiceArticle = readWord;
    //        voiceSource.clip = ac;
    //        voiceSource.Play();
    //    }
    //}
    //public void OnVoiceBtnClick()
    //{
    //    string readArticle = SpeechGeneration.Instance().ReplaceWord(ArticleController.Instance().testPl);
    //    //string test = "evaṃ me sutaṃ– ekaṃ samayaṃ bhagavā ";
    //    //string readArticle = SpeechGeneration.Instance().ReplaceWordTGL(test);
    //    if (voiceSource.clip != null && currVoiceArticle == readArticle)
    //    {
    //        voiceSource.Play();
    //        return;
    //    }
    //    AudioClip ac = SpeechGeneration.Instance().SpeekPali(readArticle, -40);
    //    if (ac != null)
    //    {
    //        currVoiceArticle = readArticle;
    //        voiceSource.clip = ac;
    //        voiceSource.Play();
    //    }
    //}
    public void OnVoiceBtnClick()
    {
        //判断是否有网
        if (!NetworkMangaer.Instance().CheckIsHaveNetwork())
        {
            //UITool.ShowToastMessage(this, "无网络连接", 35);
            UITool.ShowToastMessage(this, LocalizationManager.GetTranslation("showToastMessage_NoNetwork"), 35);
            return;
            // return false;
        }
        SpeechManager.Instance().ReadArticleSList(ArticleController.Instance().paliSentenceList,
            ArticleController.Instance().transSentenceList, ArticleController.Instance().trans, voiceSource);
    }
    //public void OnVoiceBtnClick()
    //{
    //    string readArticle = ArticleController.Instance().testCN;
    //    if (voiceSource.clip != null && currVoiceArticle == readArticle)
    //    {
    //        voiceSource.Play();
    //        return;
    //    }
    //    AudioClip ac = SpeechGeneration.Instance().SpeekCN(readArticle);
    //    if (ac != null)
    //    {
    //        currVoiceArticle = readArticle;
    //        voiceSource.clip = ac;
    //        voiceSource.Play();
    //    }
    //}
    public void OnShareBtnClick()
    {
        popSentenceSelectView.Init();
        popSentenceSelectView.gameObject.SetActive(true);

        //shareView.gameObject.SetActive(true);
        //shareView.Init();
    }
    public void OnShareCommandBtnClick()
    {
        Book currentBook = null;
        string channelID = "";
        if (articleView.gameObject.activeSelf)
        {
            currentBook = articleView.contentView.currentBook;
            channelID = articleView.contentView.currentChannelId;
        }
        if (newArticleView.gameObject.activeSelf)
        {
            currentBook = newArticleView.contentView.currentBook;
            channelID = newArticleView.contentView.currentChannelId;
        }
        if (currentBook == null)
        {
            UITool.ShowToastMessage(this, "currentBook == null", 35);
            return;
        }
        string cmd = CommonTool.GetArticleCommandByValue(currentBook.id, currentBook.paragraph,
            currentBook.chapter_len, channelID, currentBook.toc);
        CommonTool.WriteToClipboard(cmd);
        //string temp = CommonTool.GetClipboard();
        //Debug.LogError("+" + temp);
        UITool.ShowToastMessage(this, LocalizationManager.GetTranslation("star_Copy2Clipboard"), 35);
    }
    bool isSet = false;
    public void SetToggleValue(bool isOn)
    {
        //bug:??????????第一次进收藏过的单词，点击星后，没有触发OnToggleValueChanged
        //已解决：注册事件改为Awake就好了，事件还未注册就设置toggle了
        if (starToggle.isOn != isOn)
        {
            isSet = true;
            starToggle.isOn = isOn;
        }
    }
    void OnToggleValueChanged(bool value)
    {
        if (isSet)
        {
            isSet = false;
            return;
        }
        popView.Init(PopViewType.SaveArticle);
        popView.RefreshGroupList();
        popView.gameObject.SetActive(true);
    }
    // Update is called once per frame
    void Update()
    {

    }
}
