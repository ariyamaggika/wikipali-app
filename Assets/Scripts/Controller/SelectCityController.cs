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
    //public static GeoTimeZone.TimeZoneLookup tzLookup;// = new GeoTimeZone.TimeZoneLookup();
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
    //public void InitTimeZoneLookup(GeoTimeZone.TimeZoneLookup timeZoneLookup)
    //{
    //    tzLookup = timeZoneLookup;
    //    tzLookup.LoadData();

    //}
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
        //public string level;
        public string pName;
        public int pCode;
        public string fullName;
        public float lng;
        public float lat;
        public TimeZoneInfo timeZone;   //时区//有根据datetime 计算出来的时区&根据datetime的当前是否是夏令时
        public Dictionary<Language, string> transName;  //翻译后的名字
        //city->state->country
        public int countryID;           //国外二级城市的父城市ID
        public int statesID;           //国外三级城市的父城市ID
        public string countryCode;//国外城市获取时区用
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
                    //cInfo.level = pairs2[i]["level"].ToString();
                    cInfo.pName = pairs2[i]["pname"].ToString();
                    //cInfo.pCode = int.Parse(pairs2[i]["pcode"].ToString());
                    cInfo.fullName = pairs2[i]["fullname"].ToString();
                    cInfo.lng = float.Parse(pairs2[i]["longitude"].ToString());
                    cInfo.lat = float.Parse(pairs2[i]["latitude"].ToString());

                    //TimeSpan offset = TimeSpan.FromHours(8);
                    cInfo.timeZone = TimeZoneManager.Instance().GetTimeZoneByName("China Standard Time");

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
    public Dictionary<int, SecondCityInfo> GetDomesticSecondCity(int countryID)
    {
        GetAllDomesticSecondAndThirdCity(countryID);
        return domesticFirstCityInfos[countryID].secondCityInfoList;
    }
    public Dictionary<int, ThirdCityInfo> GetDomesticThirdCity(SecondCityInfo city)
    {
        GetAllDomesticSecondAndThirdCity(city.pCode);
        return city.thirdCityInfoList;
    }
    //获取所有国内所有城市信息
    void GetAllDomesticSecondAndThirdCity(int countryID)
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
                    //cInfo.level = pairs2[i]["level"].ToString();
                    cInfo.pName = pairs2[i]["pname"].ToString();
                    cInfo.pCode = int.Parse(pairs2[i]["pcode"].ToString());
                    cInfo.fullName = pairs2[i]["fullname"].ToString();
                    cInfo.lng = float.Parse(pairs2[i]["longitude"].ToString());
                    cInfo.lat = float.Parse(pairs2[i]["latitude"].ToString());

                    //TimeSpan offset = TimeSpan.FromHours(8);
                    cInfo.timeZone = TimeZoneManager.Instance().GetTimeZoneByName("China Standard Time");
                    //cInfo.timeZone = offset;

                    cInfo.transName = new Dictionary<Language, string>();

                    domesticFirstCityInfos[countryID].secondCityInfoList.Add(cInfo.id, cInfo);
                }
            }
            else
            {
                //没有二级城市的沿用一级
                SecondCityInfo cInfo = new SecondCityInfo();
                cInfo = new SecondCityInfo();
                cInfo.id = domesticFirstCityInfos[countryID].id;
                cInfo.name = domesticFirstCityInfos[countryID].name;
                //cInfo.level = pairs2[i]["level"].ToString();
                //cInfo.pName = pairs2[i]["pname"].ToString();
                cInfo.pCode = cInfo.id;
                cInfo.fullName = domesticFirstCityInfos[countryID].fullName;
                cInfo.lng = domesticFirstCityInfos[countryID].lng;
                cInfo.lat = domesticFirstCityInfos[countryID].lat;

                //TimeSpan offset = TimeSpan.FromHours(8);
                //cInfo.timeZone = offset;
                cInfo.timeZone = TimeZoneManager.Instance().GetTimeZoneByName("China Standard Time");

                cInfo.transName = new Dictionary<Language, string>();

                domesticFirstCityInfos[countryID].secondCityInfoList.Add(cInfo.id, cInfo);
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
                    cInfo.id = int.Parse(pairs3[i]["code"].ToString());
                    cInfo.name = pairs3[i]["name"].ToString();
                    //cInfo.level = pairs3[i]["level"].ToString();
                    cInfo.pName = pairs3[i]["pname"].ToString();
                    cInfo.pCode = int.Parse(pairs3[i]["pcode"].ToString());
                    cInfo.fullName = pairs3[i]["fullname"].ToString();
                    cInfo.lng = float.Parse(pairs3[i]["longitude"].ToString());
                    cInfo.lat = float.Parse(pairs3[i]["latitude"].ToString());

                    //TimeSpan offset = TimeSpan.FromHours(8);
                    cInfo.timeZone = TimeZoneManager.Instance().GetTimeZoneByName("China Standard Time");
                    //cInfo.timeZone = offset;

                    cInfo.transName = new Dictionary<Language, string>();
                    domesticFirstCityInfos[countryID].secondCityInfoList[cInfo.pCode].thirdCityInfoList.Add(cInfo.id, cInfo);
                }
            }
            //else
            //{
            //没有三级城市的沿用二级
            //for (int i = 0; i < domesticFirstCityInfos[countryID].secondCityInfoList.Count; i++)
            foreach (var city in domesticFirstCityInfos[countryID].secondCityInfoList)
            {
                if (city.Value.thirdCityInfoList.Count > 0)
                    continue;
                ThirdCityInfo cInfo = new ThirdCityInfo();
                cInfo = new ThirdCityInfo();
                cInfo.id = city.Value.id;
                cInfo.name = city.Value.name;
                //cInfo.level = pairs3[i]["level"].ToString();
                cInfo.pName = city.Value.pName;
                cInfo.pCode = city.Value.pCode;
                cInfo.fullName = city.Value.fullName;
                cInfo.lng = city.Value.lng;
                cInfo.lat = city.Value.lat;

                //TimeSpan offset = TimeSpan.FromHours(8);
                //cInfo.timeZone = offset;
                cInfo.timeZone = TimeZoneManager.Instance().GetTimeZoneByName("China Standard Time");

                cInfo.transName = new Dictionary<Language, string>();
                city.Value.thirdCityInfoList.Add(cInfo.id, cInfo);
            }
            //}
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
                    //cInfo.level = pairs2[i]["level"].ToString();
                    cInfo.pName = pairs2[i]["pname"].ToString();
                    cInfo.pCode = int.Parse(pairs2[i]["pcode"].ToString());
                    cInfo.fullName = pairs2[i]["fullname"].ToString();
                    cInfo.lng = float.Parse(pairs2[i]["longitude"].ToString());
                    cInfo.lat = float.Parse(pairs2[i]["latitude"].ToString());

                    //TimeSpan offset = TimeSpan.FromHours(8);
                    //cInfo.timeZone = offset;
                    cInfo.timeZone = TimeZoneManager.Instance().GetTimeZoneByName("China Standard Time");

                    cInfo.transName = new Dictionary<Language, string>();
                    res.Add(cInfo);
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

    public static int LAT_RANGE = 2;
    public static int LNG_RANGE = 2;
    public CityInfo GetCurrCityInfo(float lat, float lng)
    {
        CityInfo minCity = null;
        CityInfo cityInfo = new CityInfo();
        cityInfo.lat = lat;
        cityInfo.lng = lng;
        //先遍历查看国内所有一级城市，再遍历查看所有国外一级城市，再细分
#if DEBUG_PERFORMANCE || UNITY_EDITOR
        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        sw.Start();
#endif
        //List<CityInfo> firstCityInfos = new List<CityInfo>();
        dbManager.Getdb(db =>
        {
            var reader2 = db.SelectAllDomesticFirstCityByLatLng(lat, lng);
            //调用SQLite工具  解析对应数据
            Dictionary<string, object>[] pairs2 = SQLiteTools.GetValues(reader2);
            float minDistance = 999999;
            if (pairs2 != null)//身在国内
            {
                int length = pairs2.Length;
                for (int i = 0; i < length; i++)
                {

                    float nLng = float.Parse(pairs2[i]["longitude"].ToString());
                    float nLat = float.Parse(pairs2[i]["latitude"].ToString());
                    float distance = Mathf.Abs(lng - nLng) + Mathf.Abs(lat - nLat);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                    }
                    else
                        continue;
                    FirstCityInfo cInfo = new FirstCityInfo();
                    cInfo = new FirstCityInfo();
                    cInfo.id = int.Parse(pairs2[i]["code"].ToString());
                    cInfo.name = pairs2[i]["name"].ToString();
                    //cInfo.level = pairs2[i]["level"].ToString();
                    cInfo.pName = pairs2[i]["pname"].ToString();
                    //cInfo.pCode = int.Parse(pairs2[i]["pcode"].ToString());
                    cInfo.fullName = pairs2[i]["fullname"].ToString();
                    cInfo.lng = nLng;
                    cInfo.lat = nLat;
                    //TimeSpan offset = TimeSpan.FromHours(8);
                    //cInfo.timeZone = offset;
                    cInfo.timeZone = TimeZoneManager.Instance().GetTimeZoneByName("China Standard Time");
                    cInfo.transName = new Dictionary<Language, string>();
                    minCity = cInfo;
                }
                minDistance = 999999;
                //todo 排序选一个最近的，/*然后获取所有二级城市，再选一个最近的，*/然后获取所有三级城市，再选一个最近的
                if (minCity != null)
                {
                    var reader21 = db.SelectAllDomesticThirdCity(minCity.id, minCity.id + 9999);
                    //调用SQLite工具  解析对应数据
                    Dictionary<string, object>[] pairs21 = SQLiteTools.GetValues(reader21);
                    length = pairs21.Length;
                    for (int i = 0; i < length; i++)
                    {
                        float nLng = float.Parse(pairs21[i]["longitude"].ToString());
                        float nLat = float.Parse(pairs21[i]["latitude"].ToString());
                        float distance = Mathf.Abs(lng - nLng) + Mathf.Abs(lat - nLat);
                        if (distance < minDistance)
                        {
                            minDistance = distance;
                        }
                        else
                            continue;
                        ThirdCityInfo cInfo = new ThirdCityInfo();
                        cInfo = new ThirdCityInfo();
                        cInfo.id = int.Parse(pairs21[i]["code"].ToString());
                        cInfo.name = pairs21[i]["name"].ToString();
                        //cInfo.level = pairs21[i]["level"].ToString();
                        cInfo.pName = pairs21[i]["pname"].ToString();
                        //cInfo.pCode = int.Parse(pairs21[i]["pcode"].ToString());
                        cInfo.fullName = pairs21[i]["fullname"].ToString();
                        cInfo.lng = nLng;
                        cInfo.lat = nLat;
                        //TimeSpan offset = TimeSpan.FromHours(8);
                        //cInfo.timeZone = offset;
                        cInfo.timeZone = TimeZoneManager.Instance().GetTimeZoneByName("China Standard Time");
                        cInfo.transName = new Dictionary<Language, string>();
                        minCity = cInfo;
                    }
                }
            }
            else//身在国外
            {
                //边界容易在中国境内，todo:直接查三级城市
                #region 查一级城市
                //minDistance = 999999;
                //var reader3 = db.SelectAllInternationalFirstCityByLatLng(lat, lng);
                ////调用SQLite工具  解析对应数据
                //Dictionary<string, object>[] pairs3 = SQLiteTools.GetValues(reader3);
                //int length = pairs3.Length;
                //for (int i = 0; i < length; i++)
                //{
                //    float nLng = float.Parse(pairs3[i]["longitude"].ToString());
                //    float nLat = float.Parse(pairs3[i]["latitude"].ToString());
                //    float distance = Mathf.Abs(lng - nLng) + Mathf.Abs(lat - nLat);
                //    if (distance < minDistance)
                //    {
                //        minDistance = distance;
                //    }
                //    else
                //        continue;
                //    FirstCityInfo cInfo = new FirstCityInfo();
                //    cInfo = new FirstCityInfo();
                //    cInfo.id = int.Parse(pairs3[i]["id"].ToString());
                //    cInfo.name = pairs3[i]["name"].ToString();
                //    cInfo.lng = nLng;
                //    cInfo.lat = nLat;

                //    //try
                //    //{

                //    //    GeoTimeZone.TimeZoneResult tzResult = tzLookup.GetTimeZone(cInfo.lat, cInfo.lng);
                //    //    Debug.LogError(tzResult.Result);
                //    //    //TimeZoneInfo info = TimeZoneInfo.FindSystemTimeZoneById(tzResult.Result);
                //    //    TimeZoneInfo info = TimeZoneInfo.FindSystemTimeZoneById(GetMSTZID(tzResult.Result));
                //    //    //if (info != null)
                //    //    //    Debug.LogError("@@@@" + info.BaseUtcOffset.TotalSeconds);
                //    //    if (info != null)
                //    //        cInfo.timeZoneOffset = TimeSpan.FromSeconds(info.BaseUtcOffset.TotalSeconds);
                //    //    else
                //    //        Debug.LogError("TimeZoneInfo is null!!!!!!!!!!!!!!!!!!!!!!!!" + tzResult.Result);
                //    //}
                //    //catch (Exception e)
                //    //{
                //    //    try
                //    //    {
                //    //        TimeZoneInfo info = TimeZoneInfo.FindSystemTimeZoneById(GetMSTZIDByCCode(cInfo.countryCode));
                //    //        //if (info != null)
                //    //        //    Debug.LogError("@@@@" + info.BaseUtcOffset.TotalSeconds);
                //    //        if (info != null)
                //    //            cInfo.timeZoneOffset = TimeSpan.FromSeconds(info.BaseUtcOffset.TotalSeconds);
                //    //        else
                //    //            Debug.LogError("GetMSTZIDByCCode is null!!!!!!!!!!!!!!!!!!!!!!!!" + cInfo.countryCode);
                //    //    }
                //    //    catch (Exception e2)
                //    //    {
                //    //        Debug.LogError(e);

                //    //        Debug.LogError(e2);
                //    //[{"zoneName":"Indian/Kerguelen","gmtOffset":18000,"gmtOffsetName":"UTC+05:00","abbreviation":"TFT","tzName":"French Southern and Antarctic Time"}]
                //    string timezones = pairs3[i]["timezones"].ToString();
                //    Match mc = r_timezoneRegex.Match(timezones);
                //    string gmtoffsetStr = mc.Value.Substring("\"gmtOffset\":".Length);
                //    gmtoffsetStr = gmtoffsetStr.Substring(0, gmtoffsetStr.Length - 1);
                //    int gmtoffset = int.Parse(gmtoffsetStr);
                //    TimeSpan offset = TimeSpan.FromSeconds(gmtoffset);
                //    cInfo.timeZoneOffset = offset;
                //    //    }
                //    //}


                //    //{"ja":"南極大陸","cn":"南极洲"}
                //    string transName = pairs3[i]["translations"].ToString();
                //    Match jaMc = r_jaRegex.Match(transName);
                //    string jaTransNameStr = cInfo.name;
                //    if (!string.IsNullOrEmpty(jaMc.Value))
                //    {
                //        jaTransNameStr = jaMc.Value.Substring("\"ja\":".Length);
                //        jaTransNameStr = jaTransNameStr.Substring(0, jaTransNameStr.Length - 1).Replace("\"", "");
                //    }
                //    string cnTransNameStr = cInfo.name;
                //    Match cnMc = r_cnRegex.Match(transName);
                //    if (!string.IsNullOrEmpty(cnMc.Value))
                //    {
                //        cnTransNameStr = cnMc.Value.Substring("\"cn\":".Length);
                //        cnTransNameStr = cnTransNameStr.Substring(0, cnTransNameStr.Length - 1).Replace("\"", "");
                //    }
                //    cInfo.transName = new Dictionary<Language, string>();
                //    //            ZH_CN,      //简体中文
                //    //ZH_TW,      //繁体中文
                //    //EN,         //英语
                //    //JP,         //日语
                //    //MY,         //缅语
                //    //SI,         //新哈拉语（兰卡语）
                //    //TH          //泰语
                //    cInfo.transName.Add(Language.ZH_CN, cnTransNameStr);
                //    cInfo.transName.Add(Language.ZH_TW, cnTransNameStr);
                //    cInfo.transName.Add(Language.EN, cInfo.name);
                //    cInfo.transName.Add(Language.JP, jaTransNameStr);
                //    cInfo.transName.Add(Language.MY, cInfo.name);
                //    cInfo.transName.Add(Language.SI, cInfo.name);
                //    cInfo.transName.Add(Language.TH, cInfo.name);
                //    minCity = cInfo;
                //}
                //minDistance = 999999;
                ////todo 排序选一个最近的，/*然后获取所有二级城市，再选一个最近的，*/然后获取所有三级城市，再选一个最近的
                //if (minCity != null)
                //{
                //    var reader31 = db.SelectAllInternationalFirstThirdCity(minCity.id);
                //    //调用SQLite工具  解析对应数据
                //    Dictionary<string, object>[] pairs31 = SQLiteTools.GetValues(reader31);
                //    length = pairs31.Length;
                //    for (int i = 0; i < length; i++)
                //    {
                //        float nLng = float.Parse(pairs31[i]["longitude"].ToString());
                //        float nLat = float.Parse(pairs31[i]["latitude"].ToString());
                //        float distance = Mathf.Abs(lng - nLng) + Mathf.Abs(lat - nLat);
                //        if (distance < minDistance)
                //        {
                //            minDistance = distance;
                //        }
                //        else
                //            continue;
                //        ThirdCityInfo cInfo = new ThirdCityInfo();
                //        cInfo = new ThirdCityInfo();
                //        cInfo.id = int.Parse(pairs31[i]["id"].ToString());
                //        cInfo.name = pairs31[i]["name"].ToString();
                //        cInfo.lng = nLng;
                //        cInfo.lat = nLat;
                //        cInfo.timeZoneOffset = minCity.timeZoneOffset;
                //        cInfo.countryID = minCity.id;
                //        cInfo.statesID = int.Parse(pairs31[i]["state_id"].ToString());
                //        minCity = cInfo;
                //    }
                //}
                #endregion
                #region 直接插三级城市
                //todo 没有3级城市，只有1级城市的就查不到了
                minDistance = 999999;
                var reader3 = db.SelectAllInternationalThirdCityByLatLng(lat, lng);
                //调用SQLite工具  解析对应数据
                Dictionary<string, object>[] pairs3 = SQLiteTools.GetValues(reader3);
                int length = pairs3.Length;
                for (int i = 0; i < length; i++)
                {
                    float nLng = float.Parse(pairs3[i]["longitude"].ToString());
                    float nLat = float.Parse(pairs3[i]["latitude"].ToString());
                    float distance = Mathf.Abs(lng - nLng) + Mathf.Abs(lat - nLat);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                    }
                    else
                        continue;
                    ThirdCityInfo cInfo = new ThirdCityInfo();
                    cInfo = new ThirdCityInfo();
                    cInfo.id = int.Parse(pairs3[i]["id"].ToString());
                    cInfo.name = pairs3[i]["name"].ToString();
                    cInfo.lng = nLng;
                    cInfo.lat = nLat;
                    //cInfo.timeZoneOffset = minCity.timeZoneOffset;
                    cInfo.countryID = int.Parse(pairs3[i]["country_id"].ToString());
                    cInfo.statesID = int.Parse(pairs3[i]["state_id"].ToString());
                    minCity = cInfo;
                }
                #endregion
            }
            //matchedWordList = SelectDictLike(db, matchedWordDic,"", inputStr);
        }, DBManager.CityDBurl);

