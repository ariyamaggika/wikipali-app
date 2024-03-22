using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
//动态列表，全加载，只显示视口范围区域的Text，加速&防字太多炸掉
public class TMPScrollEnableSelf : MonoBehaviour
{
    //public float thisPosY;
    public RectTransform content;
    TextMeshProUGUI thisTMP;
    RectTransform thisRectTrans;
    float startYPos;
    float endYPos;

    public void OnInit(RectTransform _content, TextMeshProUGUI _thisTMP)
    {
        content = _content;
        thisTMP = _thisTMP;
        thisRectTrans = this.GetComponent<RectTransform>();
        startYPos = -(thisRectTrans.localPosition.y + thisRectTrans.sizeDelta.y * 0.5f);
        endYPos = startYPos + thisRectTrans.sizeDelta.y;// * 0.5f;
        Debug.LogError(startYPos);
        Debug.LogError(endYPos);
    }
    // Start is called before the first frame update
    void Start()
    {
        //OnInit(null,null);
    }

    // Update is called once per frame
    void Update()
    {
        //500容错半个屏幕长度
        if (content.localPosition.y >= startYPos-3000 && content.localPosition.y <= endYPos+3000)
            thisTMP.enabled = true;
        else
            thisTMP.enabled = false;
    }
}
