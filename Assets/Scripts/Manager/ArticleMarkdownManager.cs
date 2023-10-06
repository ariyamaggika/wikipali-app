using Imdork.SQLite;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using static ArticleController;
using static DictManager;
/// <summary>
/// 文章句子特殊语法
/// 术语&注释&句子
/// {{type|base64}}
/// </summary>
public class ArticleMarkdownManager
{
    private ArticleMarkdownManager() { }
    private static ArticleMarkdownManager manager = null;
    //静态工厂方法 
    public static ArticleMarkdownManager Instance()
    {
        if (manager == null)
        {
            manager = new ArticleMarkdownManager();
        }
        return manager;
    }
    const string MARKDOWN_TERM = "term";        //术语
    const string MARKDOWN_SENT = "sent";        //句子
    const string MARKDOWN_NOTE = "note";        //注释
    //Regex r_term = new Regex(@"\{\{term\|(.*)\}\}");
    Regex r_term = new Regex(@"\{\{term\|(.+?)\}\}");
    Regex r = new Regex(@"\{\{(\w+)\}\}");
    //翻译文章的句子显示术语
    public string SentenceSetMarkDown(string sentence, string channel, string owner)
    {
        string res = PrefilterSentenceTerm(sentence);

        return res;
    }
    Dictionary<string, TermInfoJson> termDic = new Dictionary<string, TermInfoJson>();
    public void ClearTermDic()
    {
        termDic.Clear();

    }
    string PrefilterSentenceTerm(string sentence)
    {
        //sentence = "<span><p>590.<strong>以[缘起]支的差别</strong>者，于此[缘起支]中，为了显示{{term|eyJ3b3JkIjoiYmhhdmFjYWtrYSIsInBhcmVudENoYW5uZWxJZCI6IjdhYzRkMTNiLWE0M2QtNDQwOS05MWI1LTVmMmE4MmI5MTZiMyIsInBhcmVudFN0dWRpb0lkIjoiYmMwZGM2MmEtNDY4Ni00Y2RlLThhMWUtNjAyMGFmYTkxODQ1IiwiaWQiOiJlMGE1MWE1YS1hYTI1LTRjNjctYTE5Mi0xM2UzYTRlMWQ1YjIiLCJtZWFuaW5nIjoiXHU2NzA5XHU4ZjZlIiwiY2hhbm5lbCI6IiIsImlubmVySHRtbCI6Ilx1NjcwOVx1OGY2ZShiaGF2YWNha2thKSJ9}}的不断而说{{term|eyJ3b3JkIjoic29rYSIsInBhcmVudENoYW5uZWxJZCI6IjdhYzRkMTNiLWE0M2QtNDQwOS05MWI1LTVmMmE4MmI5MTZiMyIsInBhcmVudFN0dWRpb0lkIjoiYmMwZGM2MmEtNDY4Ni00Y2RlLThhMWUtNjAyMGFmYTkxODQ1IiwiaWQiOiJiMGM3OTIwYi05NzM0LTQ3ZjctOTY2NS1mZmQ0NjA5OGEwN2YiLCJtZWFuaW5nIjoiXHU2MTAxIiwiY2hhbm5lbCI6IiIsImlubmVySHRtbCI6Ilx1NjEwMShzb2thKSJ9}}等。</p>\r\n</span>\r\n";
        //sentence = MarkdownText.ReplaceHTMLStyle(sentence);
        string res = sentence;
        MatchCollection mcs = r_term.Matches(sentence);
        Match[] mArr = mcs.ToArray();
        int offset = 0;

        for (int i = 0; i < mArr.Length; i++)
        {
            string term = mArr[i].Value.Substring("{{term|".Length);
            term = term.Substring(0, term.Length - 2);
            //Debug.LogError(term);
            TermInfoJson json = GetTermInfo(term);
            //Debug.LogError(mArr[i].Index + "," + mArr[i].Value);
            string fillWord = json.meaning;
            //第一次出现术语，要以这个格式meaing(word,meaning2)显示
            if (!termDic.ContainsKey(json.meaning))
            {
                fillWord = json.meaning + "(" + json.word + (string.IsNullOrEmpty(json.meaning2) ? "" : ("," + json.meaning2)) + ")";
                termDic.Add(fillWord, json);
                termDic.Add(json.meaning, json);
            }

            string front = res.Substring(0, mArr[i].Index - offset);
            string back = res.Substring(mArr[i].Index + mArr[i].Value.Length - offset, res.Length - mArr[i].Index - mArr[i].Value.Length + offset);
            res = front + "<" + fillWord + ">" + back;
            offset += mArr[i].Value.Length - fillWord.Length - 2;//<>占两个符号
        }
        return res;
    }
    public TermInfoJson GetTermByWord(string word)
    {
        if (termDic.ContainsKey(word))
        {
            return termDic[word];
        }
        else
            return null;
    }
    TermInfoJson GetTermInfo(string encodedInfo)
    {
        string json = CommonTool.DecodeBase64(encodedInfo);
        TermInfoJson termInfoJson = JsonUtility.FromJson<TermInfoJson>(json);
        return termInfoJson;
    }

