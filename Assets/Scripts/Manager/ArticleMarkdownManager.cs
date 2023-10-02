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
    Regex r_term = new Regex(@"\{\{term\|(\w+)\}\}");
    Regex r = new Regex(@"\{\{(\w+)\}\}");
    //�������µľ�����ʾ����
    public string SentenceSetMarkDown(string sentence, string channel, string owner)
    {
        string res = PrefilterSentenceTerm(sentence);

        return res;
    }
    Dictionary<string, TermInfoJson> termDic = new Dictionary<string, TermInfoJson>();
    string PrefilterSentenceTerm(string sentence)
    {
        termDic.Clear();
        string res = sentence;
        MatchCollection mcs = r_term.Matches(sentence);
        Match[] mArr = mcs.ToArray();
        int offset = 0;

        for (int i = 0; i < mArr.Length; i++)
        {
            string term = mArr[i].Value.Substring("{{term|".Length);
            term = term.Substring(0, term.Length - 2);
            TermInfoJson json = GetTermInfo(term);
            //Debug.LogError(mArr[i].Index + "," + mArr[i].Value);
            if (!termDic.ContainsKey(json.word))
            {
                termDic.Add(json.word, json);
            }
            string front = res.Substring(0, mArr[i].Index - offset);
            string back = res.Substring(mArr[i].Index + mArr[i].Value.Length - offset, res.Length - mArr[i].Index - mArr[i].Value.Length + offset);
            res = front + "<" + json.word + ">" + back;
            offset += mArr[i].Value.Length - json.word.Length - 2;//<>ռ��������
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
}
