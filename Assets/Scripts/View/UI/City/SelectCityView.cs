using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectCityView : MonoBehaviour
{
    public SelectAllCityView selectAllCityView;
    public PopSelectCityView popSelectCityView;

    public void SetSelectAllCity(bool isOn,bool isHistory)
    {
        if (isOn)
        {
            selectAllCityView.Init(isHistory);
        }
        selectAllCityView.gameObject.SetActive(isOn);
    }
    public void SetThisOff()
    {
        SetSelectAllCity(false,false);
        popSelectCityView.SetThisOff();
        this.gameObject.SetActive(false);
    }
    public void SetThisOn()
    {
        SetSelectAllCity(false, false);
        this.gameObject.SetActive(true);
        popSelectCityView.OnInit();
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
