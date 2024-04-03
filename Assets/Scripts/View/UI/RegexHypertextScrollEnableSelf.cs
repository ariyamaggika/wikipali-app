using Hypertext;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
//��̬�б�ȫ���أ�ֻ��ʾ�ӿڷ�Χ�����Text������&����̫��ը��
public class RegexHypertextScrollEnableSelf : MonoBehaviour
{
    //public float thisPosY;
    public RectTransform content;
    RegexHypertext thisText;
    RectTransform thisRectTrans;
    float startYPos;
    float endYPos;

    public void OnInit(RectTransform _content, RegexHypertext _thisText)
    {
        content = _content;
        thisText = _thisText;
        thisRectTrans = this.GetComponent<RectTransform>();
        startYPos = -(thisRectTrans.localPosition.y + thisRectTrans.sizeDelta.y * 0.5f);
        endYPos = startYPos + thisRectTrans.sizeDelta.y;// * 0.5f;
        //Debug.LogError(startYPos);
        //Debug.LogError(endYPos);
    }
    // Start is called before the first frame update
    void Start()
    {
        //OnInit(null,null);
    }
    int tick = 0;
    // Update is called once per frame
    void Update()
    {
        //ÿ9֡���һ��
        if (tick % 9 == 0)
        {  //500�ݴ�����Ļ����
            if (content.localPosition.y >= startYPos - 3000 && content.localPosition.y <= endYPos + 3000)
                thisText.enabled = true;
            else
                thisText.enabled = false;
            tick = 0;
        }
            ++tick;
    }
}
