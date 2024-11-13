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
    public class FirstCityInfo
    {
        public List<SecondCityInfo> secondCityInfoList = new List<SecondCityInfo>();
        public CityInfo cityInfo = new CityInfo();
    }
    public class SecondCityInfo
    {
        public List<ThirdCityInfo> thirdCityInfoList = new List<ThirdCityInfo>();
        public CityInfo cityInfo = new CityInfo();
    }

    public class ThirdCityInfo
    {
        public CityInfo cityInfo = new CityInfo();
    }
    public struct CityInfo
    {
        public int id;
        public string name;
        public string level;
        public string pName;
        public string fullName;
        public float lng;
        public float lat;
        public TimeSpan timeZoneOffset;   //时区
        public Dictionary<Language, string> transName;  //翻译后的名字
        //city->state->country
        public int countryID;           //国外二级城市的父城市ID
        public int statesID;           //国外三级城市的父城市ID
    }
    List<FirstCityInfo> domesticFirstCityInfos = new List<FirstCityInfo>();
    List<FirstCityInfo> internationalFirstCityInfos = new List<FirstCityInfo>();
    //获取所有国内一级城市信息
    public void GetAllDomesticFirstCity()
    {
        //firstCityInfos.Clear();
#if DEBUG_PERFORMANCE || UNITY_EDITOR
        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        sw.Start();
#endif

        //dbManager.Getdb(db =>
        //{
        //    //key: word,value: MatchedWord
        //    Dictionary<string, MatchedWord> matchedWordDic = new Dictionary<string, MatchedWord>();
        //    GetDictLikeByLanguage(matchedWordList, SettingManager.Instance().language, db, matchedWordDic, inputStr);
        //    if (matchedWordList.Count < LIMIT_COUNT)
        //    {
        //        //查完自己语言的词典，找不到就找英文，再找不到就找缅文
        //        if (SettingManager.Instance().language == Language.EN)
        //        {
        //            GetDictLikeByLanguage(matchedWordList, Language.MY, db, matchedWordDic, inputStr);

        //        }
        //        else if (SettingManager.Instance().language != Language.MY)
        //        {
        //            GetDictLikeByLanguage(matchedWordList, Language.EN, db, matchedWordDic, inputStr);
        //            if (matchedWordList.Count < LIMIT_COUNT)
        //                GetDictLikeByLanguage(matchedWordList, Language.MY, db, matchedWordDic, inputStr);
        //        }
        //    }
        //    //matchedWordList = SelectDictLike(db, matchedWordDic,"", inputStr);
        //}, DBManager.CityDBurl);

#if DEBUG_PERFORMANCE || UNITY_EDITOR
        sw.Stop();
        Debug.LogError("【性能】查询国内一级城市耗时：" + sw.ElapsedMilliseconds);
#endif
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
                    cInfo.cityInfo = new CityInfo();
                    cInfo.cityInfo.id = int.Parse(pairs2[i]["id"].ToString());
                    cInfo.cityInfo.name = pairs2[i]["name"].ToString();
                    cInfo.cityInfo.lng = float.Parse(pairs2[i]["longitude"].ToString());
                    cInfo.cityInfo.lat = float.Parse(pairs2[i]["latitude"].ToString());
                    //[{"zoneName":"Indian/Kerguelen","gmtOffset":18000,"gmtOffsetName":"UTC+05:00","abbreviation":"TFT","tzName":"French Southern and Antarctic Time"}]
                    string timezones = pairs2[i]["timezones"].ToString();
                    Match mc = r_timezoneRegex.Match(timezones);
                    string gmtoffsetStr = mc.Value.Substring("\"gmtOffset\":".Length);
                    gmtoffsetStr = gmtoffsetStr.Substring(0, gmtoffsetStr.Length - 1);
                    int gmtoffset = int.Parse(gmtoffsetStr);
                    TimeSpan offset = TimeSpan.FromSeconds(gmtoffset);
                    cInfo.cityInfo.timeZoneOffset = offset;
                    //{"ja":"南極大陸","cn":"南极洲"}
                    string transName = pairs2[i]["translations"].ToString();
                    Match jaMc = r_jaRegex.Match(timezones);
                    string jaTransNameStr = jaMc.Value.Substring("\"ja\":".Length);
                    jaTransNameStr = jaTransNameStr.Substring(0, jaTransNameStr.Length - 1);
                    Match cnMc = r_cnRegex.Match(timezones);
                    string cnTransNameStr = cnMc.Value.Substring("\"cn\":".Length);
                    cnTransNameStr = cnTransNameStr.Substring(0, cnTransNameStr.Length - 1);
                    cInfo.cityInfo.transName = new Dictionary<Language, string>();
                    //            ZH_CN,      //简体中文
                    //ZH_TW,      //繁体中文
                    //EN,         //英语
                    //JP,         //日语
                    //MY,         //缅语
                    //SI,         //新哈拉语（兰卡语）
                    //TH          //泰语
                    cInfo.cityInfo.transName.Add(Language.ZH_CN, cnTransNameStr);
                    cInfo.cityInfo.transName.Add(Language.ZH_TW, cnTransNameStr);
                    cInfo.cityInfo.transName.Add(Language.EN, cInfo.cityInfo.name);
                    cInfo.cityInfo.transName.Add(Language.JP, jaTransNameStr);
                    cInfo.cityInfo.transName.Add(Language.MY, cInfo.cityInfo.name);
                    cInfo.cityInfo.transName.Add(Language.SI, cInfo.cityInfo.name);
                    cInfo.cityInfo.transName.Add(Language.TH, cInfo.cityInfo.name);
                }
            }
            //matchedWordList = SelectDictLike(db, matchedWordDic,"", inputStr);
        }, DBManager.CityDBurl);

#if DEBUG_PERFORMANCE || UNITY_EDITOR
        sw.Stop();
        Debug.LogError("【性能】查询国内一级城市耗时：" + sw.ElapsedMilliseconds);
#endif
    }

}