#if DEBUG_PERFORMANCE || UNITY_EDITOR
        sw.Stop();
        Debug.LogError("【性能】查询国内一级城市耗时：" + sw.ElapsedMilliseconds);
#endif

        return minCity;

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
                    cInfo.countryCode = pairs2[i]["iso2"].ToString();
                    //?????经纬度用double??????
                    cInfo.lng = float.Parse(pairs2[i]["longitude"].ToString());
                    cInfo.lat = float.Parse(pairs2[i]["latitude"].ToString());
                 
                    //string timezones = pairs2[i]["timezones"].ToString();
                    //Match mc = r_timezoneRegex.Match(timezones);
                    //string gmtoffsetStr = mc.Value.Substring("\"gmtOffset\":".Length);
                    //gmtoffsetStr = gmtoffsetStr.Substring(0, gmtoffsetStr.Length - 1);
                    ////Debug.LogError("&&&&" + gmtoffsetStr);
                    //int gmtoffset = int.Parse(gmtoffsetStr);
                    //TimeSpan offset = TimeSpan.FromSeconds(gmtoffset);
                    //cInfo.timeZone = offset;
                    TimeZoneInfo timeZone = TimeZoneManager.Instance().CaculateTimeZone(cInfo.lat, cInfo.lng);
                    cInfo.timeZone = timeZone;
                    //    }
                    //}


                    //{"ja":"南極大陸","cn":"南极洲"}
                    string transName = pairs2[i]["translations"].ToString();
                    Match jaMc = r_jaRegex.Match(transName);
                    string jaTransNameStr = cInfo.name;
                    if (!string.IsNullOrEmpty(jaMc.Value))
                    {
                        jaTransNameStr = jaMc.Value.Substring("\"ja\":".Length);
                        jaTransNameStr = jaTransNameStr.Substring(0, jaTransNameStr.Length - 1).Replace("\"", "");
                    }
                    string cnTransNameStr = cInfo.name;
                    Match cnMc = r_cnRegex.Match(transName);
                    if (!string.IsNullOrEmpty(cnMc.Value))
                    {
                        cnTransNameStr = cnMc.Value.Substring("\"cn\":".Length);
                        cnTransNameStr = cnTransNameStr.Substring(0, cnTransNameStr.Length - 1).Replace("\"", "");
                    }
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
    public Dictionary<int, SecondCityInfo> GetInternationalSecondCity(int countryID)
    {
        GetAllInternationalSecondAndThirdCity(countryID);
        return internationalFirstCityInfos[countryID].secondCityInfoList;
    }
    public Dictionary<int, ThirdCityInfo> GetInternationalThirdCity(SecondCityInfo city)
    {
        GetAllInternationalSecondAndThirdCity(city.countryID);
        return city.thirdCityInfoList;
    }
    //获取所有国外一级城市的子二三级城市信息
    void GetAllInternationalSecondAndThirdCity(int countryID)
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
                    cInfo.countryCode = pairs2[i]["country_code"].ToString();

                    cInfo.name = pairs2[i]["name"].ToString();
                    //??????????????有的国外二级城市没有经纬度
                    if (!pairs2[i].ContainsKey("longitude") || pairs2[i]["longitude"] == null)//二级城市经纬度为空就取一级城市的经纬度
                    {
                        cInfo.lng = internationalFirstCityInfos[countryID].lng;
                        cInfo.lat = internationalFirstCityInfos[countryID].lat;
                    }
                    else
                    {
                        cInfo.lng = float.Parse(pairs2[i]["longitude"].ToString());
                        cInfo.lat = float.Parse(pairs2[i]["latitude"].ToString());
                    }
                    TimeZoneInfo timeZone = TimeZoneManager.Instance().CaculateTimeZone(cInfo.lat, cInfo.lng);
                    //cInfo.timeZoneOffset = internationalFirstCityInfos[countryID].timeZoneOffset;
                    cInfo.timeZone = timeZone;
                    cInfo.countryID = countryID;
                    internationalFirstCityInfos[countryID].secondCityInfoList.Add(cInfo.id, cInfo);
                }
            }
            else
            {
                //没有二级城市的沿用一级
                SecondCityInfo cInfo = new SecondCityInfo();
                cInfo = new SecondCityInfo();
                cInfo.id = internationalFirstCityInfos[countryID].id;
                cInfo.name = internationalFirstCityInfos[countryID].name;
                //cInfo.level = pairs2[i]["level"].ToString();
                //cInfo.pName = pairs2[i]["pname"].ToString();
                cInfo.pCode = cInfo.id;
                cInfo.fullName = internationalFirstCityInfos[countryID].fullName;
                cInfo.lng = internationalFirstCityInfos[countryID].lng;
                cInfo.lat = internationalFirstCityInfos[countryID].lat;
                cInfo.countryCode = internationalFirstCityInfos[countryID].countryCode;

                cInfo.timeZone = internationalFirstCityInfos[countryID].timeZone;
                cInfo.countryID = countryID;
                internationalFirstCityInfos[countryID].secondCityInfoList.Add(cInfo.id, cInfo);
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
                    //cInfo.timeZoneOffset = internationalFirstCityInfos[countryID].timeZoneOffset;
                    TimeZoneInfo timeZone = TimeZoneManager.Instance().CaculateTimeZone(cInfo.lat, cInfo.lng);
                    cInfo.timeZone = timeZone;
                    cInfo.countryID = countryID;
                    cInfo.statesID = int.Parse(pairs3[i]["state_id"].ToString());
                    cInfo.countryCode = pairs3[i]["country_code"].ToString();

                    internationalFirstCityInfos[countryID].secondCityInfoList[cInfo.statesID].thirdCityInfoList.Add(cInfo.id, cInfo);
                }
            }
            //else
            //{
            //没有三级城市的沿用二级
            foreach (var city in internationalFirstCityInfos[countryID].secondCityInfoList)
            {
                if (city.Value.thirdCityInfoList.Count > 0)
                    continue;
                ThirdCityInfo cInfo = new ThirdCityInfo();
                cInfo = new ThirdCityInfo();
                cInfo.id = city.Value.id;
                cInfo.name = city.Value.name;
                //cInfo.level = pairs3[i]["level"].ToString();
                cInfo.pName = city.Value.pName;
                cInfo.pCode = city.Value.pCode;
                cInfo.fullName = city.Value.fullName;
                cInfo.lng = city.Value.lng;
                cInfo.lat = city.Value.lat;
                cInfo.countryCode = city.Value.countryCode;

                cInfo.timeZone = city.Value.timeZone;

                cInfo.transName = new Dictionary<Language, string>();

                cInfo.countryID = city.Value.countryID;
                cInfo.statesID = city.Value.statesID;

                city.Value.thirdCityInfoList.Add(cInfo.id, cInfo);
            }
            //}
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
    //???todo只有三级城市信息 是否需要查找一二级城市???
    public List<CityInfo> FuzzySearchInternationalCityInfoByName(string name)
    {
        List<CityInfo> res = new List<CityInfo>();
#if DEBUG_PERFORMANCE || UNITY_EDITOR
        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        sw.Start();
#endif
        dbManager.Getdb(db =>
        {
            var reader2 = db.FuzzySearchInternationalThirdCity(name, CITY_LIMIT_COUNT);
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
                    cInfo.countryCode = pairs2[i]["country_code"].ToString();

                    cInfo.lng = float.Parse(pairs2[i]["longitude"].ToString());
                    cInfo.lat = float.Parse(pairs2[i]["latitude"].ToString());
                    TimeZoneInfo timeZone = TimeZoneManager.Instance().CaculateTimeZone(cInfo.lat, cInfo.lng);
                    cInfo.timeZone = timeZone;
                    //todo 无时区信息
                    //try
                    //{

                    //    GeoTimeZone.TimeZoneResult tzResult = tzLookup.GetTimeZone(cInfo.lat, cInfo.lng);
                    //    Debug.LogError(tzResult.Result);
                    //    //TimeZoneInfo info = TimeZoneInfo.FindSystemTimeZoneById(tzResult.Result);
                    //    TimeZoneInfo info = TimeZoneInfo.FindSystemTimeZoneById(GetMSTZID(tzResult.Result));
                    //    //if (info != null)
                    //    //    Debug.LogError("@@@@" + info.BaseUtcOffset.TotalSeconds);
                    //    if (info != null)
                    //        cInfo.timeZoneOffset = TimeSpan.FromSeconds(info.BaseUtcOffset.TotalSeconds);
                    //    else
                    //        Debug.LogError("TimeZoneInfo is null!!!!!!!!!!!!!!!!!!!!!!!!" + tzResult.Result);
                    //}
                    //catch (Exception e)
                    //{
                    //    Debug.LogError(e);
                    //    try
                    //    {
                    //        TimeZoneInfo info = TimeZoneInfo.FindSystemTimeZoneById(GetMSTZIDByCCode(cInfo.countryCode));
                    //        //if (info != null)
                    //        //    Debug.LogError("@@@@" + info.BaseUtcOffset.TotalSeconds);
                    //        if (info != null)
                    //            cInfo.timeZoneOffset = TimeSpan.FromSeconds(info.BaseUtcOffset.TotalSeconds);
                    //        else
                    //            Debug.LogError("GetMSTZIDByCCode is null!!!!!!!!!!!!!!!!!!!!!!!!" + cInfo.countryCode);
                    //    }
                    //    catch (Exception e2)
                    //    {
                    //        Debug.LogError(e);
                    //        Debug.LogError(e2);
                    //    }
                    //}

                    //cInfo.timeZoneOffset = 
                    cInfo.countryID = int.Parse(pairs2[i]["country_id"].ToString());
                    cInfo.statesID = int.Parse(pairs2[i]["state_id"].ToString());
                    res.Add(cInfo);
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
    public List<CityInfo> FuzzySearchInternationalCityInfoByTransName(string name)
    {
        List<CityInfo> res = new List<CityInfo>();
#if DEBUG_PERFORMANCE || UNITY_EDITOR
        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        sw.Start();
#endif
        dbManager.Getdb(db =>
        {
            var reader2 = db.FuzzySearchInternationalCityByTrans(name, CITY_LIMIT_COUNT);
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
                    TimeZoneInfo timeZone = TimeZoneManager.Instance().CaculateTimeZone(cInfo.lat, cInfo.lng);
                    cInfo.timeZone = timeZone;
                    cInfo.countryCode = pairs2[i]["iso2"].ToString();

                    //{"ja":"南極大陸","cn":"南极洲"}
                    string transName = pairs2[i]["translations"].ToString();
                    Match jaMc = r_jaRegex.Match(transName);
                    string jaTransNameStr = cInfo.name;
                    if (!string.IsNullOrEmpty(jaMc.Value))
                    {
                        jaTransNameStr = jaMc.Value.Substring("\"ja\":".Length);
                        jaTransNameStr = jaTransNameStr.Substring(0, jaTransNameStr.Length - 1).Replace("\"", "");
                    }
                    string cnTransNameStr = cInfo.name;
                    Match cnMc = r_cnRegex.Match(transName);
                    if (!string.IsNullOrEmpty(cnMc.Value))
                    {
                        cnTransNameStr = cnMc.Value.Substring("\"cn\":".Length);
                        cnTransNameStr = cnTransNameStr.Substring(0, cnTransNameStr.Length - 1).Replace("\"", "");
                    }
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
                    //todo 无时区信息
                    //cInfo.timeZoneOffset = internationalFirstCityInfos[countryID].timeZoneOffset;
                    //cInfo.countryID = int.Parse(pairs2[i]["country_id"].ToString());
                    //cInfo.statesID = int.Parse(pairs2[i]["state_id"].ToString());
                    res.Add(cInfo);
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
    //    public void GetCityTimeZone(CityInfo third)
    //    {
    //#if DEBUG_PERFORMANCE || UNITY_EDITOR
    //        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
    //        sw.Start();
    //#endif
    //        dbManager.Getdb(db =>
    //        {
    //            var reader2 = db.SelectInternationalFirstCity(third.countryID);
    //            //调用SQLite工具  解析对应数据
    //            Dictionary<string, object> pairs2 = SQLiteTools.GetValue(reader2);
    //            if (pairs2 != null)
    //            {

    //                //try
    //                //{

    //                //    GeoTimeZone.TimeZoneResult tzResult = tzLookup.GetTimeZone(third.lat, third.lng);
    //                //    Debug.LogError(tzResult.Result);
    //                //    //TimeZoneInfo info = TimeZoneInfo.FindSystemTimeZoneById(tzResult.Result);
    //                //    TimeZoneInfo info = TimeZoneInfo.FindSystemTimeZoneById(GetMSTZID(tzResult.Result));
    //                //    //if (info != null)
    //                //    //    Debug.LogError("@@@@" + info.BaseUtcOffset.TotalSeconds);
    //                //    if (info != null)
    //                //        third.timeZoneOffset = TimeSpan.FromSeconds(info.BaseUtcOffset.TotalSeconds);
    //                //    else
    //                //        Debug.LogError("TimeZoneInfo is null!!!!!!!!!!!!!!!!!!!!!!!!" + tzResult.Result);
    //                //}
    //                //catch (Exception e)
    //                //{
    //                //    try
    //                //    {
    //                //        TimeZoneInfo info = TimeZoneInfo.FindSystemTimeZoneById(GetMSTZIDByCCode(third.countryCode));
    //                //        //if (info != null)
    //                //        //    Debug.LogError("@@@@" + info.BaseUtcOffset.TotalSeconds);
    //                //        if (info != null)
    //                //            third.timeZoneOffset = TimeSpan.FromSeconds(info.BaseUtcOffset.TotalSeconds);
    //                //        else
    //                //            Debug.LogError("GetMSTZIDByCCode is null!!!!!!!!!!!!!!!!!!!!!!!!" + third.countryCode);
    //                //    }
    //                //    catch (Exception e2)
    //                //    {
    //                //        Debug.LogError(e);
    //                //        Debug.LogError(e2);
    //                string timezones = pairs2["timezones"].ToString();
    //                Match mc = r_timezoneRegex.Match(timezones);
    //                string gmtoffsetStr = mc.Value.Substring("\"gmtOffset\":".Length);
    //                gmtoffsetStr = gmtoffsetStr.Substring(0, gmtoffsetStr.Length - 1);
    //                int gmtoffset = int.Parse(gmtoffsetStr);
    //                TimeSpan offset = TimeSpan.FromSeconds(gmtoffset);
    //                third.timeZoneOffset = offset;
    //                //    }

    //                //}
    //                //todo 无时区信息
    //                //cInfo.timeZoneOffset = internationalFirstCityInfos[countryID].timeZoneOffset;
    //                //cInfo.countryID = pairs2;
    //                //cInfo.statesID = int.Parse(pairs2[i]["state_id"].ToString());
    //            }
    //            //matchedWordList = SelectDictLike(db, matchedWordDic,"", inputStr);
    //        }, DBManager.CityDBurl);

    //#if DEBUG_PERFORMANCE || UNITY_EDITOR
    //        sw.Stop();
    //        Debug.LogError("【性能】查询国外一级城市耗时：" + sw.ElapsedMilliseconds);
    //#endif
    //    }
    #endregion
    #region timeZoneID转换
    public string GetMSTZID(string tz)
    {

        string res = "";
        dbManager.Getdb(db =>
        {
            var readerPali = db.SearchMSTimeZoneIDByTimeZoneID(tz);
            Dictionary<string, object> paliPair = SQLiteTools.GetValue(readerPali);
            if (paliPair != null)
            {
                res = paliPair["other"].ToString();
            }

        }, DBManager.CityDBurl);
        return res;
    }
    public string GetMSTZIDByCCode(string tz)
    {

        string res = "";
        dbManager.Getdb(db =>
        {
            var readerPali = db.SearchMSTimeZoneIDByCcode(tz);
            Dictionary<string, object> paliPair = SQLiteTools.GetValue(readerPali);
            if (paliPair != null)
            {
                res = paliPair["other"].ToString();
            }

        }, DBManager.CityDBurl);
        return res;
    }
    #endregion
}
