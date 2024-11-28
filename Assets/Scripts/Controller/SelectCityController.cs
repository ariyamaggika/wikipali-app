using Imdork.SQLite;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using static ArticleController;
using static ArticleManager;
using static DictManager;
using static SelectCityController;
using static SettingManager;

public class SelectCityController
{
    //懒汉式单例类.在第一次调用的时候实例化自己 
    private SelectCityController() { }
    private static SelectCityController controller = null;
    //静态工厂方法 
    public static SelectCityController Instance()
    {
        if (controller == null)
        {
            controller = new SelectCityController();
        }
        return controller;
    }
    public DBManager dbManager = DBManager.Instance();
    //策略，First全部读取储存，second，third饿汉式， 点到First时一起读取。
    //读取后储存在内存，下次点选时先判断有无读取过
    public class FirstCityInfo : CityInfo
    {
        public Dictionary<int, SecondCityInfo> secondCityInfoList = new Dictionary<int, SecondCityInfo>();
        //public CityInfo cityInfo = new CityInfo();
    }
    public class SecondCityInfo : CityInfo
    {
        public Dictionary<int, ThirdCityInfo> thirdCityInfoList = new Dictionary<int, ThirdCityInfo>();
        // public CityInfo cityInfo = new CityInfo();
    }

    public class ThirdCityInfo : CityInfo
    {
        //public CityInfo cityInfo = new CityInfo();
    }
    public class CityInfo
    {
        public int id;
        public string name;
        public string level;
        public string pName;
        public int pCode;
        public string fullName;
        public float lng;
        public float lat;
        public TimeSpan timeZoneOffset;   //时区
        public Dictionary<Language, string> transName;  //翻译后的名字
        //city->state->country
        public int countryID;           //国外二级城市的父城市ID
        public int statesID;           //国外三级城市的父城市ID
    }
    Dictionary<int, FirstCityInfo> domesticFirstCityInfos = new Dictionary<int, FirstCityInfo>();//国内
    Dictionary<int, FirstCityInfo> internationalFirstCityInfos = new Dictionary<int, FirstCityInfo>();//国外<id，信息>
                                                                                                      //List<CityInfo> allCityInfos = new List<CityInfo>();//所有城市信息？
    #region 查询国内城市信息
    public Dictionary<int, FirstCityInfo> GetAllDomesticFirstCityInfos()
    {
        if (domesticFirstCityInfos.Count == 0)
        {
            GetAllDomesticFirstCity();
        }
        return domesticFirstCityInfos;
    }
    //获取所有国内一级城市信息
    void GetAllDomesticFirstCity()
    {
        domesticFirstCityInfos.Clear();
#if DEBUG_PERFORMANCE || UNITY_EDITOR
        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        sw.Start();
#endif

        dbManager.Getdb(db =>
        {
            var reader2 = db.SelectAllDomesticFirstCity();
            //调用SQLite工具  解析对应数据
            Dictionary<string, object>[] pairs2 = SQLiteTools.GetValues(reader2);
            if (pairs2 != null)
            {
                int length = pairs2.Length;
                for (int i = 0; i < length; i++)
                {
                    FirstCityInfo cInfo = new FirstCityInfo();
                    cInfo = new FirstCityInfo();
                    cInfo.id = int.Parse(pairs2[i]["code"].ToString());
                    cInfo.name = pairs2[i]["name"].ToString();
                    cInfo.level = pairs2[i]["level"].ToString();
                    cInfo.pName = pairs2[i]["pname"].ToString();
                    //cInfo.pCode = int.Parse(pairs2[i]["pcode"].ToString());
                    cInfo.fullName = pairs2[i]["fullname"].ToString();
                    cInfo.lng = float.Parse(pairs2[i]["longitude"].ToString());
                    cInfo.lat = float.Parse(pairs2[i]["latitude"].ToString());

                    TimeSpan offset = TimeSpan.FromHours(8);
                    cInfo.timeZoneOffset = offset;

                    cInfo.transName = new Dictionary<Language, string>();

                    domesticFirstCityInfos.Add(cInfo.id, cInfo);
                }
            }
            //matchedWordList = SelectDictLike(db, matchedWordDic,"", inputStr);
        }, DBManager.CityDBurl);

#if DEBUG_PERFORMANCE || UNITY_EDITOR
        sw.Stop();
        Debug.LogError("【性能】查询国内一级城市耗时：" + sw.ElapsedMilliseconds);
#endif
    }
    //获取所有国内所有城市信息
    public void GetAllDomesticSecondAndThirdCity(int countryID)
    {
        if (domesticFirstCityInfos[countryID].secondCityInfoList.Count > 0)
            return;
#if DEBUG_PERFORMANCE || UNITY_EDITOR
        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        sw.Start();
#endif

        dbManager.Getdb(db =>
        {
            //所有二级
            var reader2 = db.SelectAllDomesticSecondCity(countryID, countryID + 9999);
            //调用SQLite工具  解析对应数据
            Dictionary<string, object>[] pairs2 = SQLiteTools.GetValues(reader2);
            if (pairs2 != null)
            {
                int length = pairs2.Length;
                for (int i = 0; i < length; i++)
                {
                    SecondCityInfo cInfo = new SecondCityInfo();
                    cInfo = new SecondCityInfo();
                    cInfo.id = int.Parse(pairs2[i]["code"].ToString());
                    cInfo.name = pairs2[i]["name"].ToString();
                    cInfo.level = pairs2[i]["level"].ToString();
                    cInfo.pName = pairs2[i]["pname"].ToString();
                    cInfo.pCode = int.Parse(pairs2[i]["pcode"].ToString());
                    cInfo.fullName = pairs2[i]["fullname"].ToString();
                    cInfo.lng = float.Parse(pairs2[i]["longitude"].ToString());
                    cInfo.lat = float.Parse(pairs2[i]["latitude"].ToString());

                    TimeSpan offset = TimeSpan.FromHours(8);
                    cInfo.timeZoneOffset = offset;

                    cInfo.transName = new Dictionary<Language, string>();

                    domesticFirstCityInfos[countryID].secondCityInfoList.Add(cInfo.id, cInfo);
                }
            }
            //所有三级
            var reader3 = db.SelectAllDomesticThirdCity(countryID, countryID + 9999);
            //调用SQLite工具  解析对应数据
            Dictionary<string, object>[] pairs3 = SQLiteTools.GetValues(reader3);
            if (pairs3 != null)
            {
                int length = pairs3.Length;
                for (int i = 0; i < length; i++)
                {
                    ThirdCityInfo cInfo = new ThirdCityInfo();
                    cInfo = new ThirdCityInfo();
                    cInfo.id = int.Parse(pairs2[i]["code"].ToString());
                    cInfo.name = pairs2[i]["name"].ToString();
                    cInfo.level = pairs2[i]["level"].ToString();
                    cInfo.pName = pairs2[i]["pname"].ToString();
                    cInfo.pCode = int.Parse(pairs2[i]["pcode"].ToString());
                    cInfo.fullName = pairs2[i]["fullname"].ToString();
                    cInfo.lng = float.Parse(pairs2[i]["longitude"].ToString());
                    cInfo.lat = float.Parse(pairs2[i]["latitude"].ToString());

                    TimeSpan offset = TimeSpan.FromHours(8);
                    cInfo.timeZoneOffset = offset;

                    cInfo.transName = new Dictionary<Language, string>();
                    domesticFirstCityInfos[countryID].secondCityInfoList[cInfo.pCode].thirdCityInfoList.Add(cInfo.id, cInfo);
                }
            }
            //matchedWordList = SelectDictLike(db, matchedWordDic,"", inputStr);
        }, DBManager.CityDBurl);

#if DEBUG_PERFORMANCE || UNITY_EDITOR
        sw.Stop();
        Debug.LogError("【性能】查询国内二三级城市耗时：" + sw.ElapsedMilliseconds);
#endif
    }
    //模糊查询根据名字查国内城市信息
    public List<CityInfo> FuzzySearchDomesticCityInfoByName(string name)
    {
        List<CityInfo> res = new List<CityInfo>();
#if DEBUG_PERFORMANCE || UNITY_EDITOR
        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        sw.Start();
#endif
        dbManager.Getdb(db =>
        {
            var reader2 = db.FuzzySearchDomesticCity(name, CITY_LIMIT_COUNT);
            //调用SQLite工具  解析对应数据
            Dictionary<string, object>[] pairs2 = SQLiteTools.GetValues(reader2);
            if (pairs2 != null)
            {
                int length = pairs2.Length;
                for (int i = 0; i < length; i++)
                {
                    CityInfo cInfo = new CityInfo();
                    cInfo = new CityInfo();
                    cInfo.id = int.Parse(pairs2[i]["code"].ToString());
                    cInfo.name = pairs2[i]["name"].ToString();
                    cInfo.level = pairs2[i]["level"].ToString();
                    cInfo.pName = pairs2[i]["pname"].ToString();
                    cInfo.pCode = int.Parse(pairs2[i]["pcode"].ToString());
                    cInfo.fullName = pairs2[i]["fullname"].ToString();
                    cInfo.lng = float.Parse(pairs2[i]["longitude"].ToString());
                    cInfo.lat = float.Parse(pairs2[i]["latitude"].ToString());

                    TimeSpan offset = TimeSpan.FromHours(8);
                    cInfo.timeZoneOffset = offset;

                    cInfo.transName = new Dictionary<Language, string>();
                }
            }
            //matchedWordList = SelectDictLike(db, matchedWordDic,"", inputStr);
        }, DBManager.CityDBurl);

#if DEBUG_PERFORMANCE || UNITY_EDITOR
        sw.Stop();
        Debug.LogError("【性能】模糊查询国内城市耗时：" + sw.ElapsedMilliseconds);
#endif
        return res;
    }
    #endregion
    #region 查询国外城市信息
    public Dictionary<int, FirstCityInfo> GetAllInternationalFirstCityInfos()
    {
        if (internationalFirstCityInfos.Count == 0)
        {
            GetAllInternationalFirstCity();
        }
        return internationalFirstCityInfos;
    }
    Regex r_timezoneRegex = new Regex(@"""gmtOffset"":(.+?),");
    Regex r_jaRegex = new Regex(@"""ja"":(.+?),");
    Regex r_cnRegex = new Regex(@"""cn"":(.+?),");
    //获取所有国外一级城市信息
    public void GetAllInternationalFirstCity()
    {
        internationalFirstCityInfos.Clear();
#if DEBUG_PERFORMANCE || UNITY_EDITOR
        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        sw.Start();
#endif

        dbManager.Getdb(db =>
        {
            var reader2 = db.SelectAllInternationalFirstCity();
            //调用SQLite工具  解析对应数据
            Dictionary<string, object>[] pairs2 = SQLiteTools.GetValues(reader2);
            if (pairs2 != null)
            {
                int length = pairs2.Length;
                for (int i = 0; i < length; i++)
                {
                    FirstCityInfo cInfo = new FirstCityInfo();
                    cInfo = new FirstCityInfo();
                    cInfo.id = int.Parse(pairs2[i]["id"].ToString());
                    cInfo.name = pairs2[i]["name"].ToString();
                    cInfo.lng = float.Parse(pairs2[i]["longitude"].ToString());
                    cInfo.lat = float.Parse(pairs2[i]["latitude"].ToString());
                    //[{"zoneName":"Indian/Kerguelen","gmtOffset":18000,"gmtOffsetName":"UTC+05:00","abbreviation":"TFT","tzName":"French Southern and Antarctic Time"}]
                    string timezones = pairs2[i]["timezones"].ToString();
                    Match mc = r_timezoneRegex.Match(timezones);
                    string gmtoffsetStr = mc.Value.Substring("\"gmtOffset\":".Length);
                    gmtoffsetStr = gmtoffsetStr.Substring(0, gmtoffsetStr.Length - 1);
                    int gmtoffset = int.Parse(gmtoffsetStr);
                    TimeSpan offset = TimeSpan.FromSeconds(gmtoffset);
                    cInfo.timeZoneOffset = offset;
                    //{"ja":"南極大陸","cn":"南极洲"}
                    string transName = pairs2[i]["translations"].ToString();
                    Match jaMc = r_jaRegex.Match(transName);
                    string jaTransNameStr = jaMc.Value.Substring("\"ja\":".Length);
                    jaTransNameStr = jaTransNameStr.Substring(0, jaTransNameStr.Length - 1);
                    Match cnMc = r_cnRegex.Match(transName);
                    string cnTransNameStr = cnMc.Value.Substring("\"cn\":".Length);
                    cnTransNameStr = cnTransNameStr.Substring(0, cnTransNameStr.Length - 1);
                    cInfo.transName = new Dictionary<Language, string>();
                    //            ZH_CN,      //简体中文
                    //ZH_TW,      //繁体中文
                    //EN,         //英语
                    //JP,         //日语
                    //MY,         //缅语
                    //SI,         //新哈拉语（兰卡语）
                    //TH          //泰语
                    cInfo.transName.Add(Language.ZH_CN, cnTransNameStr);
                    cInfo.transName.Add(Language.ZH_TW, cnTransNameStr);
                    cInfo.transName.Add(Language.EN, cInfo.name);
                    cInfo.transName.Add(Language.JP, jaTransNameStr);
                    cInfo.transName.Add(Language.MY, cInfo.name);
                    cInfo.transName.Add(Language.SI, cInfo.name);
                    cInfo.transName.Add(Language.TH, cInfo.name);
                    internationalFirstCityInfos.Add(cInfo.id, cInfo);
                }
            }
            //matchedWordList = SelectDictLike(db, matchedWordDic,"", inputStr);
        }, DBManager.CityDBurl);

#if DEBUG_PERFORMANCE || UNITY_EDITOR
        sw.Stop();
        Debug.LogError("【性能】查询国外一级城市耗时：" + sw.ElapsedMilliseconds);
#endif
    }
    //获取所有国外一级城市的子二三级城市信息
    public void GetAllInternationalSecondAndThirdCity(int countryID)
    {
        if (internationalFirstCityInfos[countryID].secondCityInfoList.Count > 0)
            return;
#if DEBUG_PERFORMANCE || UNITY_EDITOR
        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        sw.Start();
#endif

        dbManager.Getdb(db =>
        {
            //所有二级
            var reader2 = db.SelectAllInternationalSecondCity(countryID);
            //调用SQLite工具  解析对应数据
            Dictionary<string, object>[] pairs2 = SQLiteTools.GetValues(reader2);
            if (pairs2 != null)
            {
                int length = pairs2.Length;
                for (int i = 0; i < length; i++)
                {
                    SecondCityInfo cInfo = new SecondCityInfo();
                    cInfo = new SecondCityInfo();
                    cInfo.id = int.Parse(pairs2[i]["id"].ToString());
                    cInfo.name = pairs2[i]["name"].ToString();
                    cInfo.lng = float.Parse(pairs2[i]["longitude"].ToString());
                    cInfo.lat = float.Parse(pairs2[i]["latitude"].ToString());
                    cInfo.timeZoneOffset = internationalFirstCityInfos[countryID].timeZoneOffset;
                    cInfo.countryID = countryID;
                    internationalFirstCityInfos[countryID].secondCityInfoList.Add(cInfo.id, cInfo);
                }
            }
            //所有三级
            var reader3 = db.SelectAllInternationalFirstThirdCity(countryID);
            //调用SQLite工具  解析对应数据
            Dictionary<string, object>[] pairs3 = SQLiteTools.GetValues(reader3);
            if (pairs3 != null)
            {
                int length = pairs3.Length;
                for (int i = 0; i < length; i++)
                {
                    ThirdCityInfo cInfo = new ThirdCityInfo();
                    cInfo = new ThirdCityInfo();
                    cInfo.id = int.Parse(pairs3[i]["id"].ToString());
                    cInfo.name = pairs3[i]["name"].ToString();
                    cInfo.lng = float.Parse(pairs3[i]["longitude"].ToString());
                    cInfo.lat = float.Parse(pairs3[i]["latitude"].ToString());
                    cInfo.timeZoneOffset = internationalFirstCityInfos[countryID].timeZoneOffset;
                    cInfo.countryID = countryID;
                    cInfo.statesID = int.Parse(pairs3[i]["state_id"].ToString());

                    internationalFirstCityInfos[countryID].secondCityInfoList[cInfo.statesID].thirdCityInfoList.Add(cInfo.id, cInfo);
                }
            }
            //matchedWordList = SelectDictLike(db, matchedWordDic,"", inputStr);
        }, DBManager.CityDBurl);

#if DEBUG_PERFORMANCE || UNITY_EDITOR
        sw.Stop();
        Debug.LogError("【性能】查询国外二三级城市耗时：" + sw.ElapsedMilliseconds);
#endif
    }
    //模糊查询5个城市信息
    const int CITY_LIMIT_COUNT = 5;
    //模糊查询根据名字查国外城市信息
    public List<CityInfo> FuzzySearchInternationalCityInfoByName(string name)
    {
        List<CityInfo> res = new List<CityInfo>();
#if DEBUG_PERFORMANCE || UNITY_EDITOR
        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        sw.Start();
#endif
        dbManager.Getdb(db =>
        {
            var reader2 = db.FuzzySearchInternationalCity(name, CITY_LIMIT_COUNT);
            //调用SQLite工具  解析对应数据
            Dictionary<string, object>[] pairs2 = SQLiteTools.GetValues(reader2);
            if (pairs2 != null)
            {
                int length = pairs2.Length;
                for (int i = 0; i < length; i++)
                {
                    CityInfo cInfo = new CityInfo();
                    cInfo = new CityInfo();
                    cInfo.id = int.Parse(pairs2[i]["id"].ToString());
                    cInfo.name = pairs2[i]["name"].ToString();
                    cInfo.lng = float.Parse(pairs2[i]["longitude"].ToString());
                    cInfo.lat = float.Parse(pairs2[i]["latitude"].ToString());
                    //todo 无时区信息
                    //cInfo.timeZoneOffset = internationalFirstCityInfos[countryID].timeZoneOffset;
                    cInfo.countryID = int.Parse(pairs2[i]["country_id"].ToString());
                    cInfo.statesID = int.Parse(pairs2[i]["state_id"].ToString());
                }
            }
            //matchedWordList = SelectDictLike(db, matchedWordDic,"", inputStr);
        }, DBManager.CityDBurl);

#if DEBUG_PERFORMANCE || UNITY_EDITOR
        sw.Stop();
        Debug.LogError("【性能】模糊查询国外城市耗时：" + sw.ElapsedMilliseconds);
#endif
        return res;
    }
    //根据国外三级子城市信息，获取时区信息
    public void GetCityTimeZone(CityInfo third)
    {
#if DEBUG_PERFORMANCE || UNITY_EDITOR
        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        sw.Start();
#endif
        dbManager.Getdb(db =>
        {
            var reader2 = db.SelectInternationalFirstCity(third.countryID);
            //调用SQLite工具  解析对应数据
            Dictionary<string, object> pairs2 = SQLiteTools.GetValue(reader2);
            if (pairs2 != null)
            {
                string timezones = pairs2["timezones"].ToString();
                Match mc = r_timezoneRegex.Match(timezones);
                string gmtoffsetStr = mc.Value.Substring("\"gmtOffset\":".Length);
                gmtoffsetStr = gmtoffsetStr.Substring(0, gmtoffsetStr.Length - 1);
                int gmtoffset = int.Parse(gmtoffsetStr);
                TimeSpan offset = TimeSpan.FromSeconds(gmtoffset);
                third.timeZoneOffset = offset;

                //todo 无时区信息
                //cInfo.timeZoneOffset = internationalFirstCityInfos[countryID].timeZoneOffset;
                //cInfo.countryID = pairs2;
                //cInfo.statesID = int.Parse(pairs2[i]["state_id"].ToString());
            }
            //matchedWordList = SelectDictLike(db, matchedWordDic,"", inputStr);
        }, DBManager.CityDBurl);

#if DEBUG_PERFORMANCE || UNITY_EDITOR
        sw.Stop();
        Debug.LogError("【性能】查询国外一级城市耗时：" + sw.ElapsedMilliseconds);
#endif
    }
    #endregion
}
