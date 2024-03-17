using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArticleLoadingView : MonoBehaviour
{
    public ArticleView articleView;
    public LoadingTexView loadingTexView;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    Func<object> refreshFunc;
    float timer = 0;
    bool startTimer = false;
    // Update is called once per frame
    void Update()
    {
        if (startTimer)
        {
            timer += Time.deltaTime;
            if (timer > 30)
            {
                TimeOut();
            }
        }
    }
    void TimeOut()
    {
        startTimer = false;
        timer = 0;
        articleView.SetOfflineGuideOn(refreshFunc);
    }
    //todo 添加超时刷新功能
    //30s后超时，进入离线界面，需要点击刷新，重新获取文章
    public void StartLoading(Func<object> _refreshFunc)
    {
        refreshFunc = _refreshFunc;
        timer = 0;
        startTimer = true;
        this.gameObject.SetActive(true);
        loadingTexView.StartLoadingTex();
    }
    public void StopLoading()
    {
        startTimer = false;
        timer = 0;
        this.gameObject.SetActive(false);
        loadingTexView.StopLoadingTex();
    }
}
