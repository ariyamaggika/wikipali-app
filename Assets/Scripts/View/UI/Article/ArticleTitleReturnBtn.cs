using I2.Loc;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArticleTitleReturnBtn : MonoBehaviour
{
    public Image img;
    public Button btn;
    public ScrollRect sr;
    public Text title;
    public ArticleView articleView;
    // Start is called before the first frame update
    void Start()
    {
        Init();

        btn.onClick.AddListener(OnBtnClick);

    }
    public void Init()
    {
        img.gameObject.SetActive(false);
        //title.text = "圣典";
        title.text = LocalizationManager.GetTranslation("article_Article");
        title.gameObject.SetActive(false);
        StartCoroutine(ShowArticleC());
        //sr.horizontalNormalizedPosition = 1;
    }
    public void SetPath(string path)
    {
        img.gameObject.SetActive(true);
        title.text = path;
        //Debug.LogError(title.text);
        title.gameObject.SetActive(false);
        StartCoroutine(ShowArticleC());
    }
    IEnumerator ShowArticleC()
    {
        yield return null;
        title.gameObject.SetActive(true);
        StartCoroutine(ShowArticleC2());
        //sr.horizontalNormalizedPosition = 1;
    }
    IEnumerator ShowArticleC2()
    {
        yield return null;
        StartCoroutine(ShowArticleC3());
       // sr.horizontalNormalizedPosition = 1;
    }
    IEnumerator ShowArticleC3()
    {
        yield return null;
        //StartCoroutine(ShowArticleC2());
        sr.horizontalNormalizedPosition = 1;
    }
    public void OnBtnClick()
    {
        SpeechManager.Instance().StopLoadSpeech();
        articleView.ReturnBtnClick();
    }
    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.W))
        //{
        //    sr.horizontalNormalizedPosition = 1;
        //    Debug.LogError("@!@@");
        //}
        //if (Input.GetKeyDown(KeyCode.Q))
        //{
        //    sr.horizontalNormalizedPosition = 0;
        //    Debug.LogError("?????");
        //}
    }
}
