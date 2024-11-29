using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static SelectCityController;

public class Level3ChildToggleView : MonoBehaviour, IPointerClickHandler
{
    public CityInfo cityInfo;
    public Text name;
    public int level;//1,2,3
    public PopSelectCityView popSelectCityView;
    public void OnInit(CityInfo _cityInfo)
    {
        cityInfo = _cityInfo;
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
            popSelectCityView.SetDomesticLevel2List(cityInfo);

        }
        else if (level == 2)
        {
            //获取全部3级城市信息显示
            popSelectCityView.SetDomesticLevel3List((SecondCityInfo)cityInfo);

        }
        else if (level == 3)
        {

        }
    }
    // Update is called once per frame
    void Update()
    {

    }
}
