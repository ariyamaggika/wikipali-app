using Hypertext;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using static ArticleMarkdownManager;

public class ArticleRegexText : MonoBehaviour
{
    public PopTermView popTView;

    public RegexHypertext articleText;
    const string RegexTerm = @"\<(.+?)\>";
    const string RegexNote = @"\[ע��(\w+)\]";

    // Start is called before the first frame update
    void Start()
    {
        //articleText.OnClick(RegexHashTag, new Color(0, 0.2235f, 0.898f, 1), hashtag => TermText(hashtag));
        articleText.OnClick(RegexHypertext.TERM_TAG, hashtag => TermText(hashtag));
        articleText.OnClick(RegexNote, new Color(0, 0.2235f, 0.898f, 1), hashtag => NoteText(hashtag));

    }
    void TermText(string regex)
    {
        string word = regex.Substring(1);
        word = word.Substring(0, word.Length - 1);
        TermInfoJson term = ArticleMarkdownManager.Instance().GetTermByWord(word);
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
        NoteJson note = ArticleMarkdownManager.Instance().GetNoteByID(word);
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
