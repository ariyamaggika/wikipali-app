using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static DictManager;
/// <summary>
/// 词典面板
/// </summary>
public class DicView : MonoBehaviour
{
    public Button searchBtn;
    public Button delBtn;
    //用户输入的查词
    public InputField userInput;
    public Button itemDicBtn;
    public DetailDicItemView detailDicItem;
    public OtherWordItemView otherWordItemView;
    public RectTransform summaryScrollContent;
    public RectTransform detailScrollContent;
    //单词列表面板
    public RectTransform SummaryScrollView;
    //单词详情面板
    public RectTransform DetailScrollView;
    //todo 单例模式
    public DictManager dicManager = DictManager.Instance();
    public bool isDelBtnOn = false;
    //是否是补全单词，补全不是用户输入
    public bool isComplement = false;
    public void SetSummaryText(string text)
    {
        SetSummaryOn();
        userInput.text = text;
    }
    public void OnDelBtnClick()
    {
        SetSummaryOn();
        userInput.text = "";
    }
    public void OnSearchBtnClick()
    {
        //Debug.LogError("你单击了Button");
        DestroyItemDicList();
        //SqliteDataReader reader = dbManager.db.SelectOrderASC("bh-paper", "word");
        SearchWord(userInput.text);
    }
    public void OnSearchValueChanged(string value)
    {
        if (isComplement)
            return;
        //Debug.LogError(value);
        DestroyItemDicList();
        SearchWord(userInput.text);
    }
    List<GameObject> itemDicList = new List<GameObject>();
    void SearchWord(string inputStr)
    {
        if (string.IsNullOrEmpty(inputStr))
        {
            SetDelBtn(false);
            return;
        }
        bool isChinese = CommonTool.CheckStringIsChinese(inputStr);
        bool isMyanmar = CommonTool.CheckStringIsMyanmar(inputStr);
        MatchedWord[] matchedWordArr = null;
        //反向查询，中文
        if (isChinese)
        {
            matchedWordArr = dicManager.MatchWordChinese(inputStr);
        }
        else if (isMyanmar)
        {
            matchedWordArr = dicManager.MatchWordMyanmar(inputStr);
        }
        else//正向查询
        {
            matchedWordArr = dicManager.MatchWord(inputStr);
        }
        SetDelBtn(true);
        //限制了//由于混入多个词典与英文和pali问查找结果，个数没做限制，在此处做限制
        int length = matchedWordArr.Length;// matchedWordArr.Length > DictManager.LIMIT_COUNT ? LIMIT_COUNT : matchedWordArr.Length;
        float height = itemDicBtn.GetComponent<RectTransform>().sizeDelta.y;
        //去格位除尾查
        //if (matchedWordArr.Length == 0)
        //{
        //    GameObject inst = Instantiate(itemDicBtn.gameObject, summaryScrollContent, false);
        //    inst.transform.position = itemDicBtn.transform.position;
        //    inst.GetComponent<ItemDicView>().SetCaseWord(inputStr);
        //    inst.SetActive(true);
        //    itemDicList.Add(inst);
        //    summaryScrollContent.sizeDelta = new Vector2(summaryScrollContent.sizeDelta.x, height * 1);
        //}
        //else
        //{
        for (int i = 0; i < length; i++)
        {
            GameObject inst = Instantiate(itemDicBtn.gameObject, summaryScrollContent, false);
            inst.transform.position = itemDicBtn.transform.position;
            //inst.GetComponent<RectTransform>().position -= Vector3.up * height * i;

            //matchedWordArr[i].meaning.Substring(0, matchedWordArr[i].meaning.IndexOf(System.Environment.NewLine));
            //string first = new StringReader(str).ReadLine();
            inst.GetComponent<ItemDicView>().SetMeaning(matchedWordArr[i]);
            inst.SetActive(true);
            itemDicList.Add(inst);
        }
        if (!isMyanmar && !isChinese)
        {  //去格位除尾查
            GameObject instC = Instantiate(itemDicBtn.gameObject, summaryScrollContent, false);
            instC.transform.position = itemDicBtn.transform.position;
            instC.GetComponent<ItemDicView>().SetCaseWord(inputStr);
            instC.SetActive(true);
            itemDicList.Add(instC);
            //summaryScrollContent.sizeDelta = new Vector2(summaryScrollContent.sizeDelta.x, height * 1);
            summaryScrollContent.sizeDelta = new Vector2(summaryScrollContent.sizeDelta.x, height * (length + 1));
        }
        // }

    }
    //销毁下拉列表GO
    private void DestroyItemDicList()
    {
        int length = itemDicList.Count;
        if (length == 0)
            return;
        for (int i = 0; i < length; i++)
        {
            Destroy(itemDicList[i]);
        }
        itemDicList.Clear();
    }
    //设置删除输入文字按钮开关
    void SetDelBtn(bool sw)
    {
        if (sw != isDelBtnOn)
        {
            isDelBtnOn = sw;
            delBtn.gameObject.SetActive(sw);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        AddInputNameClickEvent();
        delBtn.onClick.AddListener(OnDelBtnClick);
        //userInput.OnPointerClick.AddListener(OnSearchInputClick);
        userInput.onValueChanged.AddListener(OnSearchValueChanged);
    }
    private void AddInputNameClickEvent() //可以在Awake中调用
    {
        var eventTrigger = userInput.GetComponent<EventTrigger>();
        UnityAction<BaseEventData> selectEvent = OnSearchInputFieldClicked;
        EventTrigger.Entry onClick = new EventTrigger.Entry()
        {
            eventID = EventTriggerType.PointerClick
        };

        onClick.callback.AddListener(selectEvent);
        eventTrigger.triggers.Add(onClick);
    }

    private void OnSearchInputFieldClicked(BaseEventData data)
    {
        SetSummaryOn();
        //Debug.LogError("ddd");
    }
    void SetSummaryOn()
    {
        DetailScrollView.gameObject.SetActive(false);
        SummaryScrollView.gameObject.SetActive(true);
    }
    void SetSummaryOff()
    {
        DetailScrollView.gameObject.SetActive(true);
        SummaryScrollView.gameObject.SetActive(false);
    }
    /// <summary>
    /// 点击某个单词查询
    /// </summary>
    /// <param name="word"></param>
    public void OnItemDicClick(MatchedWord word)
    {
        SetSummaryOff();
        //TODO 根据查到的word的id和dicID查询，而不是直接用word全查？？？？但是太麻烦，，，要改 有多个相同词条存在
        DisplayWordDetail(word.word);
    }
    public void OnItemDicClick(string word)
    {
        SetSummaryOff();
        //TODO 根据查到的word的id和dicID查询，而不是直接用word全查？？？？但是太麻烦，，，要改 有多个相同词条存在
        DisplayWordDetail(word);
    }
    List<GameObject> detailDicItemList = new List<GameObject>();
    void DisplayWordDetail(string word)
    {
        //保存上次预览记录
        SettingManager.Instance().SaveOpenLastWord(word);
        DestroyDetailDicItemList();
        if (string.IsNullOrEmpty(word))
        {
            SetDelBtn(false);
            return;
        }
        //补全查词
        isComplement = true;
        userInput.text = word;
        isComplement = false;

        bool isMyanmar = CommonTool.CheckStringIsMyanmar(word[0].ToString());

        MatchedWordDetail[] matchedWordArr = isMyanmar ? dicManager.MatchWordDetailMyanmar(word) : dicManager.MatchWordDetail(word);
        //去格位除尾查
        Dictionary<string, List<string>> caseWordList = DicCase.CaseEndingUnion2(new Dictionary<string, List<string>>(), word);
        //if(caseWordList.Count == 0)
        otherWordItemView.Init(caseWordList);
        dicManager.currWord = word;
        dicManager.SetWordStar(word);
        int length = matchedWordArr.Length;
        //float height = 0;
        for (int i = 0; i < length + 1; i++)
        {
            GameObject inst = Instantiate(detailDicItem.gameObject, detailScrollContent, false);
            inst.transform.position = detailDicItem.transform.position;
            //inst.GetComponent<RectTransform>().position -= Vector3.up * height;
            DetailDicItemView ddiv = inst.GetComponent<DetailDicItemView>();
            //社区词典
            if (i == 0)
            {
                ddiv.InitCommunityDic(word);
                NetworkMangaer.Instance().GetCommunityDic(word, (str) =>
                {
                    //在线就等社区词典后enabled
                    isLateSetHeight = true;
                    ddiv.SetCommunityDic(str);
                    //等下一帧UI刷新后获取位置
                    StartCoroutine(SetHeight());
                    return null;
                });
            }
            else
            {
                ddiv.Init(matchedWordArr[i - 1]);
            }
            //防止字数炸裂，初始化只显示前3个词典，后面交给enabledCtrl
            //if (i > 3)
            //{
            //    ddiv.detailTxt.enabled = false;
            //}
            //float textHeight = ddiv.GetHeight();
            //inst.GetComponent<RectTransform>().sizeDelta += new Vector2(0, textHeight);
            //height += textHeight;
            inst.SetActive(true);
            detailDicItemList.Add(inst);
        }
        //去格位除尾查，最后显示
        otherWordItemView.SetLayerEnd();
        //离线就直接开enabled
        if (!NetworkMangaer.Instance().CheckIsHaveNetwork())
        {
            isLateSetHeight = true;
        }
        //等下一帧UI刷新后获取位置
        StartCoroutine(SetHeight());

    }
    bool isLateSetHeight = false;
    IEnumerator SetHeight()
    {
        yield return null;
        int length = detailDicItemList.Count;
        float height = 0;
        for (int i = 0; i < length; i++)
        {
            //detailDicItemList[i].transform.position = detailDicItem.transform.position;
            //detailDicItemList[i].GetComponent<RectTransform>().position -= Vector3.up * height;
            DetailDicItemView ddiv = detailDicItemList[i].GetComponent<DetailDicItemView>();
            float textHeight = ddiv.GetHeight();
            detailDicItemList[i].GetComponent<RectTransform>().sizeDelta += new Vector2(0, textHeight);
            ddiv.itemHeight = detailDicItemList[i].GetComponent<RectTransform>().sizeDelta.y;

            //?为啥会缩100？
            //height += ddiv.GetHeight() + 200;
        }
        detailScrollContent.sizeDelta = new Vector2(detailScrollContent.sizeDelta.x, height);
        Debug.LogError("-------------------------------------------");
        //todo://////////////////////
        //下一帧计算位置
        if (isLateSetHeight)
        {
            isLateSetHeight = false;
            StartCoroutine(SetTMPEnabledCtrl());
        }

    }
    IEnumerator SetTMPEnabledCtrl()
    {
        yield return null;
        int length = detailDicItemList.Count;
        for (int i = 0; i < length; i++)
        {
            RegexHypertextScrollEnableSelf textEnabledCtrl = null;
            bool getTEC = detailDicItemList[i].TryGetComponent<RegexHypertextScrollEnableSelf>(out textEnabledCtrl);
            if (!getTEC)
                textEnabledCtrl = detailDicItemList[i].gameObject.AddComponent<RegexHypertextScrollEnableSelf>();
            DetailDicItemView ddiv = detailDicItemList[i].GetComponent<DetailDicItemView>();
            textEnabledCtrl.OnInit(detailScrollContent, ddiv.detailTxt);
        }
    }
    //销毁词典列表GO
    private void DestroyDetailDicItemList()
    {
        int length = detailDicItemList.Count;
        if (length == 0)
            return;
        for (int i = 0; i < length; i++)
        {
            Destroy(detailDicItemList[i]);
        }
        detailDicItemList.Clear();
    }
    // Update is called once per frame
    void Update()
    {

    }
}
