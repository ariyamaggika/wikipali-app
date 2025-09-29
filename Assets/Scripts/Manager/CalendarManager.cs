using SunCalcNet;
using SunCalcNet.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Experimental.GlobalIllumination;
using static SelectCityController;
using Org.BouncyCastle.Crypto;
using System.Security.Cryptography;
using static ZXing.QrCode.Internal.Mode;

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
    List<int> idSt;
    List<string> nameSt;
    List<string> pNameSt;
    List<int> pCodeSt;
    List<string> fullNameSt;
    List<float> lngSt;
    List<float> latSt;
    List<string> timeZoneIDSt;
    List<int> countryIDSt;
    List<int> statesIDSt;
    List<string> countryCodeSt;
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
        idSt = new List<int>(ids);
        nameSt = new List<string>(names);
        pNameSt = new List<string>(pNames);
        pCodeSt = new List<int>(pCodes);
        fullNameSt = new List<string>(fullNames);
        lngSt = new List<float>(lngs);
        latSt = new List<float>(lats);
        timeZoneIDSt = new List<string>(timeZoneIDs);
        countryIDSt = new List<int>(countryIDs);
        statesIDSt = new List<int>(statesIDs);
        countryCodeSt = new List<string>(countryCodes);

    }

    public void AddCityHistorySave(CityInfo ci)
    {
        //todo 查重，并且提升到第一位
        //todo Dictionary<Language, string> transName
        if (idSt.Contains(ci.id) && nameSt.Contains(ci.name))
        {
            int idi = idSt.IndexOf(ci.id);
            int idn = nameSt.IndexOf(ci.name);
            if (idi == idn)
            {
                idSt.RemoveAt(idi);
                nameSt.RemoveAt(idi);
                pNameSt.RemoveAt(idi);
                pCodeSt.RemoveAt(idi);
                fullNameSt.RemoveAt(idi);
                lngSt.RemoveAt(idi);
                latSt.RemoveAt(idi);
                timeZoneIDSt.RemoveAt(idi);
                countryIDSt.RemoveAt(idi);
                statesIDSt.RemoveAt(idi);
                countryCodeSt.RemoveAt(idi);
            }
        }
        idSt.Add(ci.id);
        if (idSt.Count > CITY_HISTORY_COUNT)
            idSt.RemoveAt(0);
        PlayerPrefsX.SetIntArray("cityHistory_id", idSt.ToArray());
        nameSt.Add(ci.name);
        if (nameSt.Count > CITY_HISTORY_COUNT)
            nameSt.RemoveAt(0);
        PlayerPrefsX.SetStringArray("cityHistory_name", nameSt.ToArray());
        pNameSt.Add(ci.pName);
        if (pNameSt.Count > CITY_HISTORY_COUNT)
            pNameSt.RemoveAt(0);
        PlayerPrefsX.SetStringArray("cityHistory_pName", pNameSt.ToArray());
        pCodeSt.Add(ci.pCode);
        if (pCodeSt.Count > CITY_HISTORY_COUNT)
            pCodeSt.RemoveAt(0);
        PlayerPrefsX.SetIntArray("cityHistory_pCode", pCodeSt.ToArray());
        fullNameSt.Add(ci.fullName);
        if (fullNameSt.Count > CITY_HISTORY_COUNT)
            fullNameSt.RemoveAt(0);
        PlayerPrefsX.SetStringArray("cityHistory_fullName", fullNameSt.ToArray());
        lngSt.Add(ci.lng);
        if (lngSt.Count > CITY_HISTORY_COUNT)
            lngSt.RemoveAt(0);
        PlayerPrefsX.SetFloatArray("cityHistory_lng", lngSt.ToArray());
        latSt.Add(ci.lat);
        if (latSt.Count > CITY_HISTORY_COUNT)
            latSt.RemoveAt(0);
        PlayerPrefsX.SetFloatArray("cityHistory_lat", latSt.ToArray());
        timeZoneIDSt.Add(ci.timeZone.Id);
        if (timeZoneIDSt.Count > CITY_HISTORY_COUNT)
            timeZoneIDSt.RemoveAt(0);
        PlayerPrefsX.SetStringArray("cityHistory_timeZoneID", timeZoneIDSt.ToArray());
        countryIDSt.Add(ci.countryID);
        if (countryIDSt.Count > CITY_HISTORY_COUNT)
            countryIDSt.RemoveAt(0);
        PlayerPrefsX.SetIntArray("cityHistory_countryID", countryIDSt.ToArray());
        statesIDSt.Add(ci.statesID);
        if (statesIDSt.Count > CITY_HISTORY_COUNT)
            statesIDSt.RemoveAt(0);
        PlayerPrefsX.SetIntArray("cityHistory_statesID", statesIDSt.ToArray());
        countryCodeSt.Add(ci.countryCode);
        if (countryCodeSt.Count > CITY_HISTORY_COUNT)
            countryCodeSt.RemoveAt(0);
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

        for (int i = idA.Length - 1; i > -1; i--)
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
