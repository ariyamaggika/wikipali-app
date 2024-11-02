using I2.Loc;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using static ArticleController;
using static iTween;

public class StarGroupDictView : MonoBehaviour
{
    public Toggle starToggle;
    public Button shareBtn;
    public Button shareCommandBtn;
    public Button voiceBtn;
    public AudioSource voiceSource;
    public PopView popView;
    public ShareView shareView;
    public Button foldBtn;
    public Button expandBtn;
    public RectTransform foldPos;
    public RectTransform expandPos;
    // Start is called before the first frame update
    void Awake()
    {
        starToggle.onValueChanged.AddListener(OnToggleValueChanged);
        shareBtn.onClick.AddListener(OnShareBtnClick);
        shareCommandBtn.onClick.AddListener(OnShareCommandBtnClick);
        voiceBtn.onClick.AddListener(OnVoiceBtnClick);
        foldBtn.onClick.AddListener(OnFoldBtnClick);
        expandBtn.onClick.AddListener(OnExpandBtnClick);
    }
    public static string currVoiceWord;
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
        string readWord = SpeechGeneration.Instance().ReplaceWord(DictManager.Instance().currWord);
        if (voiceSource.clip != null && currVoiceWord == readWord)
        {
            voiceSource.Play();
            return;
        }
        AudioClip ac = SpeechGeneration.Instance().SpeekPali(readWord, -10);
        if (ac != null)
        {
            currVoiceWord = readWord;
            voiceSource.clip = ac;
            voiceSource.Play();
        }
    }
    //public void OnVoiceBtnClick()
    //{
    //    string readWord = SpeechGeneration.Instance().ReplaceWord(DictManager.Instance().currWord);
    //    if (voiceSource.clip != null && currVoiceWord == readWord)
    //    {
    //        voiceSource.Play();
    //        return;
    //    }
    //    AudioClip ac = SpeechGeneration.Instance().Speek(readWord);
    //    if (ac != null)
    //    {
    //        currVoiceWord = readWord;
    //        voiceSource.clip = ac;
    //        voiceSource.Play();
    //    }
    //}
    public void OnShareBtnClick()
    {
        shareView.gameObject.SetActive(true);
        shareView.Init();
    }
    public void OnShareCommandBtnClick()
    {

        string cmd = CommonTool.GetDicCommandByValue(DictManager.Instance().currWord);
        //测试代码
        //Debug.LogError(CommonTool.GetValueByCommand(cmd));
        //Debug.LogError("gā");
        //if ("gā" != CommonTool.GetValueByCommand(cmd))
        //    Debug.LogError("!=");
        //if ("gā" == CommonTool.GetValueByCommand(cmd))
        //    Debug.LogError("==");
        //List<string> resText = new List<string>();
        //resText.Add(CommonTool.GetValueByCommand(cmd));
        //resText.Add("gā");
        //File.WriteAllLines("Assets/Editor/testSaveDic.txt", resText.ToArray());
        //GameManager.Instance().ShowDicWord(CommonTool.GetValueByCommand(cmd));

        CommonTool.WriteToClipboard(cmd);
        //防止重复打开自己分享的内容
        GameManager.Instance().lastCopyText = cmd;
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
        popView.Init(PopViewType.SaveDic);
        popView.RefreshGroupList();
        popView.gameObject.SetActive(true);
    }
    public void OnFoldBtnClick()
    {
        foldBtn.gameObject.SetActive(false);
        expandBtn.gameObject.SetActive(true);
        //iTween.MoveTo
        Hashtable args = new Hashtable();
        args["position"] = foldPos.transform.position;
        args["time"] = 1;
        args.Add("easetype", EaseType.easeOutQuart);
        iTween.MoveTo(this.gameObject, args);
    }
    public void OnExpandBtnClick()
    {
        expandBtn.gameObject.SetActive(false);
        foldBtn.gameObject.SetActive(true);
        Hashtable args = new Hashtable();
        args["position"] = expandPos.transform.position;
        args["time"] = 1;
        args.Add("easetype", EaseType.easeOutQuart);
        iTween.MoveTo(this.gameObject, args);
    }
    // Update is called once per frame
    void Update()
    {

    }
}
