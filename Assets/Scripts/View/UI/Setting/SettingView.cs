﻿using I2.Loc;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static SettingManager;
using static UpdateManager;

public class SettingView : MonoBehaviour
{
    public Button aboutBtn;
    public Button reportBtn;
    public Button updateBtn;
    public Button offlinePackDownloadBtn;
    public Button languageBtn;
    public Text languageText;
    public Button paliVoiceTypeBtn;
    public Text paliVoiceTypeText;
    public Button paliVoiceGenderBtn;
    public Text paliVoiceGenderText;
    public Button paliVoiceSpeedBtn;
    public Text paliVoiceSpeedText;
    public Button articleClassifyBtn;
    public Text versionText;
    public Text offlinePackTimeText;
    public GameObject updateRedPoint;
    public GameObject offlinePackDownloadRedPoint;
    public GameObject reportGO;
    public Button paliRemoveBracketSliderBtn;
    public Slider paliRemoveBracketSliderToggle;
    public Button transContentSliderBtn;
    public Slider transContentSliderToggle;
    public Button openLastSliderBtn;
    public Slider openLastSliderToggle;
    public CommonGroupView commonGroupView;
    // Start is called before the first frame update
    void Start()
    {
        //if (GameManager.Instance().canUpdate)
        //    SetUpdateRedPoint();
        paliVoiceSpeedText.text = SettingManager.Instance().GetPaliVoiceSpeedName();
        paliVoiceTypeText.text = SettingManager.Instance().GetPaliVoiceTypeName();
        languageText.text = SettingManager.Instance().GetLanguageTypeStr();
        paliVoiceGenderText.text = SettingManager.Instance().GetPaliVoiceGenderName();
        paliRemoveBracketSliderToggle.value = SettingManager.Instance().GetPaliRemoveBracket();
        paliRemoveBracketSliderToggle.onValueChanged.AddListener(OnPaliRemoveBracketToggleValueChanged);
        paliRemoveBracketSliderBtn.onClick.AddListener(OnPaliRemoveBracketBtnClick);
        transContentSliderToggle.value = SettingManager.Instance().GetTransContent();
        transContentSliderToggle.onValueChanged.AddListener(OnTransContentToggleValueChanged);
        transContentSliderBtn.onClick.AddListener(OnTransContentBtnClick);
        openLastSliderToggle.value = SettingManager.Instance().GetOpenLast();
        openLastSliderToggle.onValueChanged.AddListener(OnOpenLastToggleValueChanged);
        openLastSliderBtn.onClick.AddListener(OnOpenLastBtnClick);
        aboutBtn.onClick.AddListener(OnAboutBtnClick);
        offlinePackDownloadBtn.onClick.AddListener(OnOfflinePackDownloadBtnClick);
        updateBtn.onClick.AddListener(OnUpdateBtnClick);
        reportBtn.onClick.AddListener(OnReportBtnClick);
        paliVoiceSpeedBtn.onClick.AddListener(OnPaliVoiceSpeedBtnClick);
        paliVoiceTypeBtn.onClick.AddListener(OnPaliVoiceTypeBtnClick);
        languageBtn.onClick.AddListener(OnLanguageBtnClick);
        paliVoiceGenderBtn.onClick.AddListener(OnPaliVoiceGenderBtnClick);
        articleClassifyBtn.onClick.AddListener(OnArticleClassifyBtnClick);
        versionText.text = "v" + Application.version;
        offlinePackTimeText.text = SettingManager.Instance().GetDBPackTime();
    }
    void OnPaliRemoveBracketToggleValueChanged(float value)
    {
        //Debug.LogError(value);
        SettingManager.Instance().SetPaliRemoveBracket((int)value);
    }

    void OnPaliRemoveBracketBtnClick()
    {
        if (paliRemoveBracketSliderToggle.value > 0.5f)
        {
            paliRemoveBracketSliderToggle.value = 0;
        }
        else
        {
            paliRemoveBracketSliderToggle.value = 1;
        }
    }
    void OnTransContentToggleValueChanged(float value)
    {
        //Debug.LogError(value);
        SettingManager.Instance().SetTransContent((int)value);
    }

