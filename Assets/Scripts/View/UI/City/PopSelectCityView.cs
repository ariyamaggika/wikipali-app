using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopSelectCityView : MonoBehaviour
{
    public SelectCityView mainView;
    public Button searchBtn;
    public Toggle domesticToggle;
    public Toggle internationalToggle;
    public Button okBtn;
    public Button cancelBtn;
    public Button backBtn;
    public GameObject domesticGroup;
    public GameObject internationalGroup;

    public void SetThisOff()
    {
        SetInner(true);
    }
    //¹úÄÚtoggle
    public void SetInner(bool isInner)
    {
        domesticGroup.gameObject.SetActive(isInner);
        internationalGroup.gameObject.SetActive(!isInner);
    }
    // Start is called before the first frame update
    void Start()
    {
        searchBtn.onClick.AddListener(OnSearchBtnClick);
        okBtn.onClick.AddListener(OnOkBtnClick);
        cancelBtn.onClick.AddListener(OnBackBtnClick);
        backBtn.onClick.AddListener(OnBackBtnClick);
    }
    void OnSearchBtnClick()
    { 
    
    }
    void OnBackBtnClick()
    {

    }
    void OnOkBtnClick()
    {

    }
    // Update is called once per frame
    void Update()
    {

    }
}
