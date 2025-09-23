using SunCalcNet;
using SunCalcNet.Model;
using SunCalcNet.Tests;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using I2.Loc;
using static SelectCityController;
using System.Reflection;
using ZXing;
//using CoordinateSharp;

[CLSCompliant(false)]
public class CalendarView : MonoBehaviour
{
    public Text sunriseText;
    public Text solarNoonText;
    public Text sunsetText;
    public Text westEraText;
    public Text buddhistEraText;
    public CalendarController controllerView;
    public ToggleGroup calToggleGroup;
    public Toggle mmCalToggle;
    public Toggle realCalToggle;
    public Toggle farmerCalToggle;
    public Dropdown calMenu;    //缅历、农历、星宿历

    public Button todayBtn;
    public Button selectCityBtn;
    public Button currPosBtn;
    public SelectCityView selectCityView;
    public GameObject selectCityPart;
    public GameObject currCityPart;
    public Text selectCityText;
    public Text currCityText;
    public Text currCityLatText;
    public Text currCityLngText;
    public Text selectCityLatText;
    public Text selectCityLngText;
    public Text selectCityTimeoffsetText;
    public Text selectCityTimezoneText;
    public Text selectCityDstText;
    public CityInfo nowCity = null;
    public Text currCityTimeoffsetText;
    public Text currCityTimezoneText;
    public Text currCityDstText;
    //选择其他国家后显示的当前手机设备的时区时间
    public Text currPhoneTZsunriseText;
    public Text currPhoneTZsolarNoonText;
    public Text currPhoneTZsunsetText;

    // Start is called before the first frame update
    void Awake()
    {
        currCityLatText.text = "请打开GPS定位";
        currCityLngText.text = "请打开GPS定位";
        selectCityLatText.text = "请打开GPS定位";
        selectCityLngText.text = "请打开GPS定位";
        sunriseText.text = "请打开GPS定位";
        solarNoonText.text = "请打开GPS定位";
        sunsetText.text = "请打开GPS定位";

        currCityText.text = "请打开GPS定位";
        //timeZoneText.text = "请打开GPS定位";
        //currCityText.text = "请打开GPS定位";

        nowCity = new CityInfo();
        nowCity.lat = 0;
        nowCity.lng = 0;
        nowCity.timeZone = TimeZoneInfo.FindSystemTimeZoneById("Greenwich Standard Time");
    }

    void Start()
    {
        setCalMenuLanguage();
        //SunCalcTests sunCalcTests = new SunCalcTests();
        //sunCalcTests.Get_Sun_Phases_Returns_Sun_Phases_For_The_Given_Date_And_Location();

        //GetSunTime(DateTime.Today);
        //float lat = 24;
        //float lng = 103;
        //(lat, lng) = CalendarManager.Instance().GetLocation();
        //latText.text = lat.ToString();
        //lngText.text = lng.ToString();

        //CalendarManager.Instance().StopLocation();
        //temp
        //CalendarManager.Instance().StartLocation();
        //放awake里会出现两个toggle都是on的bug
        int isMMCal = SettingManager.Instance().GetCalType();
        calMenu.SetValueWithoutNotify(isMMCal - 1);
        if (isMMCal == 1)
        {
            mmCalToggle.isOn = true;
            realCalToggle.isOn = false;
            farmerCalToggle.isOn = false;
        }
        else if (isMMCal == 2)
        {
            realCalToggle.isOn = true;
            mmCalToggle.isOn = false;
            farmerCalToggle.isOn = false;
        }
        else if (isMMCal == 3)
        {
            farmerCalToggle.isOn = true;
            realCalToggle.isOn = false;
            mmCalToggle.isOn = false;
        }
        mmCalToggle.onValueChanged.AddListener(OnToggleValueChanged);
        calMenu.onValueChanged.AddListener(OnChangeCal);
        realCalToggle.onValueChanged.AddListener(OnToggleValueChanged);
        farmerCalToggle.onValueChanged.AddListener(OnToggleValueChanged);
        todayBtn.onClick.AddListener(OnClickToday);
        selectCityBtn.onClick.AddListener(OnClickSelectCity);
        currPosBtn.onClick.AddListener(OnClickCurrPosBtn);
        SetEra(DateTime.Today);
        SetIsCurrCity(true);
    }
    public void setCalMenuLanguage()
    {
        calMenu.options[0].text = LocalizationManager.GetTranslation("calendar_BurmeseCalendar");
        calMenu.options[1].text = LocalizationManager.GetTranslation("calendar_StarCalendar");
        calMenu.options[2].text = LocalizationManager.GetTranslation("calendar_ChineseCalendar");
    }
    public void SetIsCurrCity(bool isCurrCity)
    {
        if (isCurrCity)
        {
            selectCityPart.SetActive(false);
            currCityPart.SetActive(true);
        }
        else
        {
            selectCityPart.SetActive(true);
            currCityPart.SetActive(false);
        }
    }
    public void SetSelectCity(CityInfo cityInfo)
    {
        SetIsCurrCity(false);
        SetSelectLocationTime(cityInfo);
        CalendarManager.Instance().AddCityHistorySave(cityInfo);
    }
    void OnClickSelectCity()
    {
        selectCityView.SetThisOn();

    }
    void OnClickCurrPosBtn()
    {
        SetCurrLocationTime();
        SetIsCurrCity(true);
    }
    void OnClickToday()
    {
        controllerView.ClickToday();
        //test
        //string temp = CommonTool.GetClipboard();
        //Debug.LogError("+" + temp);
        //UITool.ShowToastMessage(this, temp, 35);
    }
    int toggleFlag = 0;
    void OnChangeCal(int index)
    {
        SettingManager.Instance().SetCalType(index + 1);
        controllerView.Start();
    }
    void OnToggleValueChanged(bool value)
    {
        //Debug.LogError(value);
        if (toggleFlag == 0)
        {
            if (mmCalToggle.isOn)
                SettingManager.Instance().SetCalType(1);
            else if (realCalToggle.isOn)
                SettingManager.Instance().SetCalType(2);
            else if (farmerCalToggle.isOn)
                SettingManager.Instance().SetCalType(3);
            controllerView.Start();
        }
        //3个toggle只会有2个toggle产生变化
        ++toggleFlag;
        if (toggleFlag == 2)
            toggleFlag = 0;
    }

