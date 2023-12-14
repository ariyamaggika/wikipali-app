using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static C2SArticleGetNewDBInfo;

public class NewArticleNodeItemView : MonoBehaviour
{
    public Button inArticleBtn;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI subText;
    public TextMeshProUGUI contentText;

    public TextMeshProUGUI progressText;
    public Image progressImg;

    public TextMeshProUGUI userText;
    public TextMeshProUGUI timeText;
    // Start is called before the first frame update
    void Start()
    {
        inArticleBtn.onClick.AddListener(OnInArticleBtnClick);

    }
    public void OnInArticleBtnClick()
    {

    }
    public void Init(NewArticleData data)
    {
        titleText.text = data.title;
        subText.text = data.toc;
        contentText.text = data.summary;
        data.progress = Mathf.Clamp01(data.progress);
        progressText.text = (int)(data.progress * 100) + "%";
        progressImg.fillAmount = data.progress;
        userText.text = data.channel.name;
        timeText.text = CommonTool.FormatDate(DateTime.Parse(data.updated_at));
    }
    // Update is called once per frame
    void Update()
    {

    }
}
