using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static DictManager;
using static SelectCityController;

public class SelectAllCityView : MonoBehaviour
{
    public Button closeBtn;
    public InputField inputField;
    public ItemCityView itemBtn;
    public SelectCityView selectCityView;
    public RectTransform scrollContent;
    public Button delBtn;
    public Text searchTitleText;
    public Text historyTitleText;
    bool isDelBtnOn = false;

    // Start is called before the first frame update
    void Start()
    {
        closeBtn.onClick.AddListener(OnCloseBtnClick);
        inputField.onValueChanged.AddListener(OnSearchValueChanged);
        delBtn.onClick.AddListener(OnDelBtnClick);
    }
    public void Init(bool isHistory)
    {
        OnDelBtnClick();
        //查看搜索历史
        if (isHistory)
        {
            historyTitleText.gameObject.SetActive(true);
            searchTitleText.gameObject.SetActive(false);
            inputField.gameObject.SetActive(false);
        }
        else
        {
            historyTitleText.gameObject.SetActive(false);
            searchTitleText.gameObject.SetActive(true);
            inputField.gameObject.SetActive(true);
        }
    }
    public void OnDelBtnClick()
    {
        inputField.text = "";
        DestroyItemDicList();
    }
    void OnCloseBtnClick()
    {
        selectCityView.SetSelectAllCity(false, false);
    }
    public void OnSearchValueChanged(string value)
    {
        DestroyItemDicList();
        SearchWord(inputField.text);

    }
    List<GameObject> itemList = new List<GameObject>();
    //设置删除输入文字按钮开关
    void SetDelBtn(bool sw)
    {
        if (sw != isDelBtnOn)
        {
            isDelBtnOn = sw;
            delBtn.gameObject.SetActive(sw);
        }
    }
    void SearchWord(string inputStr)
    {
        if (string.IsNullOrEmpty(inputStr))
        {
            SetDelBtn(false);
            return;
        }
        bool isChinese = CommonTool.CheckStringIsChinese(inputStr);
        bool isMyanmar = CommonTool.CheckStringIsMyanmar(inputStr);
        List<CityInfo> cityInfoArr = null;
        //中文
        if (isChinese)
        {
            cityInfoArr = SelectCityController.Instance().FuzzySearchDomesticCityInfoByName(inputStr);
            cityInfoArr.AddRange(SelectCityController.Instance().FuzzySearchInternationalCityInfoByTransName(inputStr));
            //matchedWordArr = dicManager.MatchWordChinese(inputStr);
        }
        else//国外
        {
            //???todo只有三级城市信息 是否需要查找一二级城市???
            cityInfoArr = SelectCityController.Instance().FuzzySearchInternationalCityInfoByName(inputStr);
            //matchedWordArr = dicManager.MatchWord(inputStr);
        }
        SetDelBtn(true);
        int length = cityInfoArr.Count;// matchedWordArr.Length > DictManager.LIMIT_COUNT ? LIMIT_COUNT : matchedWordArr.Length;
        for (int i = 0; i < length; i++)
        {
            GameObject inst = Instantiate(itemBtn.gameObject, scrollContent, false);
            inst.transform.position = itemBtn.transform.position;
            inst.GetComponent<ItemCityView>().Init(cityInfoArr[i]);
            inst.SetActive(true);
            itemList.Add(inst);
        }

    }
    private void DestroyItemDicList()
    {
        int length = itemList.Count;
        if (length == 0)
            return;
        for (int i = 0; i < length; i++)
        {
            Destroy(itemList[i]);
        }
        itemList.Clear();
    }
}
