using GeoTimeZone;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeZoneManager
{
    private TimeZoneManager() { }
    private static TimeZoneManager manager = null;
    //静态工厂方法 
    public static TimeZoneManager Instance()
    {
        if (manager == null)
        {
            manager = new TimeZoneManager();
        }
        return manager;
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
    //挂在camera
    public TimeZoneLookup timeZoneLookup;
    static Dictionary<string, string> IANATime = new Dictionary<string, string>();
    static Dictionary<string, string> IANA2WIN = new Dictionary<string, string>();
    public void InitData(TimeZoneLookup _timeZoneLookup)
    {
        timeZoneLookup = _timeZoneLookup;
        timeZoneLookup.LoadData();

        Timezones defaultTS = JsonUtility.FromJson<Timezones>(ReadJsonFromStreamingAssetsPath("Json/TimeZone/IANATimeZone"));

        for (int i = 0; i < defaultTS.info.Count; i++)
        {
            IANATime.Add(defaultTS.info[i].name, defaultTS.info[i].time);
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

    public TimeZoneInfo CaculateTimeZone(float lat,float log)
    {
        TimeZoneResult res = timeZoneLookup.GetTimeZone(lat, log);
        Debug.LogError(res.Result);
        //TimeZoneConverter
        DateTimeOffset dateTimeOffset = DateTimeOffset.Now;
        string tz = IANA2WIN[res.Result];
        TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(tz);
        //TimeSpan offset = timeZoneInfo.GetUtcOffset(dateTimeOffset.DateTime);
        return timeZoneInfo;
        //Debug.LogError("windows:h:" + offset.Hours + "m:" + offset.Minutes);
        //if (res.Result.Contains("GMT"))
        //{
        //    //Etc/GMT-6
        //    Debug.LogError("IANA:" + res.Result);
        //}
        //else
        //    Debug.LogError("IANA:" + test[res.Result]);
        //return 0;
    }
    public TimeZoneInfo GetTimeZoneByName(string name)
    {
        TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(name);
        return timeZoneInfo;
    }
    /* 根据经度获取时区；例如121：+8;-121：-8;返回值为字符串（返回正数时候带+符号）
*https://download.csdn.net/blog/column/11112744/124842449
* @param currentLon
* @return
*/
    //public static int CaculateTimeZone(float currentLon)
    //{
    //    int timeZone;
    //    int shangValue = (int)(currentLon / 15);
    //    float yushuValue = Math.Abs(currentLon % 15);
    //    if (yushuValue <= 7.5)
    //    {
    //        timeZone = shangValue;
    //    }
    //    else
    //    {
    //        timeZone = shangValue + (currentLon > 0 ? 1 : -1);
    //    }
    //    return timeZone >= 0 ? Math.Abs(timeZone) : -Math.Abs(timeZone);
    //}
}
