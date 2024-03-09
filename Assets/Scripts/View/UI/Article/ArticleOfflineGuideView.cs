using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArticleOfflineGuideView : MonoBehaviour
{
    public Button downLoadOfflinePackBtn;
    public Button refreshButton;
    void Awake()
    {
        downLoadOfflinePackBtn.onClick.AddListener(OnDownloadBtnClick);
        refreshButton.onClick.AddListener(OnRefreshBtnClick);
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void OnDownloadBtnClick()
    {
        GameManager.Instance().ShowSettingViewDownloadOfflinePackPage();
    }
    public void OnRefreshBtnClick()
    {

    }
}
