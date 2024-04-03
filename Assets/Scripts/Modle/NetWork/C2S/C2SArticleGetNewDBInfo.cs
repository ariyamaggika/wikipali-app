using CI.HttpClient;
using iTextSharp.text;
using iTextSharp.text.pdf.parser;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using UnityEngine;
using static ArticleController;
using static ArticleManager;
using static C2SDicGetInfo;
using static DictManager;

public class C2SArticleGetNewDBInfo
{
    //获取所有版本风格
    public static void GetChannelDataBatch(string view = "public", string update_at = "", int limit = 30, int offset = 0)
    {
        HttpClient client = new HttpClient();

        //ProgressSlider.value = 100;
        //client.Get(new System.Uri("https://staging.wikipali.org/api/v2/channel-io?view=public&limit=30&offset=0"), HttpCompletionOption.StreamResponseContent, (r) =>
        client.Get(new System.Uri(string.Format(@"https://staging.wikipali.org/api/v2/channel-io?view={0}&limit={1}&offset={2}", view, limit, offset)),
            HttpCompletionOption.StreamResponseContent, (r) =>
        {
            //RightText.text = "Download: " + r.PercentageComplete.ToString() + "%";
            //ProgressSlider.value = 100 - r.PercentageComplete;
            byte[] responseData = r.ReadAsByteArray();
            string json = Encoding.Default.GetString(responseData);
            Debug.LogError(json);
        });
    }
    //根据channel id获取 channel信息
    #region 根据channel id获取 channel信息
    public class ChannelDataListJson
    {
        public bool ok;
        public string message;
        public List<ChannelDataJson> data;
    }
    public class ChannelDataJson
    {
        public string uid;
        public string name;
        public string summary;
        public string owner_uid;
    }
    public static void GetChannelDataByID(string channelID)
    {
        HttpClient client = new HttpClient();

        client.Get(new System.Uri(string.Format(@"https://staging.wikipali.org/api/v2/channel/{0}", channelID)),
            HttpCompletionOption.StreamResponseContent, (r) =>
            {
                //RightText.text = "Download: " + r.PercentageComplete.ToString() + "%";
                //ProgressSlider.value = 100 - r.PercentageComplete;
                byte[] responseData = r.ReadAsByteArray();
                string json = Encoding.Default.GetString(responseData);
                Debug.LogError(json);
            });
    }

    #endregion

    #region 获取文章chapter 信息(包含进度&title)
    public class AllChapterDataJson
    {
        public bool ok;
        public List<ChapterMessageDataJson> message;
    }
    public class ChapterMessageDataJson
    {
        public string title;
        public string toc;
        public string book;
        public string para;
        public string path;
        public List<ChapterTagDataJson> tags;
        public List<ChapterChannelDataJson> channel;
        public List<ChapterStudioDataJson> studio;
        public string channel_id;
        public string summary;
        public int view;
        public int like;
        public int status;
        public int progress;
        public int progress_line;
        public string created_at;
        public string updated_at;
    }
    public class ChapterTagDataJson
    {
        public string id;
        public string name;
        public string description;
    }
    public class ChapterChannelDataJson
    {
        public string name;
        public string owner_uid;
    }
    public class ChapterStudioDataJson
    {
        public string id;
        public string nickName;
        public string studioName;
        public string realName;
        public string avatar;
    }
    public static void GetChapterData(string channelID)
    {



    }

    #endregion

    #region 获取文章chapter 信息
    public class ChapterDataListJson
    {
        public List<ChapterDatJson> list;
    }
    public class ChapterDatJson
    {
        public string id;
        public int bookID;
        public int paragraph;//段落数
        public string language;
        public string title;
        public string channel_id;
        public ChannelChapterDBData channelData;
        public float progress;

    }
    public static void GetChapterDataByBookIDList(List<int> bookIDList)
    {



    }
    public static void GetChapterDataByBookID(int bookIDList)
    {
        List<int> idList = new List<int>(bookIDList);
        GetChapterDataByBookIDList(idList);

    }
    #endregion
    #region 获取文章Sentence内容
    [Serializable]
    public class SentenceDataListJson
    {
        public bool ok;
        public SentenceDataRowsJson data;
        public string message;
    }
    [Serializable]
    public class SentenceDataRowsJson
    {
        public int count;
        public List<SentenceDataJson> rows;
    }
    [Serializable]
    public class SentenceDataJson
    {
        public string id;
        public string html;
        public int book;
        public int paragraph;
        public int word_start;
        public int word_end;
    }

