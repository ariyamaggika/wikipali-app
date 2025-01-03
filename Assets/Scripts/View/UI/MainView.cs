﻿//主界面
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainView : MonoBehaviour
{
    public Toggle dicToggle;
    public Toggle articleToggle;
    public Toggle newToggle;
    public Toggle calendarToggle;
    public Toggle userToggle;
    public Toggle settingToggle;
    public DicView dicView;
    public ArticleView articleView;
    public NewArticleView newArticleView;
    public CalendarView calendarView;
    public UserView userView;
    public SettingView settingView;

    public Dropdown settingDropdown;
    // Start is called before the first frame update
    void Start()
    {
        dicToggle.onValueChanged.AddListener(OnDicToggleValueChanged);
        articleToggle.onValueChanged.AddListener(OnArticleToggleValueChanged);
        newToggle.onValueChanged.AddListener(OnNewToggleValueChanged);
        calendarToggle.onValueChanged.AddListener(OnCalendarToggleValueChanged);
        userToggle.onValueChanged.AddListener(OnUserToggleValueChanged);
        settingToggle.onValueChanged.AddListener(OnSettingToggleValueChanged);
        settingDropdown.onValueChanged.AddListener(OnSettingDropdownValueChanged);
    }
    public void SetDicOn()
    {
        dicToggle.isOn = true;
    }
    public void SetArticleOn()
    {
        articleToggle.isOn = true;
    }
    public void SetSettingOn()
    {
        settingToggle.isOn = true;
    }
    void OnDicToggleValueChanged(bool value)
    {
        dicView.gameObject.SetActive(value);

    }
    void OnArticleToggleValueChanged(bool value)
    {
        articleView.gameObject.SetActive(value);
    }
    void OnNewToggleValueChanged(bool value)
    {
        newArticleView.gameObject.SetActive(value);
    }
    void OnCalendarToggleValueChanged(bool value)
    {
        calendarView.gameObject.SetActive(value);
    }
    void OnUserToggleValueChanged(bool value)
    {
        userView.gameObject.SetActive(value);
    }
    void OnSettingToggleValueChanged(bool value)
    {
        settingView.gameObject.SetActive(value);
    }

    private void OnSettingDropdownValueChanged(int index)
    {
        Debug.Log(index);
        switch (index)
        {
            case 0: break;
            case 1: break;
            case 2: break;
            //case 1: break;
            default: break;
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
