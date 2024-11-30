using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectCityView : MonoBehaviour
{
    public SelectAllCityView selectAllCityView;
    public PopSelectCityView popSelectCityView;

    public void SetSelectAllCity(bool isOn)
    {
        selectAllCityView.gameObject.SetActive(isOn);
    }
    public void SetThisOff()
    {
        SetSelectAllCity(false);
        popSelectCityView.SetThisOff();
        this.gameObject.SetActive(false);
    }
    public void SetThisOn()
    {
        SetSelectAllCity(false);
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
