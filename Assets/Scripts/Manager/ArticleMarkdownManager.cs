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
/// ���¾��������﷨
/// ����&ע��&����
/// {{type|base64}}
/// </summary>
public class ArticleMarkdownManager
{
    private ArticleMarkdownManager() { }
    private static ArticleMarkdownManager manager = null;
    //��̬�������� 
    public static ArticleMarkdownManager Instance()
    {
        if (manager == null)
        {
            manager = new ArticleMarkdownManager();
        }
        return manager;
    }
    const string MARKDOWN_TERM = "term";        //����
    const string MARKDOWN_SENT = "sent";        //����
    const string MARKDOWN_NOTE = "note";        //ע��
    //Regex r_term = new Regex(@"\{\{term\|(.*)\}\}");
    Regex r_term = new Regex(@"\{\{term\|(.+?)\}\}");
    Regex r_note = new Regex(@"\{\{note\|(.+?)\}\}");
    Regex r = new Regex(@"\{\{(\w+)\}\}");
    //�������µľ�����ʾ����
    public string SentenceSetMarkDown(string sentence, string channel, string owner)
    {
        string res = PrefilterSentenceTerm(sentence);
        res = PrefilterSentenceNote(res);

        return res;
    }
    public void ClearMarkdownInfo()
    {
        termDic.Clear();
        noteDic.Clear();
    }
    #region term����
    Dictionary<string, TermInfoJson> termDic = new Dictionary<string, TermInfoJson>();

