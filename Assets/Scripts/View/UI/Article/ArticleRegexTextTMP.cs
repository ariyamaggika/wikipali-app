using Hypertext;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static ArticleMarkdownTMPManager;

public class ArticleRegexTextTMP : MonoBehaviour, IPointerClickHandler
{

    public PopTermView popTView;

    //public RegexHypertext articleText;
    const string RegexTerm = @"\<(.+?)\>";
    const string RegexNote = @"\[×¢ÊÍ(\w+)\]";

    public TextMeshProUGUI text;
    public Camera cam;
    public void OnPointerClick(PointerEventData eventData)
    {
        Vector3 pos = new Vector3(eventData.position.x, eventData.position.y, 0);
        int linkIndex = TMP_TextUtilities.FindIntersectingLink(text, pos, cam); //--UIÏà»ú
        if (linkIndex > -1)
        {
            TMP_LinkInfo linkInfo = text.textInfo.linkInfo[linkIndex];
            string linkText = linkInfo.GetLinkText();
            if(!linkText.Contains("[×¢ÊÍ"))
                TermText(linkText);
            else
                NoteText(linkText);
            //foreach (Match match in Regex.Matches(linkText, RegexTerm))
            //{
            //    TermText(match.Value);
            //}
            //Debug.Log(linkInfo.GetLinkText());
            //Application.OpenURL(linkInfo.GetLinkID());
        }
    }





    // Start is called before the first frame update
    void Start()
    {
        //articleText.OnClick(RegexHashTag, new Color(0, 0.2235f, 0.898f, 1), hashtag => TermText(hashtag));
        //articleText.OnClick(RegexHypertext.TERM_TAG, hashtag => TermText(hashtag));
        //articleText.OnClick(RegexNote, new Color(0, 0.2235f, 0.898f, 1), hashtag => NoteText(hashtag));

    }
    void TermText(string regex)
    {
        //string word = regex.Substring(1);
        //word = word.Substring(0, word.Length - 1);
        string word = regex;
        TermInfoJson term = ArticleMarkdownTMPManager.Instance().GetTermByWord(word);
        if (term != null)
        {
            string title = term.meaning;
            string content = CommonTool.COLOR_BLUE_FLAG + term.word + "</color>\r\n" + term.summary;
            popTView.Init(title, content,true, term.word);
        }

    }
    void NoteText(string regex)
    {
        string word = regex.Substring(3);
        word = word.Substring(0, word.Length - 1);
        NoteJson note = ArticleMarkdownTMPManager.Instance().GetNoteByID(word);
        if (note != null)
        {
            string title = "  ";
            string content = note.note;
            popTView.Init(title, content);
        }
    }
    // Update is called once per frame
    void Update()
    {

    }
}
