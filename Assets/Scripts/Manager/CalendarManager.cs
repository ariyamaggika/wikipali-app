using SunCalcNet;
using SunCalcNet.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Experimental.GlobalIllumination;
using static SelectCityController;

public class CalendarManager
{
    private CalendarManager() { }
    private static CalendarManager manager = null;
    //静态工厂方法 
    public static CalendarManager Instance()
    {
        if (manager == null)
        {
            manager = new CalendarManager();
        }
        //Input.location.Start();
        return manager;
    }
    LocationService location = Input.location;
    public void StartLocation()
    {
        location.Start();
    }
    public void StopLocation()
    {
        location.Stop();
    }
    public (float, float) GetLocation()
    {
        location = Input.location;
        //return (7.2f,80.1f );
#if UNITY_EDITOR
        return (24.91675f, 103.16268f);
        //return (24.91675f, 103.16268f);
        //return (15, 100);//thai
        //return (-79.2f, 43.9f);//美国
        //return (7.2f, 80.1f);
#endif
        return (location.lastData.latitude, location.lastData.longitude);
    }
    //日中时间
    public string GetSunSolarNoonTime(DateTime time, CityInfo cityInfo)//, float lat, float lng, float height = 0)
    {
        //?为什么0h1m1s显示为前一天？，一定要1h1m1s才行
        DateTime newDate = new DateTime(time.Year, time.Month, time.Day, 3, 1, 0);
        SunPhase solarNoon = new SunPhase(SunPhaseName.SolarNoon, newDate);
        //时区时差
        TimeSpan ts = cityInfo.timeZone.GetUtcOffset(time);
        float lat = cityInfo.lat;
        float lng = cityInfo.lng;
        //(lat, lng) = GetLocation();

        var height = 0;// 2000;
        //Act
        var sunPhases = SunCalc.GetSunPhases(newDate, lat, lng, height, 0).ToList();
        TimeSpan sp = new TimeSpan();// TimeZoneInfo.Local.GetUtcOffset(time);
        TimeSpan BaseUtcOffset = new TimeSpan(ts.Hours, ts.Minutes, ts.Seconds);
        sp = -BaseUtcOffset;// ts;// sp -  BaseUtcOffsetSriLanka;// targetTimeZone.GetUtcOffset(time);


        var sunPhaseValueSolarNoon = sunPhases.First(x => x.Name.Value == solarNoon.Name.Value);
        //string sunPhaseTimeSolarNoon = sunPhaseValueSolarNoon.PhaseTime.ToString("HH:mm:ss");
        string sunPhaseTimeSolarNoon = (sunPhaseValueSolarNoon.PhaseTime - sp).ToString("HH:mm");
        return sunPhaseTimeSolarNoon;
    }


    bool locationed = false;

    public bool isLocationed()
    {
#if UNITY_EDITOR
        return true;
#endif

        float lat = 24;
        float lng = 103;
        (lat, lng) = CalendarManager.Instance().GetLocation();
        if (lat == 0 && lng == 0)
            return false;
        else
            return true;

    }
    //保存10个查询历史
    const int CITY_HISTORY_COUNT = 10;
    Stack<int> idSt;
    Stack<string> nameSt;
    Stack<string> pNameSt;
    Stack<int> pCodeSt;
    Stack<string> fullNameSt;
    Stack<float> lngSt;
    Stack<float> latSt;
    Stack<string> timeZoneIDSt;
    Stack<int> countryIDSt;
    Stack<int> statesIDSt;
    Stack<string> countryCodeSt;
    //饿汉式，先读取
    public void LoadCityHistory()
    {
        int[] ids = PlayerPrefsX.GetIntArray("cityHistory_id");
        string[] names = PlayerPrefsX.GetStringArray("cityHistory_name");
        //string[] levels = PlayerPrefsX.GetStringArray("cityHistory_level");
        string[] pNames = PlayerPrefsX.GetStringArray("cityHistory_pName");
        int[] pCodes = PlayerPrefsX.GetIntArray("cityHistory_pCode");
        string[] fullNames = PlayerPrefsX.GetStringArray("cityHistory_fullName");
        float[] lngs = PlayerPrefsX.GetFloatArray("cityHistory_lng");
        float[] lats = PlayerPrefsX.GetFloatArray("cityHistory_lat");
        string[] timeZoneIDs = PlayerPrefsX.GetStringArray("cityHistory_timeZoneID");
        int[] countryIDs = PlayerPrefsX.GetIntArray("cityHistory_countryID");
        int[] statesIDs = PlayerPrefsX.GetIntArray("cityHistory_statesID");
        string[] countryCodes = PlayerPrefsX.GetStringArray("cityHistory_countryCode");
        idSt = new Stack<int>(ids);
        nameSt = new Stack<string>(names);
        pNameSt = new Stack<string>(pNames);
        pCodeSt = new Stack<int>(pCodes);
        fullNameSt = new Stack<string>(fullNames);
        lngSt = new Stack<float>(lngs);
        latSt = new Stack<float>(lats);
        timeZoneIDSt = new Stack<string>(timeZoneIDs);
        countryIDSt = new Stack<int>(countryIDs);
        statesIDSt = new Stack<int>(statesIDs);
        countryCodeSt = new Stack<string>(countryCodes);

    }

