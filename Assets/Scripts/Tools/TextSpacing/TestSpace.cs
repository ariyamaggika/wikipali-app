//using System.Collections;
//using System.Collections.Generic;
//using TMPro;
//using UnityEngine;
//using UnityEngine.EventSystems;
//using UnityEngine.UI;

//public class TestSpace : MonoBehaviour
//{
//    public Text text;
//    // Start is called before the first frame update
//    void Start()
//    {
//        //text.text = "New T各个 \tTextNew Text\r\nNew Text\t\t Text\r\n各 TextNew TextNew Text\r\nNew TextNew Text";
//        //text.text = "|语种|缩写|全称|\r\n|-|-|-|\r\n|巴利|kri|kriyā|\r\n|汉|**动**|动词|\r\n|英|**v.**|verb|";
//        string test = "|语种|缩写|全称|\r\n|-|-|-|\r\n|巴利|**na**|napuṃsaka<br>napuṃsakaliṅga|\r\n|汉|<b>动</b>|动词|\r\n|英|**v.**|verb|";
//        text.text = MarkdownText.PreprocessText(test);

//    }

//    // Update is called once per frame
//    void Update()
//    {
        
//    }


//}
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class TestSpace : MonoBehaviour, IPointerClickHandler
{
    public TextMeshProUGUI text;

    public void OnPointerClick(PointerEventData eventData)
    {
        Vector3 pos = new Vector3(eventData.position.x, eventData.position.y, 0);
        int linkIndex = TMP_TextUtilities.FindIntersectingLink(text, pos, null); //--UI相机
        if (linkIndex > -1)
        {
            TMP_LinkInfo linkInfo = text.textInfo.linkInfo[linkIndex];
            Debug.Log(linkInfo.GetLinkText());
            //Application.OpenURL(linkInfo.GetLinkID());
        }
    }
}
