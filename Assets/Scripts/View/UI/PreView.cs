using Hypertext;
using Org.BouncyCastle.Bcpg;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.UI;
//隐私政策确认界面
public class PreView : MonoBehaviour
{
    //1.检查是否联网
    //2.获取版本号
    //3.比对本地存储版本号
    //4.如果不相同就弹出隐私政策
    //5.本地保存隐私政策版本号

    public RegexHypertext textBig;
    public Text textSmall;
    public Button okBtn;
    public Button quitBtn;
    string pUrl;
    int pVersion;
    // Start is called before the first frame update
    void Start()
    {
        okBtn.onClick.AddListener(OKBtn);
        quitBtn.onClick.AddListener(QuitBtn);

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void OKBtn()
    {
        SaveNewPrivacyVersion();
        this.gameObject.SetActive(false);
        //请求权限
        //Permission.RequestUserPermission("android.hardware.location");
        //Permission.RequestUserPermission("android.hardware.location.gps");
        UITool.ShowToastUp(GameManager.Instance(), "请允许wikipali app使用定位权限\r\n为了提供您所在位置的明相日中等服务。\r\n我们需要获取您设备的所在定位信息。\r\n不授权不影响您使用APP"
    , 4, 80);
        //延后一帧获取定位权限，确保弹窗先出来
        GameManager.Instance().StartLocation();
        //StartCoroutine(StartLocation());

    }
    IEnumerator StartLocation()
    {
        yield return null;
        CalendarManager.Instance().StartLocation();
    }
    public void QuitBtn()
    {
        Application.Quit();
    }
    void SaveNewPrivacyVersion()
    {
        SettingManager.Instance().SetPrivacyVersion(pVersion);
    }
    const string RegexHashTag = @"《(\w+)》";

    public bool CheckPrivacyVersion(int newVersion, string url)
    {
        int nowVersion = SettingManager.Instance().GetPrivacyVersion();
        if (nowVersion != newVersion)
        {
            pUrl = url;
            pVersion = newVersion;
            if (nowVersion == 0)
                textBig.text = PrivacyTextBig(url);
            else
                textBig.text = PrivacyTextBigUpdate(url);

            textBig.OnClick(RegexHashTag, new Color(0, 0.2235f, 0.898f, 1), hashtag => HttpLink(hashtag));
            textSmall.text = PrivacyTextSmall();
            this.gameObject.SetActive(true);
        }
        return true;
    }
    void HttpLink(string content)
    {
        if (content == "《用户隐 私政策》" || content == "《用户隐私政策》")
        {
            Application.OpenURL(pUrl);
        }
    }
    string PrivacyTextBig(string url)
    {
        string res = "欢迎使用“wikipali app”我们非常重视您 的个人信息和隐私保护。在您使用“wikipali app”服务之前，请仔细阅读《用户隐私政策》我们将按照您同意的条款使用您的个人信息，以便为您提供服务。";
        return res;

    }
    string PrivacyTextBigUpdate(string url)
    {
        string res = "欢迎使用“wikipali app”我们的用户隐私政策有更新。在您使用“wikipali app”服务之前，请仔细阅读《用户隐私政策》我们将按照您同意的条款使用您的个人信息，以便为您提供服务。";
        return res;

    }
    string PrivacyTextSmall()
    {
        string res = "如您同意此政策，请点击“同意”并开始使用我们的产品和服务，我们尽全力保护您的个人信息安全。";
        return res;
    }
}
