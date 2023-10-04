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
    const string RegexHashTag = @"\<(\w+)\>";

    // Start is called before the first frame update
    void Start()
    {
        //articleText.OnClick(RegexHashTag, new Color(0, 0.2235f, 0.898f, 1), hashtag => TermText(hashtag));
        articleText.OnClick(RegexHashTag, Color.red, hashtag => TermText(hashtag));

    }
    void TermText(string regex)
    {
        string word = regex.Substring(1);
        word = word.Substring(0, word.Length - 1);
        TermInfoJson term = ArticleMarkdownManager.Instance().GetTermByWord(word);
        if (term != null)
        {
            string title = term.meaning;
            string content = "<color=#5895FF>" + term.word + "</color>\r\n" + term.meaning2 + "\r\n" + term.summary;
            popTView.Init(title, content);
        }

    }
    // Update is called once per frame
    void Update()
    {

    }
}
