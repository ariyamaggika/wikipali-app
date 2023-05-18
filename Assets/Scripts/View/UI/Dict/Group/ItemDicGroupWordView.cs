﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemDicGroupWordView : MonoBehaviour
{
    public PopViewType currViewType;
    public Text titleText;
    public Button delBtn;
    public Button textBtn;
    string articleTitle;
    string word;
    int groupID;
    CommonGroupView commonView;
    public DicGroupView dicGroupView;
    public PopView popView;
    public void Init(string _word, int _groupID, CommonGroupView _commonView, PopViewType currViewType)
    {
        if (currViewType == PopViewType.SaveDic)
        {
            word = _word;
        }
        else if (currViewType == PopViewType.SaveArticle)
        {
            articleTitle = _word;
        }
        commonView = _commonView;
        groupID = _groupID;
        titleText.text = word;
    }
    //public void Init(string _articleTitle, int _groupID, CommonGroupView _commonView)
    //{
    //    articleTitle = _articleTitle;
    //    commonView = _commonView;
    //    groupID = _groupID;
    //    titleText.text = word;
    //}

    // Start is called before the first frame update
    void Start()
    {
        delBtn.onClick.AddListener(OnDelBtnClick);
        textBtn.onClick.AddListener(OnTextBtnClick);
    }
    public void OnDelBtnClick()
    {
        if (currViewType == PopViewType.SaveDic)
        {
            DictManager.Instance().DelWord(groupID, word);

        }
        else if (currViewType == PopViewType.SaveArticle)
        {
            ArticleManager.Instance().DelArticle(groupID, articleTitle);

        }
        commonView.RefreshGroupList();
    }
    //跳转到查词
    public void OnTextBtnClick()
    {
        GameManager.Instance().mainView.SetDicOn();
        GameManager.Instance().mainView.dicView.OnItemDicClick(word);
        popView.OnCloseBackBtnClick();
        dicGroupView.OnCloseBtnClick();
        commonView.OnCloseBtnClick();
    }
    // Update is called once per frame
    void Update()
    {

    }
}
