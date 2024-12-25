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
        DateTime newDate = new DateTime(time.Year, time.Month, time.Day, 1, 1, 0);
        SunPhase solarNoon = new SunPhase(SunPhaseName.SolarNoon, newDate);
        //时区时差
        TimeSpan ts = cityInfo.timeZoneOffset;
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

}
