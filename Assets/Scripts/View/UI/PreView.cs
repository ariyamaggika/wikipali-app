using Hypertext;
using Org.BouncyCastle.Bcpg;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.UI;
//��˽����ȷ�Ͻ���
public class PreView : MonoBehaviour
{
    //1.����Ƿ�����
    //2.��ȡ�汾��
    //3.�ȶԱ��ش洢�汾��
    //4.�������ͬ�͵�����˽����
    //5.���ر�����˽���߰汾��

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
        //����Ȩ��
        //Permission.RequestUserPermission("android.hardware.location");
        //Permission.RequestUserPermission("android.hardware.location.gps");
        UITool.ShowToastUp(GameManager.Instance(), "������wikipali appʹ�ö�λȨ��\r\nΪ���ṩ������λ�õ��������еȷ���\r\n������Ҫ��ȡ���豸�����ڶ�λ��Ϣ��\r\n����Ȩ��Ӱ����ʹ��APP"
    , 4, 80);
        //�Ӻ�һ֡��ȡ��λȨ�ޣ�ȷ�������ȳ���
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
    const string RegexHashTag = @"��(\w+)��";

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
        if (content == "���û��� ˽���ߡ�" || content == "���û���˽���ߡ�")
        {
            Application.OpenURL(pUrl);
        }
    }
    string PrivacyTextBig(string url)
    {
        string res = "��ӭʹ�á�wikipali app�����Ƿǳ������� �ĸ�����Ϣ����˽����������ʹ�á�wikipali app������֮ǰ������ϸ�Ķ����û���˽���ߡ����ǽ�������ͬ�������ʹ�����ĸ�����Ϣ���Ա�Ϊ���ṩ����";
        return res;

    }
    string PrivacyTextBigUpdate(string url)
    {
        string res = "��ӭʹ�á�wikipali app�����ǵ��û���˽�����и��¡�����ʹ�á�wikipali app������֮ǰ������ϸ�Ķ����û���˽���ߡ����ǽ�������ͬ�������ʹ�����ĸ�����Ϣ���Ա�Ϊ���ṩ����";
        return res;

    }
    string PrivacyTextSmall()
    {
        string res = "����ͬ������ߣ�������ͬ�⡱����ʼʹ�����ǵĲ�Ʒ�ͷ������Ǿ�ȫ���������ĸ�����Ϣ��ȫ��";
        return res;
    }
}