    [Serializable]
    public class TermInfoJson
    {
        public string id;
        public string word;
        public string meaning;
        public string meaning2;
        public string channel;/**该术语在term表中的channel_id */
        public string parentChannelId;/**该术语所在译文的channel_id */
        public string parentStudioId;/**该术语所在译文的studio_id */
        public string summary;
        public bool isCommunity;
    }
    class TermInfo
    {
        public string word_en;
        public string word;
        public string meaning;
        public string otherMeaning;
        public string note;
        public string tag;
        public string channelID;
        public string language;
        public string owner;
    }
    //先只查channel,查不到就只差owner，都查不到就只显示ID
    //TermInfo GetTermInfoByChannelAndOwner(string channel, string owner)
    //{




    //}
    TermInfo SelectTermInfo(DbAccess db, string channel, string owner)
    {
        //先只查channel
        var reader = db.SelectSame("dhamma_terms", channel, "channel_id", 1);

        //调用SQLite工具  解析对应数据
        Dictionary<string, object>[] pairs = SQLiteTools.GetValues(reader);
        if (pairs != null)
        {
            int length = pairs.Length;
            for (int i = 0; i < length; i++)
            {
                TermInfo res = new TermInfo();
                res.word = pairs[i]["word"].ToString();
                res.word_en = pairs[i]["word_en"].ToString();
                res.meaning = pairs[i]["meaning"].ToString();
                res.otherMeaning = pairs[i]["other_meaning"].ToString();
                res.note = pairs[i]["note"].ToString();
                res.channelID = pairs[i]["channel_id"].ToString();
                res.language = pairs[i]["language"].ToString();
                res.owner = pairs[i]["owner"].ToString();
                return res;
            };
        }
        //查不到就只差owner
        //var reader = db.SelectSame("dhamma_terms", channel, "channel_id", 1);

        ////调用SQLite工具  解析对应数据
        //Dictionary<string, object>[] pairs = SQLiteTools.GetValues(reader);
        //if (pairs != null)
        //{
        //    int length = pairs.Length;
        //    for (int i = 0; i < length; i++)
        //    {
        //        TermInfo res = new TermInfo();
        //        res.word = pairs[i]["word"].ToString();
        //        res.word_en = pairs[i]["word_en"].ToString();
        //        res.meaning = pairs[i]["meaning"].ToString();
        //        res.otherMeaning = pairs[i]["other_meaning"].ToString();
        //        res.note = pairs[i]["note"].ToString();
        //        res.channelID = pairs[i]["channel_id"].ToString();
        //        res.language = pairs[i]["language"].ToString();
        //        res.owner = pairs[i]["owner"].ToString();
        //        return res;
        //    };
        //}
        return null;
    }
    TermInfo SelectTermInfo(DbAccess db, string uuid)
    {
        //通过UUID查term
        var reader = db.SelectSame("dhamma_terms", uuid, "uuid", 1);

        //调用SQLite工具  解析对应数据
        Dictionary<string, object>[] pairs = SQLiteTools.GetValues(reader);
        if (pairs != null)
        {
            int length = pairs.Length;
            for (int i = 0; i < length; i++)
            {
                TermInfo res = new TermInfo();
                res.word = pairs[i]["word"].ToString();
                res.word_en = pairs[i]["word_en"].ToString();
                res.meaning = pairs[i]["meaning"].ToString();
                res.otherMeaning = pairs[i]["other_meaning"].ToString();
                res.note = pairs[i]["note"].ToString();
                res.channelID = pairs[i]["channel_id"].ToString();
                res.language = pairs[i]["language"].ToString();
                res.owner = pairs[i]["owner"].ToString();
                return res;
            };
        }
        return null;
    }
}
