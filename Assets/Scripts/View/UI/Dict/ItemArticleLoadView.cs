using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static ArticleController;
using static ArticleManager;
using static DictManager;

public class ItemArticleLoadView : MonoBehaviour
{
    public Button refreshBtn;
    public LoadingTexView loadTex;
    public Image refreshTex;
    Func<string, object> GetServerSentencesData;
    string inputStr;
    bool isRefresh = false;
    public void OnInit(string _inputStr, Func<string, object> _GetServerSentencesData)
    {
        inputStr = _inputStr;
        GetServerSentencesData = _GetServerSentencesData;
        isRefresh = false;
    }
    public void SetLoad()
    {
        refreshTex.gameObject.SetActive(false);
        startTimer = true;
        loadTex.StartLoadingTex();
        isRefresh = false;
    }
    public void SetRefresh()
    {
        refreshTex.gameObject.SetActive(true);
        loadTex.StopLoadingTex();
        isRefresh = true;
    }
    public void SetOff()
    {
        refreshTex.gameObject.SetActive(false);
        loadTex.StopLoadingTex();
        isRefresh = false;
        startTimer = false;
        timer = 0;
    }
    // Start is called before the first frame update
    void Start()
    {
        refreshBtn.onClick.AddListener(OnBtnClick);

    }
    public void OnBtnClick()
    {
        if (isRefresh)
        {
            isRefresh = false;
            SetLoad();
            GetServerSentencesData(inputStr);
        }
    }
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
        SetRefresh();
    }
}
