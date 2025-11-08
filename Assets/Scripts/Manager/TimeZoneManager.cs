using GeoTimeZone;
using NodaTime.Extensions;
using NodaTime;
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
    public class TimezoneSpan
    {
        //public KeyValuePair<string, string> kvp;
        public string name;
        public TimeSpan time;
    }
    //挂在camera
    public TimeZoneLookup timeZoneLookup;
    static Dictionary<string, string> IANATime = new Dictionary<string, string>();
    static Dictionary<string, string> IANA2WIN = new Dictionary<string, string>();
    static Dictionary<string, string> WIN2IANA = new Dictionary<string, string>();
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
                WIN2IANA.TryAdd(defaultIS.info[i].WinTime, splits[j]);
            }
        }
    }
    public String CaculateTimeZoneName(float lat, float log)
    {
        TimeZoneResult res = timeZoneLookup.GetTimeZone(lat, log);
        Debug.LogError(res.Result);
        //TimeZoneConverter
        DateTimeOffset dateTimeOffset = DateTimeOffset.Now;
#if DEBUG_PERFORMANCE || UNITY_EDITOR
        //string tz = IANA2WIN[res.Result];
        string tz = res.Result;

#else
        string tz = res.Result;
#endif

        return tz;

    }
    public string Win2Iana(string name)
    {
        return WIN2IANA[name];
    }
    public TimezoneSpan CaculateTimeZone(float lat, float log)
    {
        TimeZoneResult res = timeZoneLookup.GetTimeZone(lat, log);
        Debug.LogError(res.Result);
        //TimeZoneConverter
        DateTimeOffset dateTimeOffset = DateTimeOffset.Now;
#if DEBUG_PERFORMANCE || UNITY_EDITOR
        //string tz = IANA2WIN[res.Result];
        string tz = res.Result;

#else
        string tz = res.Result;
#endif
        TimezoneSpan timeZoneInfo = TimeZoneManager.Instance().GetTimeZoneByAddress(tz, dateTimeOffset.DateTime);
        //TimeZoneInfo.FindSystemTimeZoneById(tz);
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
    //public TimezoneSpan GetTimeZoneByName(string name, DateTime dt)
    //{
    //    TimezoneSpan timeZoneInfo = CalendarManager.Instance().GetTimeZoneByAddress(name, dt);
    //    //TimeZoneInfo.FindSystemTimeZoneById(name);
    //    return timeZoneInfo;
    //}
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

    #region 获取时区时差
    //这个API算时差一定要用local time才准，如果是选择了有夏令时的地区，需要根据UTC时间加上时差才行，所以要算两次时差
    public TimeSpan GetTimeSpanByAddress(string name, DateTime dt)
    {
        Instant dti = dt.ToInstant();// SystemClock.Instance.GetCurrentInstant();
        //nowCity.timeZone = TimeZoneInfo.FindSystemTimeZoneById("Greenwich Standard Time");
        //"Asia/Taipei"
        NodaTime.DateTimeZone tz = DateTimeZoneProviders.Tzdb[name];
        TimeSpan ts = TimeSpan.FromSeconds(tz.GetUtcOffset(dti).Seconds);
        //todo 可能有未知bug
        //美国等时区,获取准确的夏令时冬令时，这个API需要local time
        //if (ts.Hours < 0)
        {
            dt -= ts;
            dti = dt.ToInstant();// SystemClock.Instance.GetCurrentInstant();
                                 //nowCity.timeZone = TimeZoneInfo.FindSystemTimeZoneById("Greenwich Standard Time");
                                 //"Asia/Taipei"
            ts = TimeSpan.FromSeconds(tz.GetUtcOffset(dti).Seconds);
        }

        //Debug.LogError(tz.GetUtcOffset(dti).Seconds / 3600);
        return ts;
    }
    public TimezoneSpan GetTimeZoneByAddress(string name, DateTime dt)
    {
        if (string.IsNullOrEmpty(name))
        {
            TimezoneSpan tzn = new TimezoneSpan();
            tzn.time = new TimeSpan();
            tzn.name = "";
            return tzn;
        }
        Debug.LogError(name);
        Debug.LogError(dt);
        Instant dti = dt.ToInstant();// SystemClock.Instance.GetCurrentInstant();
        //nowCity.timeZone = TimeZoneInfo.FindSystemTimeZoneById("Greenwich Standard Time");
        //"Asia/Taipei"
        NodaTime.DateTimeZone tz = DateTimeZoneProviders.Tzdb[name];
        TimeSpan ts = TimeSpan.FromSeconds(tz.GetUtcOffset(dti).Seconds);
        //todo 可能有未知bug
        //美国等时区,获取准确的夏令时冬令时，这个API需要local time
        //if (ts.Hours < 0)
        {
            dt -= ts;
            dti = dt.ToInstant();// SystemClock.Instance.GetCurrentInstant();
                                 //nowCity.timeZone = TimeZoneInfo.FindSystemTimeZoneById("Greenwich Standard Time");
                                 //"Asia/Taipei"
            ts = TimeSpan.FromSeconds(tz.GetUtcOffset(dti).Seconds);
        }


        //Debug.LogError(tz.GetUtcOffset(dti).Seconds / 3600);
        TimezoneSpan tzs = new TimezoneSpan();
        tzs.time = ts;
        tzs.name = name;
        return tzs;
    }
    public bool GetIsDayLightSavingTimeByAddress(string name, DateTime dt)
    {
        Instant dti = dt.ToInstant();// SystemClock.Instance.GetCurrentInstant();
        //nowCity.timeZone = TimeZoneInfo.FindSystemTimeZoneById("Greenwich Standard Time");
        //"Asia/Taipei"
        NodaTime.DateTimeZone tz = DateTimeZoneProviders.Tzdb[name];
        //todo 可能有未知bug
        //美国等时区,获取准确的夏令时冬令时，这个API需要local time
        TimeSpan ts = TimeSpan.FromSeconds(tz.GetUtcOffset(dti).Seconds);
        //if (ts.Hours < 0)
        {
            dt -= ts;
            dti = dt.ToInstant();// SystemClock.Instance.GetCurrentInstant();
                                 //nowCity.timeZone = TimeZoneInfo.FindSystemTimeZoneById("Greenwich Standard Time");
                                 //"Asia/Taipei"
        }

        ZonedDateTime now = new ZonedDateTime(dti, tz);
        var daylight = now.IsDaylightSavingTime();
        //TimeSpan ts = TimeSpan.FromSeconds(tz.GetUtcOffset(dti).Seconds);
        ////Debug.LogError(tz.GetUtcOffset(dti).Seconds / 3600);
        //TimezoneSpan tzs = new TimezoneSpan();
        //tzs.time = ts;
        //tzs.name = name;
        return daylight;
    }
    #endregion
}
