using CI.HttpClient;
using GeoTimeZone;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using ZXing.Common;
using static ArticleController;
using static ArticleManager;
using static C2SGetTimeZone;

public class TestHttp : MonoBehaviour
{
    public Button button;
    public InputField inputField;
    public InputField inputField2;
    public TimeZoneLookup timeZoneLookup;

    // Start is called before the first frame update
    void Start()
    {
        timeZoneLookup.LoadData();
        //GetChannel();
        //PostChannel();
        button.onClick.AddListener(OnBtnClick);
        Timezones defaultTS = JsonUtility.FromJson<Timezones>(ReadJsonFromStreamingAssetsPath("Json/TimeZone/IANATimeZone"));

        for (int i = 0; i < defaultTS.info.Count; i++)
        {
            test.Add(defaultTS.info[i].name, defaultTS.info[i].time);
        }
        IANAWinNames defaultIS = JsonUtility.FromJson<IANAWinNames>(ReadJsonFromStreamingAssetsPath("Json/TimeZone/IANAWindowsMapping"));
        for (int i = 0; i < defaultIS.info.Count; i++)
        {
            string[] splits = defaultIS.info[i].IANATime.Split(' ');
            for (int j = 0; j < splits.Length; j++)
            {
                IANA2WIN.TryAdd(splits[j], defaultIS.info[i].WinTime);
            }
        }
    }
    [Serializable]
    public class Timezone
    {
        //public KeyValuePair<string, string> kvp;
        public string name;
        public string time;
    }
    [Serializable]
    public class Timezones
    {
        public List<Timezone> info;
    }
    string ReadJsonFromStreamingAssetsPath(string jsonName)
    {
        TextAsset textAsset = Resources.Load<TextAsset>(jsonName);
        return textAsset.text;
    }
    [Serializable]
    public class IANAWinName
    {
        //public KeyValuePair<string, string> kvp;
        public string IANATime;
        public string WinTime;
    }
    [Serializable]
    public class IANAWinNames
    {
        public List<IANAWinName> info;
    }

    public int CaculateTimeZone2(float lat, float log)
    {

        TimeZoneResult res = timeZoneLookup.GetTimeZone(lat, log);
        Debug.LogError(res.Result);
        //TimeZoneConverter
        DateTimeOffset dateTimeOffset = DateTimeOffset.Now;
        string tz = IANA2WIN[res.Result];
        TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(tz);
        TimeSpan offset = timeZoneInfo.GetUtcOffset(dateTimeOffset.DateTime);
        Debug.LogError("windows:h:" + offset.Hours + "m:" + offset.Minutes);
        if (res.Result.Contains("GMT"))
        {
            //Etc/GMT-6
            Debug.LogError("IANA:" + res.Result);
        }
        else
            Debug.LogError("IANA:" + test[res.Result]);
        return 0;
    }
    static Dictionary<string, string> test = new Dictionary<string, string>();
    static Dictionary<string, string> IANA2WIN = new Dictionary<string, string>();
    void OnBtnClick()
    {

        float lat = float.Parse(inputField.text);
        float lon = float.Parse(inputField2.text);
        Debug.LogError(/*CommonTool.*/CaculateTimeZone2(lat, lon));
        //筛选IANA有的IANA2Win的缺失字段
        //foreach (KeyValuePair<string, string> iana in test)
        //{
        //    if (!IANA2WIN.ContainsKey(iana.Key))
        //    {
        //        Debug.LogError(iana.Key);
        //    }
        
        //}

        //C2SGetTimeZone.GetCommunityDicData(lat, lon, OnTimeZoneCallBack);

    }
    public object OnTimeZoneCallBack(TimeZoneJsonData tz)
    {
        Debug.LogError(tz.offset);
        return null;
    }
    // Update is called once per frame
    void Update()
    {

    }
    public void GetChannel()
    {
        HttpClient client = new HttpClient();

        //ProgressSlider.value = 100;
        //client.Get(new System.Uri("http://staging.wikipali.org/api/v2/channel-io"), HttpCompletionOption.StreamResponseContent, (r) =>
        client.Get(new System.Uri("https://staging.wikipali.org/api/v2/channel-io?view=public&limit=30&offset=0"), HttpCompletionOption.StreamResponseContent, (r) =>
        {
            //RightText.text = "Download: " + r.PercentageComplete.ToString() + "%";
            //ProgressSlider.value = 100 - r.PercentageComplete;
            byte[] responseData = r.ReadAsByteArray();
            //!!!以前是Encoding.Default.GetString(responseData);在MAC/IOS苹果机器上中文会出现乱码，所以改成UTF8格式
            string json = Encoding.UTF8.GetString(responseData);
            // string json = CommonTool.ByteToJsonUtil.ByteToJson(responseData);
            Debug.LogError(json);
            //תΪjson
        });
    }
    [Serializable]
    public class TestP
    {
        public string view = "public";
        public string update_at = "";
        public int limit = 30;
        int offset = 0;
    }
    public void PostChannel()
    {
        HttpClient client = new HttpClient();
        TestP t = new TestP();

        byte[] buffer = CommonTool.ObjectToByteArray(t);
        //new System.Random().NextBytes(buffer);

        ByteArrayContent content = new ByteArrayContent(buffer, "application/bytes");

        client.Post(new System.Uri("http://staging.wikipali.org/api/v2/channel-io"), content, HttpCompletionOption.StreamResponseContent, (r) =>
        {
            byte[] responseData = r.ReadAsByteArray();
            string json = CommonTool.ByteToJsonUtil.ByteToJson(responseData);
            Debug.LogError(json);
        }, (u) =>
        {
            //LeftText.text = "Upload: " + u.PercentageComplete.ToString() + "%";
            //ProgressSlider.value = u.PercentageComplete;
        });
    }
}
