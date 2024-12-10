using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static SelectCityController;
using static SettingManager;

public class Level3ChildToggleView : MonoBehaviour, IPointerClickHandler
{
    public CityInfo cityInfo;
    public Text name;
    public int level;//1,2,3
    public PopSelectCityView popSelectCityView;
    public bool isDomestic = false;
    public void OnInit(CityInfo _cityInfo, bool _isDomestic)
    {
        cityInfo = _cityInfo;
        isDomestic = _isDomestic;
        if (isDomestic)
            name.text = cityInfo.name;
        else if (level == 1)//只有lv1有翻译
            name.text = cityInfo.transName[(Language)SettingManager.Instance().GetLanguageType()];
        else
            name.text = cityInfo.name;

    }
    //public Toggle thisToggle;
    // Start is called before the first frame update
    void Start()
    {

    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (level == 1)
        {
            //获取全部2级3级城市信息显示
            if (isDomestic)
                popSelectCityView.SetDomesticLevel2List(cityInfo);
            else
                popSelectCityView.SetInternationalLevel2List(cityInfo);
        }
        else if (level == 2)
        {
            //获取全部3级城市信息显示
            if (isDomestic)
                popSelectCityView.SetDomesticLevel3List((SecondCityInfo)cityInfo);
            else
                popSelectCityView.SetInternationalLevel3List((SecondCityInfo)cityInfo);
        }
        else if (level == 3)
        {
            popSelectCityView.currSelectCity = cityInfo;
        }
    }
    // Update is called once per frame
    void Update()
    {

    }
}