    void OnTransContentBtnClick()
    {
        if (transContentSliderToggle.value > 0.5f)
        {
            transContentSliderToggle.value = 0;
        }
        else
        {
            transContentSliderToggle.value = 1;
        }
    }
    void OnOpenLastToggleValueChanged(float value)
    {
        //Debug.LogError(value);
        SettingManager.Instance().SetOpenLast((int)value);
    }
    void OnOpenLastBtnClick()
    {
        if (openLastSliderToggle.value > 0.5f)
        {
            openLastSliderToggle.value = 0;
        }
        else
        {
            openLastSliderToggle.value = 1;
        }
    }
    void OnReportBtnClick()
    {
        reportGO.SetActive(true);
    }
    void OnArticleClassifyBtnClick()
    {
        //commonGroupView.InitSettingOptions(new List<string> { "默认", "CSCD4" }, 0, (a) => {SettingManager.Instance().SetTransContent  return null; });
        //commonGroupView.gameObject.SetActive(true);
    }
    void OnPaliVoiceGenderBtnClick()
    {
        int sID = (int)SettingManager.Instance().GetPaliVoiceGender();
        //commonGroupView.InitSettingOptions("巴利朗读声音", new List<string> { "男声", "女声" }, sID, (id) =>
        commonGroupView.InitSettingOptions(LocalizationManager.GetTranslation("setting_PaliVoiceType"), new List<string> { 
            LocalizationManager.GetTranslation("setting_PaliVoiceType_Man0"),
           LocalizationManager.GetTranslation("setting_PaliVoiceType_Woman0") }, sID, (id) =>
        {
            SettingManager.Instance().SetPaliVoiceGender((PaliSpeakVoiceGender)id);
            paliVoiceGenderText.text = SettingManager.Instance().GetPaliVoiceGenderName();
            return null;
        });
        commonGroupView.gameObject.SetActive(true);
    }
    void OnPaliVoiceTypeBtnClick()
    {
        int sID = (int)SettingManager.Instance().GetPaliVoiceType();
        //commonGroupView.InitSettingOptions("巴利朗读风格", new List<string> { "印度风格", "缅甸风格" }, sID, (id) =>
        commonGroupView.InitSettingOptions(LocalizationManager.GetTranslation("setting_PaliVoiceStyle"), new List<string> {
            LocalizationManager.GetTranslation("setting_PaliVoiceStyle_India")
            ,LocalizationManager.GetTranslation("setting_PaliVoiceStyle_Burma")}, sID, (id) =>
        {
            SettingManager.Instance().SetPaliVoiceType((PaliSpeakVoiceType)id);
            paliVoiceTypeText.text = SettingManager.Instance().GetPaliVoiceTypeName();
            return null;
        });
        commonGroupView.gameObject.SetActive(true);
    }
    void OnLanguageBtnClick()
    {
        int sID = (int)SettingManager.Instance().GetLanguageType();
        //commonGroupView.InitSettingOptions("语言", new List<string> { "中文简体", "中文繁体", "English", "日本語", "Burmese", "Sinhala", "Thai" }, sID, (id) =>
        commonGroupView.InitSettingOptions(LocalizationManager.GetTranslation("setting_Language")
            , new List<string> { "中文简体", "中文繁体", "English", "日本語", "Burmese", "Sinhala", "Thai" }, sID, (id) =>
        {
            SettingManager.Instance().SetLanguageType((LanguageType)id);
            ChangeAllOptionsTextLanguage();
            languageText.text = SettingManager.Instance().GetLanguageTypeStr();
            return null;
        });
        commonGroupView.gameObject.SetActive(true);
    }
    //切换语言时切换所有子选项语言
    void ChangeAllOptionsTextLanguage()
    {
        paliVoiceTypeText.text = SettingManager.Instance().GetPaliVoiceTypeName();
        paliVoiceGenderText.text = SettingManager.Instance().GetPaliVoiceGenderName();

    }
    void OnPaliVoiceSpeedBtnClick()
    {
        int sID = SettingManager.PaliSpeakVoiceSpeedList.IndexOf((int)SettingManager.Instance().GetPaliVoiceSpeed());
        //commonGroupView.InitSettingOptions("巴利朗读语速", new List<string> { "0", "-10%", "-20%", "-30%", "-40%" }, sID, (id) =>
        commonGroupView.InitSettingOptions(LocalizationManager.GetTranslation("setting_ReadSpeed"),
            new List<string> { "0", "-10%", "-20%", "-30%", "-40%" }, sID, (id) =>
        {
            SettingManager.Instance().SetPaliVoiceSpeed((PaliSpeakVoiceSpeed)SettingManager.PaliSpeakVoiceSpeedList[(int)id]);
            paliVoiceSpeedText.text = SettingManager.Instance().GetPaliVoiceSpeedName();
            return null;
        });
        commonGroupView.gameObject.SetActive(true);
    }
    void OnAboutBtnClick()
    {
        commonGroupView.InitAboutView();
        commonGroupView.gameObject.SetActive(true);
    }
    public void OnOfflinePackDownloadBtnClick()
    {
        if (UpdateManager.Instance().currentOInfo == null || UpdateManager.Instance().currentOInfo.offlinePackJson == null)
        {
            UpdateManager.Instance().GetOtherInfo();
        }
        if (UpdateManager.Instance().currentOInfo == null || UpdateManager.Instance().currentOInfo.offlinePackJson == null)
            return;
        if (UpdateManager.Instance().currentOInfo.offlinePackJson.create_at == SettingManager.Instance().GetDBPackTime())
        {
            //UITool.ShowToastMessage(GameManager.Instance(), "当前已是最新版本", 35);
            UITool.ShowToastMessage(GameManager.Instance(), LocalizationManager.GetTranslation("setting_LastestVersion"), 35);
            return;
        }
        //打开详情页
        GameManager.Instance().ShowSettingViewOfflineDBPackPage(UpdateManager.Instance().currentOInfo);
    }
    void OnUpdateBtnClick()
    {
        UpdateManager.Instance().CheckUpdateOpenPage(this);

        //StartCoroutine(UpdateManager.Instance().Test());
    }
    public void SetUpdatePage(UpdateInfo currentUInfo)
    {
        commonGroupView.InitUpdateView(currentUInfo);
        commonGroupView.gameObject.SetActive(true);
    }
    public void SetUpdateRedPoint()
    {
        updateRedPoint.SetActive(true);
    }
    public void UpdateSettingViewOfflineDBTimeText()
    {
        offlinePackTimeText.text = "        " + SettingManager.Instance().GetDBPackTime();
    }
    public void SetOfflinePackPage(OtherInfo currentOInfo)
    {
        commonGroupView.InitOfflinePackView(currentOInfo);
        commonGroupView.gameObject.SetActive(true);
    }
    public void HideCommonGroupView()
    {
        commonGroupView.gameObject.SetActive(false);
    }
    public void SetOfflinePackRedPoint(bool active)
    {
        offlinePackDownloadRedPoint.SetActive(active);
    }
    // Update is called once per frame
    void Update()
    {

    }
}
