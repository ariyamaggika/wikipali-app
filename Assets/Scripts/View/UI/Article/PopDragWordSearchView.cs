using System.Collections;
using System.Collections.Generic;
using TMPro.Examples;
using UnityEngine;
using UnityEngine.UI;

public class PopDragWordSearchView : MonoBehaviour
{
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
        this.gameObject.SetActive(false);
        //≤È’“–°–¥
        GameManager.Instance().ShowDicWord(word.ToLower());
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
        //searchWordBtn.GetComponent<RectTransform>().position = Vector3.zero;
        searchWordBtn.GetComponent<RectTransform>().localPosition = pos;// +Vector3.up* sizeY*0.5f;
                                                                                 // Debug.LogError(searchWordBtn.GetComponent<RectTransform>().rect);
        Debug.LogError(searchWordBtn.GetComponent<RectTransform>().localPosition);
        Debug.LogError(searchWordBtn.GetComponent<RectTransform>().position);
        Debug.LogError(searchWordBtn.transform.position);
        Debug.LogError(searchWordBtn.transform.localPosition);
        //searchWordBtn.GetComponent<RectTransform>().localPosition = new Vector3(pos.x + sizeBtnX * 0.5f, pos.y - sizeY * 1.5f - sizeBtnY * 0.5f - fontsize, pos.z);
        //searchWordBtn.GetComponent<RectTransform>().localPosition = new Vector3(pos.x + sizeBtnX * 0.5f, pos.y, pos.z);
        //searchWordBtn.GetComponent<RectTransform>().localPosition = new Vector3(pos.x + sizeBtnX * 0.5f, pos.y, pos.z);
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
