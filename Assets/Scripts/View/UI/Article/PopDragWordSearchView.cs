using System.Collections;
using System.Collections.Generic;
using TMPro.Examples;
//using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;
using static DictManager;

public class PopDragWordSearchView : MonoBehaviour
{
    public PopTermView popTView;
    public Button searchWordBtn;
    public Button closeBackGroupBtn;
    public string word;
    public float btnSizeX;
    public float btnSizeY;
    TMP_DragWordSearch tmpTool;
    // Start is called before the first frame update
    void Start()
    {
        searchWordBtn.onClick.AddListener(OnSearchWordBtnClick);
        closeBackGroupBtn.onClick.AddListener(OnCloseBackBtnClick);

    }
    public void Init()
    {
        btnSizeX = searchWordBtn.GetComponent<RectTransform>().sizeDelta.x;
        btnSizeY = searchWordBtn.GetComponent<RectTransform>().sizeDelta.y;
    }
    public void OnSearchWordBtnClick()
    {
        tmpTool.ClearPrevSearch();
        this.gameObject.SetActive(false);
        //≤È’“–°–¥
        string wordStr = word.ToLower();
        bool isHaveWord = DictManager.Instance().IsHaveWord(wordStr);
        if (!isHaveWord)
        {
            GameManager.Instance().ShowDicWord(wordStr);
        }
        else
        {
            MatchedWordDetail wordDetail = DictManager.Instance().MatchWordDetailFastest(wordStr);
            string title = wordStr;
            string content = "<size=65><b>" + wordDetail.dicName+ "</b></size>" + "\r\n"+ wordDetail.meaning;// note.note;
            popTView.Init(title, content, true, wordStr);
        }


    }
    public void DragWord(string _word, TMP_DragWordSearch _tmpTool, Vector3 pos)
    {
        word = _word;
        tmpTool = _tmpTool;
        this.gameObject.SetActive(true);
        float sizeY = this.GetComponent<RectTransform>().sizeDelta.y;
        float sizeBtnY = searchWordBtn.GetComponent<RectTransform>().sizeDelta.y;
        float sizeBtnX = searchWordBtn.GetComponent<RectTransform>().sizeDelta.x;
        float fontsize = 30;
        searchWordBtn.GetComponent<RectTransform>().localPosition = pos;
    }
    public void OnCloseBackBtnClick()
    {
        tmpTool.ClearPrevSearch();
        this.gameObject.SetActive(false);
    }
    // Update is called once per frame
    void Update()
    {

    }
}
