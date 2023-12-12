using CI.HttpClient;
using iTextSharp.text;
using iTextSharp.text.pdf.parser;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using UnityEngine;
using static ArticleController;
using static ArticleManager;
using static DictManager;

public class C2SDicGetInfo
{

    #region 获取社区词典

    [Serializable]
    public class WordListJson
    {
        public bool ok;
        public WordRowListJson data;
        public string message;
    }
    [Serializable]
    public class WordRowListJson
    {
        public List<WordJsonData> rows;
        public int count;
    }
    [Serializable]
    public class WordJsonData
    {
        //public string id;

        public string word;
        public string type;
        public string grammar;//语法
        public string mean;
        public string parent;//词干

        //public string note;
        //public string factors;
        //public string factormean;
        //public string language;
        //public IDictInfo dict;
        //public string dict_id;
        //public string dict_name;
        //public string dict_shortname;
        //public string shortname;
        // public intconfidence;
        //public intcreator_id;
        //public string updated_at;
        public WordUserJsonData editor;
        public int exp;
    }
    [Serializable]
    public class WordUserJsonData
    {
        //public int id;
        public string uid;
        public string nickName;
        //public string userName;
        //public string realName;
        //public string avatar;
    }
    //  static string communityDicJson;
    //wikipali.cc/api/v2/userdict?view=community&word=dhamma
    public static void GetCommunityDicData(string word, Func<string, object> callback)
    {
        HttpClient client = new HttpClient();
        string communityDicJson = "";
        //client.Get(new System.Uri("https://www.wikipali.cc/api/v2/userdict?view=community&word=dhamma"),
        client.Get(new System.Uri(string.Format(@"https://www.wikipali.cc/api/v2/userdict?view=community&word={0}", word)),
        // client.Get(new System.Uri(string.Format(@"wikipali.cc/api/v2/userdict?view=community&word={0}", word)),
        HttpCompletionOption.StreamResponseContent, (r) =>
        {
            //RightText.text = "Download: " + r.PercentageComplete.ToString() + "%";
            //ProgressSlider.value = 100 - r.PercentageComplete;
            byte[] responseData = r.ReadAsByteArray();
            string json = Encoding.Default.GetString(responseData);
            communityDicJson += json;
            //Debug.LogError(json);
            //如果消息长，会返回多次消息，已json结尾作为判断是否返回的是最后一条消息
            if (json.Contains("\"message\":\"\"}"))
            {

                //communityDicJson = communityDicJson.Substring(1);
                //communityDicJson = communityDicJson.Substring(0, communityDicJson.Length - 1);
                //communityDicJson = File.ReadAllText("Assets/jsonTest.txt");
                //Debug.LogError(communityDicJson);
                //用户名 按经验值排序
                List<UserData> user = new List<UserData>();
                List<string> userDuplicateCheck = new List<string>();
                //<内容,次数>
                Dictionary<string, int> typeCount = new Dictionary<string, int>();
                Dictionary<string, int> grammarCount = new Dictionary<string, int>();//语法
                Dictionary<string, int> meanCount = new Dictionary<string, int>();
                Dictionary<string, int> parentCount = new Dictionary<string, int>();//词干
                List<StringIntData> typeCountData = new List<StringIntData>();
                List<StringIntData> grammarCountData = new List<StringIntData>();
                List<StringIntData> meanCountData = new List<StringIntData>();
                List<StringIntData> parentCountData = new List<StringIntData>();
                //Debug.LogError(communityDicJson);
                WordListJson wordList = JsonUtility.FromJson<WordListJson>(communityDicJson);
                //处理数据
                int count = wordList.data.rows.Count;
                for (int i = 0; i < count; i++)
                {
                    if (!userDuplicateCheck.Contains(wordList.data.rows[i].editor.nickName))
                    {
                        userDuplicateCheck.Add(wordList.data.rows[i].editor.nickName);
                        UserData ud = new UserData(wordList.data.rows[i].editor.nickName, wordList.data.rows[i].exp);
                        bool isIn = false;
                        //冒泡排序
                        for (int j = 0; j < user.Count; j++)
                        {
                            if (ud.exp > user[j].exp)
                            {
                                user.Insert(j, ud);
                                isIn = true;
                                break;
                            }
                        }
                        if (!isIn)
                            user.Add(ud);
                    }
                    GetKVPData(ref typeCount, wordList.data.rows[i].type);
                    GetKVPData(ref grammarCount, wordList.data.rows[i].grammar);
                    GetKVPData(ref meanCount, wordList.data.rows[i].mean);
                    GetKVPData(ref parentCount, wordList.data.rows[i].parent);
                    //if (grammarCount.ContainsKey(wordList.data.rows[i].grammar))
                    //{
                    //    ++grammarCount[wordList.data.rows[i].grammar];
                    //}
                    //else
                    //{
                    //    grammarCount.Add(wordList.data.rows[i].grammar, 1);
                    //}
                }
                GetSIData(typeCount, ref typeCountData);
                GetSIData(grammarCount, ref grammarCountData);
                GetSIData(meanCount, ref meanCountData);
                GetSIData(parentCount, ref parentCountData);
                string res = GetCommunityDicString(user, typeCountData, grammarCountData, meanCountData, parentCountData);
                callback(res);
            }

        });
    }
    static string GetCommunityDicString(List<UserData> user, List<StringIntData> typeCountData, List<StringIntData> grammarCountData, List<StringIntData> meanCountData, List<StringIntData> parentCountData)
    {
        string res = "意思：";
        for (int i = 0; i < meanCountData.Count; i++)
        {
            res += meanCountData[i].value + " " + CommonTool.COLOR_BLUE_FLAG + meanCountData[i].count + CommonTool.COLOR_END_FLAG + "  ";
        }
        res += "\n语法：";
        for (int i = 0; i < grammarCountData.Count; i++)
        {
            res += grammarCountData[i].value + " " + CommonTool.COLOR_BLUE_FLAG + grammarCountData[i].count + CommonTool.COLOR_END_FLAG + "  ";
        }
        res += "\n词干：";
        for (int i = 0; i < parentCountData.Count; i++)
        {
            res += parentCountData[i].value + " " + CommonTool.COLOR_BLUE_FLAG + parentCountData[i].count + CommonTool.COLOR_END_FLAG + "  ";
        }
        res += "\n贡献者：";
        for (int i = 0; i < user.Count; i++)
        {
            if (i != 0)
                res += "，";
            res += user[i].name;
        }
        return res;
    }
    static void GetKVPData(ref Dictionary<string, int> count, string jsonValue)
    {
        if (count.ContainsKey(jsonValue))
        {
            ++count[jsonValue];
        }
        else
        {
            //不接受空值
            if (!string.IsNullOrEmpty(jsonValue))
                count.Add(jsonValue, 1);
        }
    }
    static void GetSIData(Dictionary<string, int> count, ref List<StringIntData> sidList)
    {
        foreach (KeyValuePair<string, int> kvp in count)
        {
            StringIntData sid = new StringIntData(kvp.Key, kvp.Value);
            sidList.Add(sid);
        }
        //排序，由高到低
        QuickSort(ref sidList, 0, sidList.Count - 1);
    }

    public class UserData
    {
        public UserData(string _name, int _exp)
        {
            name = _name;
            exp = _exp;
        }
        public string name;
        public int exp;
    }
    public class StringIntData
    {
        public StringIntData(string _value, int _count)
        {
            value = _value;
            count = _count;
        }
        public string value;
        public int count;
    }

    //用户名 按经验值排序
    //根据经验值排序
    static void QuickSort(ref List<StringIntData> array, int start, int end)
    {
        if (start < end)
        {
            int mid = Partition(ref array, start, end);
            QuickSort(ref array, start, mid - 1);
            QuickSort(ref array, mid + 1, end);
        }
    }

    //分治方法 把数组中一个数放置在确定的位置
    static int Partition(ref List<StringIntData> array, int start, int end)
    {
        StringIntData x = array[end];//选取一个判定值(一般选取最后一个)
        int i = start;

        for (int j = start; j < end; j++)
        {
            if (array[j].count > x.count)
            {
                //将下标j的值与下标i的值交换 保证i的前面都小于判定值
                StringIntData temp = array[j];
                array[j] = array[i];
                array[i] = temp;
                i++;
            }
        }



        //将下标i的值与判定值交换
        array[end] = array[i];
        array[i] = x;

        return i;
    }

    #endregion

}