    public void SetEra(DateTime time)
    {
        westEraText.text = time.Year + "年" + time.Month + "月" + time.Day + "日";
        MyanmarDate myanmarDate = MyanmarDateConverter.convert(time.Year, time.Month, time.Day);
        buddhistEraText.text = myanmarDate.getBuddhistEraInt() + "年";
        //Debug.LogError(myanmarDate.getBuddhistEraInt());
    }
    //todo 整理到CalendarManager中
    public void GetSunTime(DateTime time, CityInfo cityInfo)
    {
        GetSunTime(time, cityInfo.lat, cityInfo.lng, cityInfo.timeZone);
    }
    //夏令时时间是凌晨两点开始算的，所以要按3点时间算
    public DateTime GetDSTDateTime(DateTime time)
    {
        return new DateTime(time.Year, time.Month, time.Day, 3, 1, 0);
    }
    //todo 此处可能有有隐患出bug全部设置,后面考虑统一这两个
    public void SetAllTimeZoneDST(DateTime time, CityInfo cityInfo)
    {
        DateTime tzTime = GetDSTDateTime(time);
        currCityTimeoffsetText.text = cityInfo.timeZone.GetUtcOffset(tzTime).ToString();
        selectCityTimeoffsetText.text = cityInfo.timeZone.GetUtcOffset(tzTime).ToString();
        if (cityInfo.timeZone.IsDaylightSavingTime(tzTime))
        {
            currCityDstText.gameObject.SetActive(true);
            selectCityDstText.gameObject.SetActive(true);
        }
        else
        {
            currCityDstText.gameObject.SetActive(false);
            selectCityDstText.gameObject.SetActive(false);
        }
        //currCityDstText.text = cityInfo.timeZone.IsDaylightSavingTime(tzTime) ? "Y" : "N";
        //selectCityDstText.text = cityInfo.timeZone.IsDaylightSavingTime(tzTime) ? "Y" : "N";
    }
    public void GetSunTime(DateTime time, float lat, float lng, TimeZoneInfo tz)
    {
        SunPhase solarNoon = new SunPhase(SunPhaseName.SolarNoon, time);
        SunPhase sunset = new SunPhase(SunPhaseName.Sunset, time);//日落
        SunPhase sunrise = new SunPhase(SunPhaseName.Sunrise, time);//日出
        SunPhase dawn = new SunPhase(SunPhaseName.Dawn, time);//曙光升起
        SunPhase nauticalDawn = new SunPhase(SunPhaseName.NauticalDawn, time);//航海曙光
        //航海曙光+日出-曙光升起
        //初一十五

        //var date = new DateTime(2013, 3, 5, 0, 0, 0, DateTimeKind.Utc);
        //lat:是Latitude的简写,表示纬度。lng:是Longtitude的简写,表示经度
        //wikipali办公室当前经纬度

        //(lat, lng) = CalendarManager.Instance().GetLocation();

        var height = 0;// 2000;
        //时区时差，本地时间与UTC时间的时差
        //TimeSpan ts = TimeZoneInfo.Local.GetUtcOffset(time);
        //Act
        var sunPhases = SunCalc.GetSunPhases(time, lat, lng, height, 0).ToList();// ts.Hours).ToList();
        TimeSpan ts = tz.GetUtcOffset(time);
        //TimeZoneInfo targetTimeZone1 = TimeZoneInfo.FindSystemTimeZoneById("UTC");
        //TimeZoneInfo targetTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Sri Lanka Standard Time");
        TimeSpan BaseUtcOffset = new TimeSpan(ts.Hours, ts.Minutes, ts.Seconds);
        //TimeSpan BaseUtcOffsetSriLanka = new TimeSpan(5,30,0);
        //TimeSpan BaseUtcOffsetChina = new TimeSpan(8,0,0);
        //DateTime targetTime = TimeZoneInfo.ConvertTime(currentTime, targetTimeZone);
        TimeSpan sp = new TimeSpan();// TimeZoneInfo.Local.GetUtcOffset(time);
        sp = -BaseUtcOffset;// ts;// sp -  BaseUtcOffsetSriLanka;// targetTimeZone.GetUtcOffset(time);
        var sunPhaseValueSolarNoon = sunPhases.First(x => x.Name.Value == solarNoon.Name.Value);
        var sunPhaseValueSunrise = sunPhases.First(x => x.Name.Value == sunrise.Name.Value);
        var sunPhaseValueDawn = sunPhases.First(x => x.Name.Value == dawn.Name.Value);
        var sunPhaseValueNauticalDawn = sunPhases.First(x => x.Name.Value == nauticalDawn.Name.Value);
        var sunPhaseValueSunSet = sunPhases.First(x => x.Name.Value == sunset.Name.Value);
        //航海曙光+日出-曙光升起
        TimeSpan tsd = new TimeSpan(sunPhaseValueSunrise.PhaseTime.Ticks - sunPhaseValueDawn.PhaseTime.Ticks);
        DateTime lightTime = sunPhaseValueNauticalDawn.PhaseTime + tsd;// sunPhaseValueSunrise.PhaseTime - sunPhaseValueDawn.PhaseTime;

        //string sunPhaseTimeSolarNoon = sunPhaseValueSolarNoon.PhaseTime.ToString("HH:mm:ss");
        //string sunPhaseTimeSolarNoon = TimeZoneInfo.ConvertTime(sunPhaseValueSolarNoon.PhaseTime, targetTimeZone1).ToString("HH:mm:ss");
        string sunPhaseTimeSolarNoon = (sunPhaseValueSolarNoon.PhaseTime - sp).ToString("HH:mm:ss");
        string sunPhaseTimeSunrise = (lightTime - sp).ToString("HH:mm:ss");
        string sunPhaseTimeSunSet = (sunPhaseValueSunSet.PhaseTime - sp).ToString("HH:mm:ss");
        sunriseText.text = sunPhaseTimeSunrise;
        solarNoonText.text = sunPhaseTimeSolarNoon;
        sunsetText.text = sunPhaseTimeSunSet;
        //todo时区差
        //时区时差，本地时间与UTC时间的时差
        TimeSpan sp_curr = -TimeZoneInfo.Local.GetUtcOffset(time);
        string currPhoneTimeSolarNoon = (sunPhaseValueSolarNoon.PhaseTime - sp_curr).ToString("HH:mm:ss");
        string currPhoneTimeSunrise = (lightTime - sp_curr).ToString("HH:mm:ss");
        string currPhoneTimeSunSet = (sunPhaseValueSunSet.PhaseTime - sp_curr).ToString("HH:mm:ss");
        currPhoneTZsunriseText.text = currPhoneTimeSunrise;
        currPhoneTZsolarNoonText.text = currPhoneTimeSolarNoon;
        currPhoneTZsunsetText.text = currPhoneTimeSunSet;

        //Debug.LogError("本地时区" + ts_curr);
    }
    //public void GetSunTime(DateTime time)
    //{
    //    SunPhase solarNoon = new SunPhase(SunPhaseName.SolarNoon, time);
    //    SunPhase sunrise = new SunPhase(SunPhaseName.Sunrise, time);


