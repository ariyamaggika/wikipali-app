using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewArticleTitleReturnBtn : MonoBehaviour
{
    public Image img;
    public Button btn;
    public Text title;
    public NewArticleView articleView;
    // Start is called before the first frame update
    void Start()
    {
        btn.onClick.AddListener(OnBtnClick);

    }
    public void SetText(string text)
    {
        title.text = text;
        //Debug.LogError(title.text);

    }
    public void SetBackBtnClick(bool on)
    { 
        img.gameObject.SetActive(on);
        btn.enabled = on;

    }
    public void OnBtnClick()
    {
        SpeechManager.Instance().StopLoadSpeech();
        articleView.ReturnBtnClick();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
