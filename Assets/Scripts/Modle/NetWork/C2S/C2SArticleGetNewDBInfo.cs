﻿using CI.HttpClient;
using iTextSharp.text.pdf.parser;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static ArticleManager;

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


    #endregion



}
