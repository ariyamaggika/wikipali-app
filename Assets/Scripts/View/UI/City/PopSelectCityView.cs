using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static ArticleManager;
using static SelectCityController;

public class PopSelectCityView : MonoBehaviour
{
    public SelectCityView mainView;
    public Button searchBtn;
    public Toggle domesticToggle;
    public Toggle internationalToggle;
    public Button okBtn;
    public Button cancelBtn;
    public Button backBtn;
    public GameObject level3Group;
    public GameObject letters26Group;

    public RectTransform lv1Content;
    public RectTransform lv2Content;
    public RectTransform lv3Content;
    public Level3ChildToggleView level1Toggle;
    public Level3ChildToggleView level2Toggle;
    public Level3ChildToggleView level3Toggle;
    SelectCityController controller = SelectCityController.Instance();
    //public ToggleGroup lv1TG;
    //public ToggleGroup lv2TG;
    //public ToggleGroup lv3TG;
    public CityInfo currSelectCity;
    public CalendarView calendarView;

    public void SetThisOff()
    {
        SetInner(true);
    }
    //国内toggle
    public void SetInner(bool isInner)
    {
        level3Group.gameObject.SetActive(isInner);
        letters26Group.gameObject.SetActive(!isInner);
    }
    // Start is called before the first frame update
    void Start()
    {
        searchBtn.onClick.AddListener(OnSearchBtnClick);
        okBtn.onClick.AddListener(OnOkBtnClick);
        cancelBtn.onClick.AddListener(OnBackBtnClick);
        backBtn.onClick.AddListener(OnBackBtnClick);
        domesticToggle.onValueChanged.AddListener(OnToggleValueChanged);
        internationalToggle.onValueChanged.AddListener(OnToggleValueChanged);

        //controller.GetAllDomesticFirstCity();
        //controller.GetAllInternationalFirstCity();
        //DestroyAllToggleList();
        SetDomesticLevel1List();
    }
    public void OnInit()
    {
        SetDomesticLevel1List();

    }
    void OnToggleValueChanged(bool value)
    {
        if (domesticToggle.isOn)//国内
        {
            SetDomesticLevel1List();
        }
        else if (internationalToggle.isOn)//国外
        {
            SetInternationalLevel1List();
        }
    }
    List<GameObject> toggleLv1List = new List<GameObject>();
    List<GameObject> toggleLv2List = new List<GameObject>();
    List<GameObject> toggleLv3List = new List<GameObject>();
    //销毁下拉列表GO
    private void DestroyAllToggleList()
    {
        DestroyToggleLv1List();
        DestroyToggleLv2List();
        DestroyToggleLv3List();
    }
    private void DestroyToggleLv1List()
    {
        int length = toggleLv1List.Count;
        if (length == 0)
            return;
        for (int i = 0; i < length; i++)
        {
            Destroy(toggleLv1List[i]);
        }
        toggleLv1List.Clear();
    }
    private void DestroyToggleLv2List()
    {
        int length = toggleLv2List.Count;
        if (length == 0)
            return;
        for (int i = 0; i < length; i++)
        {
            Destroy(toggleLv2List[i]);
        }
        toggleLv2List.Clear();
    }
    private void DestroyToggleLv3List()
    {
        int length = toggleLv3List.Count;
        if (length == 0)
            return;
        for (int i = 0; i < length; i++)
        {
            Destroy(toggleLv3List[i]);
        }
        toggleLv3List.Clear();
    }
    void SelectLv1FirstToggle()
    {
        if (toggleLv1List.Count > 0)
        {
            toggleLv1List[0].GetComponent<Toggle>().isOn = true;
            toggleLv1List[0].GetComponent<Level3ChildToggleView>().OnPointerClick(null);
        }
    }
    void SelectLv2FirstToggle()
    {
        if (toggleLv2List.Count > 0)
        {
            toggleLv2List[0].GetComponent<Toggle>().isOn = true;
            toggleLv2List[0].GetComponent<Level3ChildToggleView>().OnPointerClick(null);
        }
    }
    void SelectLv3FirstToggle()
    {
        if (toggleLv3List.Count > 0)
        {
            toggleLv3List[0].GetComponent<Toggle>().isOn = true;
            toggleLv3List[0].GetComponent<Level3ChildToggleView>().OnPointerClick(null);
        }
    }
    void SetDomesticLevel1List()
    {
        DestroyAllToggleList();
        Dictionary<int, FirstCityInfo> fCitys = controller.GetAllDomesticFirstCityInfos();
        foreach (var city in fCitys)
        {
            GameObject inst = Instantiate(level1Toggle.gameObject, lv1Content, false);
            inst.GetComponent<Level3ChildToggleView>().OnInit(city.Value, true);
            inst.SetActive(true);
            toggleLv1List.Add(inst);
        }
        lv1Content.localPosition = Vector3.zero;
        SelectLv1FirstToggle();
    }
    void SetInternationalLevel1List()
    {
        DestroyAllToggleList();
        Dictionary<int, FirstCityInfo> fCitys = controller.GetAllInternationalFirstCityInfos();
        foreach (var city in fCitys)
        {
            GameObject inst = Instantiate(level1Toggle.gameObject, lv1Content, false);
            inst.GetComponent<Level3ChildToggleView>().OnInit(city.Value, false);
            inst.SetActive(true);
            toggleLv1List.Add(inst);
        }
        lv1Content.localPosition = Vector3.zero;
        SelectLv1FirstToggle();

    }
    public void SetDomesticLevel2List(CityInfo pCity)
    {
        DestroyToggleLv2List();
        DestroyToggleLv3List();

        Dictionary<int, SecondCityInfo> fCitys = controller.GetDomesticSecondCity(pCity.id);
        foreach (var city in fCitys)
        {
            GameObject inst = Instantiate(level2Toggle.gameObject, lv2Content, false);
            inst.GetComponent<Level3ChildToggleView>().OnInit(city.Value, true);
            inst.SetActive(true);
            toggleLv2List.Add(inst);
        }
        lv2Content.localPosition = Vector3.zero;
        SelectLv2FirstToggle();
    }
    public void SetInternationalLevel2List(CityInfo pCity)
    {
        DestroyToggleLv2List();
        DestroyToggleLv3List();

        Dictionary<int, SecondCityInfo> fCitys = controller.GetInternationalSecondCity(pCity.id);
        foreach (var city in fCitys)
        {
            GameObject inst = Instantiate(level2Toggle.gameObject, lv2Content, false);
            inst.GetComponent<Level3ChildToggleView>().OnInit(city.Value, false);
            inst.SetActive(true);
            toggleLv2List.Add(inst);
        }
        lv2Content.localPosition = Vector3.zero;
        SelectLv2FirstToggle();

    }
    public void SetDomesticLevel3List(SecondCityInfo pCity)
    {
        DestroyToggleLv3List();
        Dictionary<int, ThirdCityInfo> fCitys = controller.GetDomesticThirdCity(pCity);
        foreach (var city in fCitys)
        {
            GameObject inst = Instantiate(level3Toggle.gameObject, lv3Content, false);
            inst.GetComponent<Level3ChildToggleView>().OnInit(city.Value, true);
            inst.SetActive(true);
            toggleLv3List.Add(inst);
        }
        lv3Content.localPosition = Vector3.zero;
        SelectLv3FirstToggle();
    }
    public void SetInternationalLevel3List(SecondCityInfo pCity)
    {

        DestroyToggleLv3List();
        Dictionary<int, ThirdCityInfo> fCitys = controller.GetInternationalThirdCity(pCity);
        foreach (var city in fCitys)
        {
            GameObject inst = Instantiate(level3Toggle.gameObject, lv3Content, false);
            inst.GetComponent<Level3ChildToggleView>().OnInit(city.Value, false);
            inst.SetActive(true);
            toggleLv3List.Add(inst);
        }
        lv3Content.localPosition = Vector3.zero;
        SelectLv3FirstToggle();
    }
    void OnSearchBtnClick()
    {

    }
    void OnBackBtnClick()
    {
        mainView.SetThisOff();
    }
    void OnOkBtnClick()
    {
        calendarView.SetIsCurrCity(false);
        mainView.SetThisOff();
    }
    // Update is called once per frame
    void Update()
    {

    }
}