    public void AddCityHistorySave(CityInfo ci)
    {
        //todo 查重，并且提升到第一位
        //todo Dictionary<Language, string> transName
        idSt.Push(ci.id);
        if (idSt.Count > CITY_HISTORY_COUNT)
            idSt.Pop();
        PlayerPrefsX.SetIntArray("cityHistory_id", idSt.ToArray());
        nameSt.Push(ci.name);
        if (nameSt.Count > CITY_HISTORY_COUNT)
            nameSt.Pop();
        PlayerPrefsX.SetStringArray("cityHistory_name", nameSt.ToArray());
        pNameSt.Push(ci.pName);
        if (pNameSt.Count > CITY_HISTORY_COUNT)
            pNameSt.Pop();
        PlayerPrefsX.SetStringArray("cityHistory_pName", pNameSt.ToArray());
        pCodeSt.Push(ci.pCode);
        if (pCodeSt.Count > CITY_HISTORY_COUNT)
            pCodeSt.Pop();
        PlayerPrefsX.SetIntArray("cityHistory_pCode", pCodeSt.ToArray());
        fullNameSt.Push(ci.fullName);
        if (fullNameSt.Count > CITY_HISTORY_COUNT)
            fullNameSt.Pop();
        PlayerPrefsX.SetStringArray("cityHistory_fullName", fullNameSt.ToArray());
        lngSt.Push(ci.lng);
        if (lngSt.Count > CITY_HISTORY_COUNT)
            lngSt.Pop();
        PlayerPrefsX.SetFloatArray("cityHistory_lng", lngSt.ToArray());
        latSt.Push(ci.lat);
        if (latSt.Count > CITY_HISTORY_COUNT)
            latSt.Pop();
        PlayerPrefsX.SetFloatArray("cityHistory_lat", latSt.ToArray());
        timeZoneIDSt.Push(ci.timeZone.Id);
        if (timeZoneIDSt.Count > CITY_HISTORY_COUNT)
            timeZoneIDSt.Pop();
        PlayerPrefsX.SetStringArray("cityHistory_timeZoneID", timeZoneIDSt.ToArray());
        countryIDSt.Push(ci.countryID);
        if (countryIDSt.Count > CITY_HISTORY_COUNT)
            countryIDSt.Pop();
        PlayerPrefsX.SetIntArray("cityHistory_countryID", countryIDSt.ToArray());
        statesIDSt.Push(ci.statesID);
        if (statesIDSt.Count > CITY_HISTORY_COUNT)
            statesIDSt.Pop();
        PlayerPrefsX.SetIntArray("cityHistory_statesID", statesIDSt.ToArray());
        countryCodeSt.Push(ci.countryCode);
        if (countryCodeSt.Count > CITY_HISTORY_COUNT)
            countryCodeSt.Pop();
        PlayerPrefsX.SetStringArray("cityHistory_countryCode", countryCodeSt.ToArray());
    }
    public List<CityInfo> GetCityHistorySave()
    {
        List<CityInfo> cityInfos = new List<CityInfo>();

        int[] idA = idSt.ToArray();
        string[] nameA = nameSt.ToArray();
        string[] pNameA = pNameSt.ToArray();
        int[] pCodeA = pCodeSt.ToArray();
        string[] fullNameA = fullNameSt.ToArray();
        float[] lngA = lngSt.ToArray();
        float[] latA = latSt.ToArray();
        string[] timeZoneIDA = timeZoneIDSt.ToArray();
        int[] countryIDA = countryIDSt.ToArray();
        int[] statesIDA = statesIDSt.ToArray();
        string[] countryCodeA = countryCodeSt.ToArray();

        for (int i = 0; i < idA.Length; i++)
        {
            CityInfo ci = new CityInfo();
            ci.id = idA[i];
            if (nameA != null && nameA.Length > i)
                ci.name = nameA[i];
            if (pNameA != null && pNameA.Length > i)
                ci.pName = pNameA[i];
            if (pCodeA != null && pCodeA.Length > i)
                ci.pCode = pCodeA[i];
            if (fullNameA != null && fullNameA.Length > i)
                ci.fullName = fullNameA[i];
            if (lngA != null && lngA.Length > i)
                ci.lng = lngA[i];
            if (latA != null && latA.Length > i)
                ci.lat = latA[i];
            if (timeZoneIDA != null && timeZoneIDA.Length > i)
                ci.timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneIDA[i]);
            if (countryIDA != null && countryIDA.Length > i)
                ci.countryID = countryIDA[i];
            if (statesIDA != null && statesIDA.Length > i)
                ci.statesID = statesIDA[i];
            if (countryCodeA != null && countryCodeA.Length > i)
                ci.countryCode = countryCodeA[i];
            ci.transName = new Dictionary<SettingManager.Language, string>();
            cityInfos.Add(ci);
        }

        return cityInfos;
    }
}
