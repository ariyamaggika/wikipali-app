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
    //todo ��ӳ�ʱˢ�¹���
    //30s��ʱ���������߽��棬��Ҫ���ˢ�£����»�ȡ����
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
