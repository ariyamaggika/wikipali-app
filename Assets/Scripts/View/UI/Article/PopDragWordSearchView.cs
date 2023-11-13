using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopDragWordSearchView : MonoBehaviour
{
    public Button searchWordBtn;
    public Button closeBackGroupBtn;
    public string word;
    // Start is called before the first frame update
    void Start()
    {
        searchWordBtn.onClick.AddListener(OnSearchWordBtnClick);
        closeBackGroupBtn.onClick.AddListener(OnCloseBackBtnClick);

    }
    public void OnSearchWordBtnClick()
    {
        this.gameObject.SetActive(false);
        GameManager.Instance().ShowDicWord(word);
    }
    public void DragWord(string _word, Vector3 pos)
    {
        word = _word;
        this.gameObject.SetActive(true);
        float sizeY = this.GetComponent<RectTransform>().sizeDelta.y;
        float sizeBtnY = searchWordBtn.GetComponent<RectTransform>().sizeDelta.y;
        float sizeBtnX = searchWordBtn.GetComponent<RectTransform>().sizeDelta.x;
        float fontsize = 30;
        searchWordBtn.GetComponent<RectTransform>().localPosition = Vector3.zero;
        searchWordBtn.GetComponent<RectTransform>().localPosition = new Vector3(pos.x+ sizeBtnX*0.5f, pos.y - sizeY * 1.5f- sizeBtnY*0.5f- fontsize, pos.z);
    }
    public void OnCloseBackBtnClick()
    {
        this.gameObject.SetActive(false);
    }
    // Update is called once per frame
    void Update()
    {

    }
}