    string PrefilterSentenceTerm(string sentence)
    {
        //sentence = "<span><p>590.<strong>��[Ե��]֧�Ĳ��</strong>�ߣ��ڴ�[Ե��֧]�У�Ϊ����ʾ{{term|eyJ3b3JkIjoiYmhhdmFjYWtrYSIsInBhcmVudENoYW5uZWxJZCI6IjdhYzRkMTNiLWE0M2QtNDQwOS05MWI1LTVmMmE4MmI5MTZiMyIsInBhcmVudFN0dWRpb0lkIjoiYmMwZGM2MmEtNDY4Ni00Y2RlLThhMWUtNjAyMGFmYTkxODQ1IiwiaWQiOiJlMGE1MWE1YS1hYTI1LTRjNjctYTE5Mi0xM2UzYTRlMWQ1YjIiLCJtZWFuaW5nIjoiXHU2NzA5XHU4ZjZlIiwiY2hhbm5lbCI6IiIsImlubmVySHRtbCI6Ilx1NjcwOVx1OGY2ZShiaGF2YWNha2thKSJ9}}�Ĳ��϶�˵{{term|eyJ3b3JkIjoic29rYSIsInBhcmVudENoYW5uZWxJZCI6IjdhYzRkMTNiLWE0M2QtNDQwOS05MWI1LTVmMmE4MmI5MTZiMyIsInBhcmVudFN0dWRpb0lkIjoiYmMwZGM2MmEtNDY4Ni00Y2RlLThhMWUtNjAyMGFmYTkxODQ1IiwiaWQiOiJiMGM3OTIwYi05NzM0LTQ3ZjctOTY2NS1mZmQ0NjA5OGEwN2YiLCJtZWFuaW5nIjoiXHU2MTAxIiwiY2hhbm5lbCI6IiIsImlubmVySHRtbCI6Ilx1NjEwMShzb2thKSJ9}}�ȡ�</p>\r\n</span>\r\n";
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
            TermInfoJson json = GetMarkdownInfo<TermInfoJson>(term);
            //Debug.LogError(mArr[i].Index + "," + mArr[i].Value);
            string fillWord = json.meaning;
            //��һ�γ������Ҫ�������ʽmeaing(word,meaning2)��ʾ
            if (!termDic.ContainsKey(json.meaning))
            {
                fillWord = json.meaning + "(" + json.word + (string.IsNullOrEmpty(json.meaning2) ? "" : ("," + json.meaning2)) + ")";
                termDic.Add(fillWord, json);
                termDic.Add(json.meaning, json);
            }

            string front = res.Substring(0, mArr[i].Index - offset);
            string back = res.Substring(mArr[i].Index + mArr[i].Value.Length - offset, res.Length - mArr[i].Index - mArr[i].Value.Length + offset);
            res = front + "<" + fillWord + ">" + back;
            offset += mArr[i].Value.Length - fillWord.Length - 2;//<>ռ��������
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
    T GetMarkdownInfo<T>(string encodedInfo)
    {
        string json = CommonTool.DecodeBase64(encodedInfo);
        T termInfoJson = JsonUtility.FromJson<T>(json);
        return termInfoJson;
    }

    [Serializable]
    public class TermInfoJson
    {
        public string id;
        public string word;
        public string meaning;
        public string meaning2;
        public string channel;/**��������term���е�channel_id */
        public string parentChannelId;/**�������������ĵ�channel_id */
        public string parentStudioId;/**�������������ĵ�studio_id */
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
    //��ֻ��channel,�鲻����ֻ��owner�����鲻����ֻ��ʾID
    //TermInfo GetTermInfoByChannelAndOwner(string channel, string owner)
    //{
    //}
    TermInfo SelectTermInfo(DbAccess db, string channel, string owner)
    {
        //��ֻ��channel
        var reader = db.SelectSame("dhamma_terms", channel, "channel_id", 1);

        //����SQLite����  ������Ӧ����
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
        //�鲻����ֻ��owner
        //var reader = db.SelectSame("dhamma_terms", channel, "channel_id", 1);

        ////����SQLite����  ������Ӧ����
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
        //ͨ��UUID��term
        var reader = db.SelectSame("dhamma_terms", uuid, "uuid", 1);

        //����SQLite����  ������Ӧ����
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
    #endregion
    #region noteע��
    Dictionary<string, NoteJson> noteDic = new Dictionary<string, NoteJson>();
    // List<NoteJson> noteList = new List<NoteJson>();
    public string PrefilterSentenceNote(string sentence)
    {
        //sentence = "{{note|eyJub3RlIjoiYmxhIDxiPmJvbGQ8XC9iPiA8aT5lbTxcL2k+IGJsYVxuXG4iLCJ0cmlnZ2VyIjoia2FjYXlhbmEifQ==}}";
        string res = sentence;
        MatchCollection mcs = r_note.Matches(sentence);
        Match[] mArr = mcs.ToArray();
        int offset = 0;

        for (int i = 0; i < mArr.Length; i++)
        {
            string note = mArr[i].Value.Substring("{{note|".Length);
            note = note.Substring(0, note.Length - 2);
            //Debug.LogError(term);
            NoteJson json = GetMarkdownInfo<NoteJson>(note);

            string id = (noteDic.Count + 1).ToString() + (string.IsNullOrEmpty(json.trigger) ? "" : (":" + json.trigger));
            noteDic.Add(id, json);

            string front = res.Substring(0, mArr[i].Index - offset);
            string back = res.Substring(mArr[i].Index + mArr[i].Value.Length - offset, res.Length - mArr[i].Index - mArr[i].Value.Length + offset);
            res = front + "[ע��" + id + "]" + back;
            offset += mArr[i].Value.Length - id.ToString().Length - 4;//[ע��]ռ4������
        }
        return res;
    }
    public NoteJson GetNoteByID(string id)
    {
        if (noteDic.ContainsKey(id))
        {
            return noteDic[id];
        }
        else
            return null;
    }
    [Serializable]
    public class NoteJson
    {
        public string trigger;//��������ʾ�����֡����û�еĻ�����ͷŸ���ť���������Ƿ���һ��ͼ�ꡣ���ɶ���С�����word���� [1]Ҳ��
        public string note;//ע������
    }
    #endregion
}
