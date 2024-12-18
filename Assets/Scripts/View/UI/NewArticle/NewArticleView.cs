using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static ArticleController;
using static C2SArticleGetNewDBInfo;

public class NewArticleView : MonoBehaviour
{
    public LoadingTexView loadingTexView;    //文章标题树
    public GameObject listViewGO;
    public NewArticleNodeItemView nodeItem;
    public NewArticleScrollRectRef thisSrr;

    public NewArticleTitleReturnBtn returnBtn;

    //todo 通用阅读部分，通用prefab和代码
    //文章内容 pali原文和翻译
    public ArticleContentScrollView contentView;
    //离线提示页面
    public ArticleOfflineGuideView offlineGuideView;
    //加载转菊花页面
    //public ArticleLoadingView articleLoadingView;

    //下拉刷新
    // Start is called before the first frame update
    void Start()
    {
        //thisSrr.callback1 = (state) => { s.SetActive(state); };
        //thisSrr.callback1 = (a) => { Debug.LogError(1); Debug.LogError(a); return null; };
        thisSrr.endDragUpdateCallback = () => { SendServerRequest(); Debug.LogError("刷新列表"); return null; };
        SendServerRequest();

    }
    public void SetOfflineGuideOn(Func<object> refreshCallBack)
    {
        offlineGuideView.Init(refreshCallBack);
        offlineGuideView.gameObject.SetActive(true);
    }
    //加载旋转动画
    void StartLoadingAnim()
    {
        loadingTexView.transform.parent.gameObject.SetActive(true);
        loadingTexView.StartLoadingTex();
    }
    void StopLoadingAnim()
    {
        loadingTexView.transform.parent.gameObject.SetActive(false);
        loadingTexView.StopLoadingTex();
    }
    public void SendServerRequest()
    {
        StartLoadingAnim();
        C2SArticleGetNewDBInfo.GetNewArticleList(CallBack);
    }
    List<GameObject> nodeList = new List<GameObject>();
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
    object CallBack(NewArticleListJson article)
    {
        StopLoadingAnim();

        //thisArticle = article;
        RefreshList(article);
        return null;
    }
    void RefreshList(NewArticleListJson article)
    {
        DestroyNodeList();
        if (article == null || article.data == null || article.data.rows == null || article.data.rows.Count == 0)
            return;
        List<NewArticleData> info = article.data.rows;
        int length = info.Count;
        float height = nodeItem.GetComponent<RectTransform>().sizeDelta.y;
        for (int i = 0; i < length; i++)
        {
            GameObject inst = Instantiate(nodeItem.gameObject, nodeItem.transform.parent);
            inst.transform.position = nodeItem.transform.position;
            inst.GetComponent<RectTransform>().position -= Vector3.up * height * i;

            inst.GetComponent<NewArticleNodeItemView>().Init(info[i],this);
            inst.SetActive(true);
            nodeList.Add(inst);
        }

    }
    public void ReturnBtnClick()
    {
        returnBtn.SetBackBtnClick(false);
        listViewGO.SetActive(true);
        contentView.gameObject.SetActive(false);
        returnBtn.SetText("最新");
    }
    public void ArticleNodeBtnClick()
    {
        //contentView.gameObject.SetActive(false);
        returnBtn.SetBackBtnClick(true);
        contentView.gameObject.SetActive(true);
        returnBtn.SetText("返回");
    }
    // Update is called once per frame
    void Update()
    {

    }
}
