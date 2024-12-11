using SunCalcNet;
using SunCalcNet.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CalendarController : MonoBehaviour
{
    public GameObject _calendarPanel;
    public Text _yearNumText;
    public Text _monthNumText;

    public CalendarDateItem _item;

    public List<CalendarDateItem> _dateItems = new List<CalendarDateItem>();
    const int _totalDateNum = 42;

    private DateTime _dateTime;
    public static CalendarController _calendarInstance;
    const int SCALE_POS = 5;
    const float SCALE_POS_Y = 1.63f;
    public CalendarView cView;

    public enum MoonType
    {
        MoonOther = 0,//其他月亮
        Moon0 = 1,//黑月---新月
        Moon1 = 2,//左黑右白---上弦月
        Moon2 = 3,//白月---满月
        Moon3 = 4,//左白右黑---下弦月
    }
    #region 月亮
    //https://github.com/ariyamaggika/suncalc
    //MoonCalc.GetMoonIllumination(/*Date*/ timeAndDate)
    //fraction: illuminated fraction of the moon; varies from 0.0 (new moon) to 1.0 (full moon)
    //phase: moon phase; varies from 0.0 to 1.0, described below
    //angle: midpoint angle in radians of the illuminated limb of the moon reckoned eastward from the north point of the disk; the moon is waxing if the angle is negative, and waning if positive
    //Moon phase value should be interpreted like this:
    //Phase Name
    //0 New Moon

    //Waxing Crescent
    //0.25 	First Quarter

    //Waxing Gibbous
    //0.5 	Full Moon

    //Waning Gibbous
    //0.75 	Last Quarter

    //Waning Crescent
    //获取月亮类型
    const float MOON_OFFST = 0.04f;
    //如实历月相
    public static MoonType GetMoonType(DateTime date)
    {

        DateTime time0 = new DateTime(date.Year, date.Month, date.Day, 0, 0, 1);
        DateTime time24 = new DateTime(date.Year, date.Month, date.Day, 23, 59, 59);
        MoonIllumination moonIllum0 = MoonCalc.GetMoonIllumination(time0);
        MoonIllumination moonIllum24 = MoonCalc.GetMoonIllumination(time24);
        //范围判断
        if ((moonIllum0.Phase >= 0.9f) && (moonIllum24.Phase <= 0.1f))//New Moon
        {
            return MoonType.Moon0;
        }
        else if ((moonIllum0.Phase <= 0.25f) && (moonIllum24.Phase >= 0.25f))//First Quarter
        {
            return MoonType.Moon1;
        }
        else if ((moonIllum0.Phase <= 0.5f) && (moonIllum24.Phase >= 0.5f))//Full Moon
        {
            return MoonType.Moon2;
        }
        else if ((moonIllum0.Phase <= 0.75f) && (moonIllum24.Phase >= 0.75f))//Last Quarter
        {
            return MoonType.Moon3;
        }
        return MoonType.MoonOther;

    }
    //获取月相准确时间
    //4分一天，4*6=24,
    //确定月相时间在哪6小时范围内
    //然后遍历6*60分钟，遍历360次精确到分钟
    //todo :可以用算法：无限循环2分法
    public static DateTime GetPreciseMoonTime(DateTime date, MoonType mt)
    {
        float minP = 0;
        float maxP = 0;
        switch (mt)
        {
            case MoonType.Moon0:
                minP = 0.1f;
                maxP = 0.9f;
                break;
            case MoonType.Moon1:
                minP = 0.25f;
                maxP = 0.25f;
                break;
            case MoonType.Moon2:
                minP = 0.5f;
                maxP = 0.5f;
                break;
            case MoonType.Moon3:
                minP = 0.75f;
                maxP = 0.75f;
                break;
        }
        DateTime time0 = new DateTime(date.Year, date.Month, date.Day, 0, 0, 1);
        DateTime time6 = new DateTime(date.Year, date.Month, date.Day, 6, 0, 0);
        DateTime time12 = new DateTime(date.Year, date.Month, date.Day, 12, 0, 0);
        DateTime time18 = new DateTime(date.Year, date.Month, date.Day, 18, 0, 0);
        DateTime time24 = new DateTime(date.Year, date.Month, date.Day, 23, 59, 59);
        MoonIllumination moonIllum0 = MoonCalc.GetMoonIllumination(time0);
        MoonIllumination moonIllum6 = MoonCalc.GetMoonIllumination(time6);
        MoonIllumination moonIllum12 = MoonCalc.GetMoonIllumination(time12);
        MoonIllumination moonIllum18 = MoonCalc.GetMoonIllumination(time18);
        MoonIllumination moonIllum24 = MoonCalc.GetMoonIllumination(time24);
        DateTime resMin = time0;
        DateTime resMax = time0;
        if (mt != MoonType.Moon0)
        {
            if ((moonIllum0.Phase <= minP) && (moonIllum6.Phase >= maxP))
            {
                resMin = time0;
                resMax = time6;
            }
            else if ((moonIllum6.Phase <= minP) && (moonIllum12.Phase >= maxP))
            {
                resMin = time6;
                resMax = time12;
            }
            else if ((moonIllum12.Phase <= minP) && (moonIllum18.Phase >= maxP))
            {
                resMin = time12;
                resMax = time18;
            }
            else if ((moonIllum18.Phase <= minP) && (moonIllum24.Phase >= maxP))
            {
                resMin = time18;
                resMax = time24;
            }
            DateTime resStart = resMin;
            DateTime resStartAdd = resMin;// +h1;
            TimeSpan m1 = new TimeSpan(0, 1, 0);
            for (int i = 0; i < 360; i++)
            {
                resStart = resStart + m1;
                resStartAdd = resStart + m1;
                MoonIllumination moonIllumStart = MoonCalc.GetMoonIllumination(resStart);
                MoonIllumination moonIllumAdd = MoonCalc.GetMoonIllumination(resStartAdd);
                if ((moonIllumStart.Phase <= minP) && (moonIllumAdd.Phase >= maxP))
                {
                    return resStart;
                }
            }
        }
        else
        {
            if ((moonIllum0.Phase >= maxP) && (moonIllum6.Phase <= minP))
            {
                resMin = time0;
                resMax = time6;
            }
            else if ((moonIllum6.Phase >= maxP) && (moonIllum12.Phase <= minP))
            {
                resMin = time6;
                resMax = time12;
            }
            else if ((moonIllum12.Phase >= maxP) && (moonIllum18.Phase <= minP))
            {
                resMin = time12;
                resMax = time18;
            }
            else if ((moonIllum18.Phase >= maxP) && (moonIllum24.Phase <= minP))
            {
                resMin = time18;
                resMax = time24;
            }
            DateTime resStart = resMin;
            DateTime resStartAdd = resMin;// +h1;
            TimeSpan m1 = new TimeSpan(0, 1, 0);
            for (int i = 0; i < 360; i++)
            {
                resStart = resStart + m1;
                resStartAdd = resStart + m1;
                MoonIllumination moonIllumStart = MoonCalc.GetMoonIllumination(resStart);
                MoonIllumination moonIllumAdd = MoonCalc.GetMoonIllumination(resStartAdd);
                if ((moonIllumStart.Phase >= maxP) && (moonIllumAdd.Phase <= minP))
                {
                    return resStart;
                }
            }
        }
        return date;
    }

    //过去一个月后前八天是否有新月或满月
    void GetLastMonthMyanmarMoon(DateTime date)
    {
        TimeSpan d1 = new TimeSpan(1, 0, 0, 0);
        DateTime ldate = new DateTime(date.Year, date.Month, date.Day, 0, 0, 0, 0);

        for (int i = 0; i < 8; i++)
        {
            ldate -= d1;
            MyanmarDate myanmarDate = MyanmarDateConverter.convert(ldate.Year, ldate.Month, ldate.Day);
            int moon = myanmarDate.moonPhase;
            switch (moon)
            {
                //case 0:
                //    return MoonType.Moon1;
                //    break;
                case 1:
                    TimeSpan d8 = new TimeSpan(8, 0, 0, 0);
                    //date += 7;
                    nextMoon3date = ldate + d8;
                    return;
                    break;
                //case 2:
                //    return MoonType.Moon3;
                //    break;
                case 3:
                    TimeSpan d82 = new TimeSpan(8, 0, 0, 0);
                    //date += 7;
                    nextMoon1date = ldate + d82;
                    return;
                    break;
            }

        }


    }
    DateTime nextMoon1date;
    DateTime nextMoon3date;
    MoonType GetMoonTypeMyanmar(DateTime date)
    {
        MyanmarDate myanmarDate = MyanmarDateConverter.convert(date.Year, date.Month, date.Day);
        //Debug.LogError(myanmarDate.getFortnightDay());
        //mp :moon phase [0=waxing, 1=full moon, 2=waning, 3=new moon],
        int moon = myanmarDate.moonPhase;
        if (nextMoon1date != null && date.Year == nextMoon1date.Year && date.Month == nextMoon1date.Month && date.Day == nextMoon1date.Day)
            return MoonType.Moon1;
        if (nextMoon3date != null && date.Year == nextMoon3date.Year && date.Month == nextMoon3date.Month && date.Day == nextMoon3date.Day)
            return MoonType.Moon3;
        switch (moon)
        {
            //case 0:
            //    return MoonType.Moon1;
            //    break;
            case 1:
                TimeSpan d8 = new TimeSpan(8, 0, 0, 0);
                //date += 7;
                nextMoon3date = date + d8;
                return MoonType.Moon2;
                break;
            //case 2:
            //    return MoonType.Moon3;
            //    break;
            case 3:
                TimeSpan d82 = new TimeSpan(8, 0, 0, 0);
                //date += 7;
                nextMoon1date = date + d82;
                return MoonType.Moon0;
                break;
        }
        return MoonType.MoonOther;
    }
    //农历月相
    MoonType GetMoonTypeFarmer(DateTime date)
    {
        Calendar calender = new Calendar(date);
        string day = calender.ChineseDayString;
        int chineseFarmerMonth = calender.ChineseMonth;
        int chineseFarmerYear = calender.ChineseYear;
        bool isLeapMon = calender.IsChineseLeapMonth;
        //MoonOther = 0,//其他月亮
        //Moon0 = 1,//黑月---新月
        //Moon1 = 2,//左黑右白---上弦月
        //Moon2 = 3,//白月---满月
        //Moon3 = 4,//左白右黑---下弦月

        if (day == "初七")
        {
            return MoonType.Moon1;
        }
        else if (day == "十五")
        {
            return MoonType.Moon2;
        }
        else if (day == "廿二")
        {
            return MoonType.Moon3;
        }
        else if (day == calender.LastDayOfChineseMonth(chineseFarmerYear, chineseFarmerMonth, isLeapMon))
        {
            return MoonType.Moon0;
        }
        //?月的最后一天是新月
        return MoonType.MoonOther;
    }
    #endregion
    void DestoryItemGOList()
    {
        int c = _dateItems.Count;
        for (int i = 0; i < c; i++)
        {
            Destroy(_dateItems[i].gameObject);
        }
        _dateItems.Clear();
    }
    public void Start()
    {
        _calendarInstance = this;
        Vector3 startPos = _item.transform.localPosition;
        DestoryItemGOList();
        //_dateItems.Add(_item);

        for (int i = 0; i < _totalDateNum; i++)
        {
            GameObject item = GameObject.Instantiate(_item.gameObject) as GameObject;
            item.SetActive(true);
            CalendarDateItem dItem = item.GetComponent<CalendarDateItem>();
            item.name = "Item" + (i + 1).ToString();
            item.transform.SetParent(_item.transform.parent);
            item.transform.localScale = Vector3.one;
            item.transform.localRotation = Quaternion.identity;
            item.transform.localPosition = new Vector3((i % 7) * 31 * SCALE_POS + startPos.x, startPos.y - (i / 7) * 25 * SCALE_POS * SCALE_POS_Y, startPos.z);

            _dateItems.Add(dItem);
        }
        _dateTime = DateTime.Now;

        CreateCalendar();
        InitToday();
        //_calendarPanel.SetActive(false);
    }
    Dictionary<DateTime, string> trueCalenderYearHolidays;
    void CreateCalendar()
    {
        DateTime firstDay = _dateTime.AddDays(-(_dateTime.Day - 1));
        //todo:待优化，目前每下个月计算一次如实历节日
        trueCalenderYearHolidays = HolidayCalculator.preYearTrueHoliday(firstDay.Year);
        GetLastMonthMyanmarMoon(firstDay);
        int index = GetDays(firstDay.DayOfWeek);

        int date = 0;
        //1:缅历  2:如实历  3:农历
        int calType = SettingManager.Instance().GetCalType();
        for (int i = 0; i < _totalDateNum; i++)
        {
            Text label = _dateItems[i].GetComponentInChildren<Text>();
            _dateItems[i].gameObject.SetActive(false);

            if (i >= index)
            {
                DateTime thatDay = firstDay.AddDays(date);
                if (thatDay.Month == firstDay.Month)
                {
                    _dateItems[i].gameObject.SetActive(true);
                    _dateItems[i].Init(thatDay, trueCalenderYearHolidays, cView.nowCity);
                    //缅历不需要定位
                    if (CalendarManager.Instance().isLocationed())
                    {
                        MoonType moon = MoonType.MoonOther;
                        string moonTime = "";
                        if (calType == 1)
                            moon = GetMoonTypeMyanmar(thatDay);
                        else if (calType == 2)
                        {
                            moon = GetMoonType(thatDay);
                            if (moon != MoonType.MoonOther)
                            {
                                //Debug.LogError(moon);
                                //Debug.LogError(GetPreciseMoonTime(thatDay, moon));
                                moonTime = GetPreciseMoonTime(thatDay, moon).ToString("HH:mm");
                            }
                        }
                        else if (calType == 3)
                            moon = GetMoonTypeFarmer(thatDay);

                        _dateItems[i].SetMoon(moon, moonTime);
                        _dateItems[i].SetSolarNoonTextActive(true);
                    }
                    else
                    {
                        MoonType moon = MoonType.MoonOther;
                        if (calType == 1)
                        {
                            moon = GetMoonTypeMyanmar(thatDay);
                            _dateItems[i].SetMoon(moon, "");
                        }
                        else if (calType == 2)
                            _dateItems[i].SetMoon(MoonType.MoonOther, "");
                        else if (calType == 3)
                        {
                            moon = GetMoonTypeFarmer(thatDay);
                            _dateItems[i].SetMoon(moon, "");
                        }

                        _dateItems[i].SetSolarNoonTextActive(false);
                    }
                    label.text = (date + 1).ToString();
                    date++;
                }
            }
        }
        _yearNumText.text = _dateTime.Year.ToString();
        _monthNumText.text = _dateTime.Month.ToString();
    }

    int GetDays(DayOfWeek day)
    {
        switch (day)
        {
            case DayOfWeek.Monday: return 1;
            case DayOfWeek.Tuesday: return 2;
            case DayOfWeek.Wednesday: return 3;
            case DayOfWeek.Thursday: return 4;
            case DayOfWeek.Friday: return 5;
            case DayOfWeek.Saturday: return 6;
            case DayOfWeek.Sunday: return 0;
        }

        return 0;
    }
    public void YearPrev()
    {
        _dateTime = _dateTime.AddYears(-1);
        CreateCalendar();
    }

    public void YearNext()
    {
        _dateTime = _dateTime.AddYears(1);
        CreateCalendar();
    }

    public void MonthPrev()
    {
        _dateTime = _dateTime.AddMonths(-1);
        CreateCalendar();
    }

    public void MonthNext()
    {
        _dateTime = _dateTime.AddMonths(1);
        CreateCalendar();
    }

    //public void ShowCalendar(Text target)
    //{
    //    _calendarPanel.SetActive(true);
    //    _target = target;
    //    _calendarPanel.transform.position = Input.mousePosition-new Vector3(0,120,0);
    //}

    Text _target;
    public void OnDateItemClick(string day)
    {
        //_target.text = _yearNumText.text + "年" + _monthNumText.text + "月" + day + "日";
        //_calendarPanel.SetActive(false);
        DateTime time = new DateTime(int.Parse(_yearNumText.text), int.Parse(_monthNumText.text), int.Parse(day), 0, 1, 0);
        cView.SetEra(time);
        //不能用UTC时间
        if (CalendarManager.Instance().isLocationed())
            cView.GetSunTime(time, cView.nowCity);//,0,0,0, DateTimeKind.Utc));

    }
    public void ClickToday()
    {
        _dateTime = DateTime.Today;
        CreateCalendar();
        int day = DateTime.Today.Day;
        OnDateItemClick(day.ToString());
        DateTime firstDay = _dateTime.AddDays(-(_dateTime.Day - 1));
        int index = GetDays(firstDay.DayOfWeek);
        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(_dateItems[day + index - 1].gameObject);
        //_dateItems[day].GetComponent<Button>().onClick();

    }
    void InitToday()
    {
        int day = DateTime.Today.Day;
        //ColorBlock cb = _dateItems[day].GetComponent<Button>().colors;
        //cb.normalColor = new Color(0.04411763f, 0.6044625f, 1);
        //Debug.LogError("!!!!!!!!!!!!");
        //_dateItems[day].GetComponent<Button>().colors = cb;//.normalColor = Color.blue;
        ClickToday();
    }
}