    //    //var date = new DateTime(2013, 3, 5, 0, 0, 0, DateTimeKind.Utc);
    //    //lat:是Latitude的简写,表示纬度。lng:是Longtitude的简写,表示经度
    //    //wikipali办公室当前经纬度
    //    var lat = 24;
    //    var lng = 103;
    //    var height = 2000;

    //    TimeSpan ts = TimeZoneInfo.Local.GetUtcOffset(time);
    //    Celestial cel = Celestial.CalculateCelestialTimes(lat, lng, time, ts.Hours);

    //    string sunPhaseTimeSunrise = cel.SunRise?.ToString("HH:mm:ss");
    //    string sunPhaseTimeSolarNoon = cel.SolarNoon?.ToString("HH:mm:ss");
    //    sunriseText.text = sunPhaseTimeSunrise;
    //    solarNoonText.text = sunPhaseTimeSolarNoon;
    //}
    // Update is called once per frame
    bool locationed = false;

    void Update()
    {
        if (!locationed && CalendarManager.Instance().isLocationed())
        {
            SetCurrLocationTime();
            locationed = true;
        }

    }
    void SetCurrLocationTime()
    {
        float lat = 24;
        float lng = 103;
        (lat, lng) = CalendarManager.Instance().GetLocation();
        CityInfo cityInfo = SelectCityController.Instance().GetCurrCityInfo(lat, lng);
        nowCity = new CityInfo();
        nowCity.lat = lat;
        nowCity.lng = lng;
        //临时：因为目前数据库时差有问题，所以当前定位时差用系统时差
        //nowCity.timeZoneOffset = cityInfo.timeZoneOffset;

        //TimeSpan ts = TimeZoneInfo.Local.GetUtcOffset(DateTime.UtcNow);
        //TimeSpan BaseUtcOffset = new TimeSpan(ts.Hours, ts.Minutes, ts.Seconds);
        //TimeSpan sp = BaseUtcOffset;//-BaseUtcOffset;
        nowCity.timeZone = TimeZoneInfo.Local;
        cityInfo.timeZone = TimeZoneInfo.Local;

        currCityLatText.text = lat.ToString("#0.0");// lat.ToString();
        currCityLngText.text = lng.ToString("#0.0");
        GetSunTime(new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day, 0, 1, 0), lat, lng, cityInfo.timeZone);
        //todo
        CalendarManager.Instance().StopLocation();
        controllerView.Start();
        SetIsCurrCity(true);
        DateTime tzTime = GetDSTDateTime(DateTime.Today);
        currCityTimeoffsetText.text = cityInfo.timeZone.GetUtcOffset(tzTime).ToString();
        if (cityInfo.timeZone.IsDaylightSavingTime(tzTime))
            currCityDstText.gameObject.SetActive(true);
        else
            currCityDstText.gameObject.SetActive(false);
        currCityTimezoneText.text = cityInfo.timeZone.DisplayName;
        if (string.IsNullOrEmpty(cityInfo.fullName))
            currCityText.text = cityInfo.name;
        else
            currCityText.text = cityInfo.fullName;

    }
    void SetSelectLocationTime(CityInfo sCityInfo)
    {
        nowCity = sCityInfo;

        float lat = sCityInfo.lat;
        float lng = sCityInfo.lng;
        selectCityLatText.text = lat.ToString("#0.0");// lat.ToString();
        selectCityLngText.text = lng.ToString("#0.0");
        GetSunTime(GetDSTDateTime(DateTime.Today), sCityInfo);
        //todo
        CalendarManager.Instance().StopLocation();
        controllerView.Start();
        SetIsCurrCity(false);
        //currCityTimezoneText.text = sCityInfo.timeZone.GetUtcOffset(DateTime.Today).ToString();
        //currCityDstText.text = sCityInfo.timeZone.IsDaylightSavingTime(DateTime.Today) ? "Y" : "N";
        DateTime tzTime = GetDSTDateTime(DateTime.Today);
        selectCityTimeoffsetText.text = sCityInfo.timeZone.GetUtcOffset(tzTime).ToString();
        if (sCityInfo.timeZone.IsDaylightSavingTime(tzTime))
            selectCityDstText.gameObject.SetActive(true);
        else
            selectCityDstText.gameObject.SetActive(false);
        selectCityTimezoneText.text = sCityInfo.timeZone.DisplayName;
        if (string.IsNullOrEmpty(sCityInfo.fullName))
            selectCityText.text = sCityInfo.name;
        else
            selectCityText.text = sCityInfo.fullName;
    }
}
