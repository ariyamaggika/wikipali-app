using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static SelectCityController;
using static SettingManager;

public class ItemCityView : MonoBehaviour
{
    public Text titleText;
    public Text detailText;
    public Button clickBtn;
    public CalendarView calendarView;
    public SelectCityView selectCityView;
    CityInfo cityInfo = null;
    // Start is called before the first frame update
    void Start()
    {
        clickBtn.onClick.AddListener(OnBtnClick);
    }
    public void Init(CityInfo _cityInfo)
    {
        cityInfo = _cityInfo;
        titleText.text = cityInfo.name;
        if (cityInfo.transName != null && cityInfo.transName.Count > 0)
            titleText.text = cityInfo.transName[(Language)SettingManager.Instance().GetLanguageType()];
        if (string.IsNullOrEmpty(titleText.text))
            titleText.text = cityInfo.name;
        detailText.text = cityInfo.fullName;
    }

    void OnBtnClick()
    {
        calendarView.SetSelectCity(cityInfo);
        selectCityView.SetThisOff();
    }
    //public void OnPointerClick(PointerEventData eventData)
    //{
    //    calendarView.SetSelectCity(cityInfo);
    //    selectCityView.SetThisOff();
    //}
    // Update is called once per frame
    void Update()
    {

    }
}
