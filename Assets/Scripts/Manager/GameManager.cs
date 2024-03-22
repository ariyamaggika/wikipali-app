using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Android;
using static UpdateManager;

public class GameManager : MonoBehaviour
{
    private static GameManager manager = null;
    //静态工厂方法 
    public static GameManager Instance()
    {
        if (manager == null)
        {
            manager = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<GameManager>();
        }
        return manager;
    }
    public InitView initView;
    public SettingView settingView;
    public StarGroupDictView dicStarGroup;
    public StarGroupArticleView articleStarGroup;
    public MainView mainView;
    public ArticleView articleView;
    public PreView preView;
    public LoadingTexView titleLoadingTexView;

    public string appVersion;//= Application.version;
    //public bool canUpdate = false;
    void Awake()
    {
        appVersion = Application.version;
        SettingManager.Instance().InitGame();
        DictManager.Instance().dicStarGroup = dicStarGroup;
        ArticleManager.Instance().articleStarGroup = articleStarGroup;
        ArticleManager.Instance().articleView = articleView;
        LoadingViewManager.Instance().titleLoadingView = titleLoadingTexView;
    }
    bool isStartUnZipProgress = false;
    bool isDownLoadProgress = false;
    Func<object> unZipCallback;
    public void StartUnZipProgress(Func<object> callback, string title = "正在解压")
    {
        initView.gameObject.SetActive(true);
        initView.Init(title);
        isStartUnZipProgress = true;
        unZipCallback = callback;
    }
    public void StartUnZipWithOutProgress(Func<object> callback)
    {
        //initView.gameObject.SetActive(true);
        //initView.Init(title);
        isStartUnZipProgress = true;
        unZipCallback = callback;
    }
    public void SetInitViewProgress(float progress)
    {
        initView.SetProgess(progress);
    }
    public void EndUnZipProgress()
    {
        initView.gameObject.SetActive(false);
        isStartUnZipProgress = false;
    }
    public object EndUnZipDB()
    {
        SettingManager.Instance().UnZipFin();
        return null;
    }
    // Start is called before the first frame update
    void Start()
    {
        CheckUpdate();
        if (CreatQR.LoadQR())
            CreatQR.CreatQr();
        SpeechGeneration.Instance().LoadTxt();

        //打开上次浏览记录
        SettingManager.Instance().OpenLast();
    }
    public DownloadManager public_dm = null;
    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.W))
        //{
        //    UITool.ShowToastUp(this, "请允许wikipali app使用定位权限\r\n为了提供您所在位置的明相日中等服务。\r\n我们需要获取您设备的所在定位信息。\r\n不授权不影响您使用APP"
        //        , 10, 80);
        //}
        if (isStartUnzip)
        {
            isStartUnzip = false;
            SettingManager.Instance().UnzipDB();
        }
        if (isStartUnZipProgress)
        {
            int progressFin = ZipManager.Instance().lzmafileProgress[0];

            ulong sizeOfEntry = ZipManager.Instance().sizeOfEntry;
            ulong bwrite = lzma.getBytesWritten();
            float progress = ((float)bwrite / (float)sizeOfEntry);
            //progress值在解压.lzma文件是不对的，所以暂时限制在99
            if (progress > 0.99f)
                progress = 0.99f;
            //Debug.LogError("s:" + sizeOfEntry);
            //Debug.LogError("b:" + bwrite);
            SetInitViewProgress(progress);
            if (100 == progressFin)
            {
                EndUnZipProgress();
                unZipCallback();
            }
        }
        if (isDownLoadProgress)
        {
            SetInitViewProgress(public_dm.progress * 0.01f);
        }
    }
    //检测更新
    void CheckUpdate()
    {
        //检测红点和下载AzureKey
        UpdateManager.Instance().CheckUpdateRedPoint();
    }

    public void ShowSettingViewOfflineDBPackPage(OtherInfo currentOInfo)
    {
        settingView.SetOfflinePackPage(currentOInfo);
    }
    public void HideSettingViewOfflineDBPackPage()
    {
        settingView.HideCommonGroupView();
    }
    public void UpdateSettingViewOfflineDBTimeText()
    {
        settingView.UpdateSettingViewOfflineDBTimeText();
    }
    public void ShowSettingViewUpdatePage(UpdateInfo currentUInfo)
    {
        settingView.SetUpdatePage(currentUInfo);
    }
    public void ShowSettingViewUpdateRedPoint()
    {
        //canUpdate = true;
        settingView.SetUpdateRedPoint();
    }
    public void ShowSettingViewOfflinePackRedPoint()
    {
        settingView.SetOfflinePackRedPoint(true);
    }
    public void HideSettingViewOfflinePackRedPoint()
    {
        settingView.SetOfflinePackRedPoint(false);
    }
    public void ShowSettingViewDownloadOfflinePackPage()
    {
        mainView.SetSettingOn();
        settingView.OnOfflinePackDownloadBtnClick();
    }
    public void StartDownLoadProgress()
    {
        initView.gameObject.SetActive(true);
        initView.Init("下载进度");
        isDownLoadProgress = true;
    }
    public void DownLoadProgressOver()
    {
        isDownLoadProgress = false;
        //直接跳转到安装APK，就不关闭InitView防止误触
        //initView.gameObject.SetActive(false);
    }

    //延迟一帧
    public void ShowArticle(int bookID, int bookParagraph, int bookChapterLen, string channelId)
    {
        //此处判断网络，在线阅读&离线包

        StartCoroutine(ShowArticleC(bookID, bookParagraph, bookChapterLen, channelId));
    }
    IEnumerator ShowArticleC(int bookID, int bookParagraph, int bookChapterLen, string channelId)
    {
        yield return null;
        mainView.SetArticleOn();
        mainView.articleView.contentView.ShowPaliContentFromStar(bookID, bookParagraph, bookChapterLen, channelId);
    }
    public void ShowDicWord(string word)
    {
        mainView.SetDicOn();
        mainView.dicView.OnItemDicClick(word);
    }
    //检测隐私政策版本
    public void CheckPrivacyVersion(int newVersion, string url)
    {
        preView.CheckPrivacyVersion(newVersion, url);
    }
    public void StartUserPermission()
    {
        StartCoroutine(StartUserPermissionLoction());

    }
    IEnumerator StartUserPermissionLoction()
    {
        yield return new WaitForSeconds(0.5f);
        CalendarManager.Instance().StartLocation();

        StartCoroutine(StartUserPermissionStorage());

    }
    IEnumerator StartUserPermissionStorage()
    {
        yield return null;
        CalendarManager.Instance().StartLocation();

        //if (Permission.HasUserAuthorizedPermission(Permission.ExternalStorageRead))
        //{
        //}
        //else
        //{
        //    Permission.RequestUserPermission(Permission.ExternalStorageRead);
        //}
        if (Permission.HasUserAuthorizedPermission(Permission.ExternalStorageWrite))
        {
            SettingManager.Instance().UnzipDB();
        }
        else
        {
            PermissionCallbacks callBack = new PermissionCallbacks();
            //Action<string> a = (string s) => { SettingManager.Instance().UnzipDB(); };
            //callBack.PermissionGranted += a;
            callBack.PermissionGranted += PermissionCallbacks_PermissionGranted;
            //Permission.RequestUserPermission(Permission.ExternalStorageWrite, callBack);
            Permission.RequestUserPermissions(new string[2] { Permission.ExternalStorageRead, Permission.ExternalStorageWrite }, callBack);
        }

    }
    bool isStartUnzip = false;
    internal void PermissionCallbacks_PermissionGranted(string permissionName)
    {
        isStartUnzip = true;
        //Debug.LogError("IEnumerator StartUnZip");
        //SettingManager.Instance().UnzipDB();
    }

    //IEnumerator StartUnZip()
    //{
    //    yield return new WaitForSeconds(0.15f);
    //    Debug.LogError("IEnumerator StartUnZip");

    //    SettingManager.Instance().UnzipDB();

    //}

    //public object StartUnZipDBSentence()
    //{
    //    StartCoroutine(ZipManager.Instance().UnZipDBSentence());
    //    return null;
    //}

    #region 网络部分
  
    #endregion

}