    //https://staging.wikipali.org/api/v2/sentence?view=paragraph&book=66&para=1182,1183&channels=7fea264d-7a26-40f8-bef7-bc95102760fb&html=true&format=unity
    public static void GetSentenceData(int bookID, string channelID, int parMin, int parMax, Func<List<SentenceDBData>, object> callback)
    {
        string parList = "";
        for (int i = parMin; i < parMax + 1; i++)
        {
            parList += i;
            if (i != parMax)
            {
                parList += ",";
            }
        }
        HttpClient client = new HttpClient();
        string allJson = "";

        client.Get(new System.Uri(string.Format(@"https://next.wikipali.org/api/v2/sentence?view=paragraph&book={0}&para={1}&channels={2}&html=true&format=unity", bookID, parList, channelID)),
        //client.Get(new System.Uri(@"https://next.wikipali.org/api/v2/sentence?view=paragraph&book=66&para=1182,1183&channels=7fea264d-7a26-40f8-bef7-bc95102760fb&html=true&format=unity"),
            HttpCompletionOption.StreamResponseContent, (r) =>
            {
                //RightText.text = "Download: " + r.PercentageComplete.ToString() + "%";
                //ProgressSlider.value = 100 - r.PercentageComplete;
                byte[] responseData = r.ReadAsByteArray();
                string json = Encoding.Default.GetString(responseData);
                //Debug.LogError(json);
                allJson += json;
                if (json.Contains("\"message\":\"\"}"))
                {
                    SentenceDataListJson dataList = JsonUtility.FromJson<SentenceDataListJson>(allJson);
                    List<SentenceDBData> sentenceTransOnline = new List<SentenceDBData>();
                    int c = dataList.data.rows.Count;
                    for (int i = 0; i < c; i++)
                    {
                        SentenceDataJson ad = dataList.data.rows[i];
                        SentenceDBData data = new SentenceDBData(ad.book,
                          ad.paragraph, ad.word_start, ad.word_end,ad.html);
                        sentenceTransOnline.Add(data);
                    }
                    callback(sentenceTransOnline);
                }
            });
    }

    #endregion

    #region 获取最新文章列表
    [Serializable]
    public class NewArticleListJson
    {
        public bool ok;
        public NewArticleRowListJson data;
        public string message;
    }
    [Serializable]
    public class NewArticleRowListJson
    {
        public List<NewArticleData> rows;
        public int count;
    }
    [Serializable]
    public class NewArticleData
    {
        public string uid;
        public int book;
        public int para;
        public string channel_id;
        public string title;
        public string toc;
        public string path;
        public float progress;
        public string summary;
        public string created_at;
        public string updated_at;
        public ChannelData channel;
    }
    [Serializable]
    public class ChannelData
    {
        public string name;
        public string owner_uid;
    }
    //https://next.wikipali.cc/api/v2/progress?view=chapter&lang=zh&channel_type=translation&limit=10&offset=0
    public static void GetNewArticleList(Func<NewArticleListJson, object> callback)
    {
        HttpClient client = new HttpClient();
        string allJson = "";

        client.Get(new System.Uri(string.Format(@"https://next.wikipali.org/api/v2/progress?view=chapter&lang={0}&channel_type=translation&limit={1}&offset=0", "zh", 10)),
            HttpCompletionOption.StreamResponseContent, (r) =>
            {
                //RightText.text = "Download: " + r.PercentageComplete.ToString() + "%";
                //ProgressSlider.value = 100 - r.PercentageComplete;
                byte[] responseData = r.ReadAsByteArray();
                string json = Encoding.Default.GetString(responseData);
                allJson += json;
                //Debug.LogError(json);
                if (json.Contains("\"message\":\"\"}"))
                {
                    NewArticleListJson dataList = JsonUtility.FromJson<NewArticleListJson>(allJson);
                    callback(dataList);
                }
            });
    }



    #endregion


}
