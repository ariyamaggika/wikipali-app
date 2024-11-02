using System;
using System.Collections;
using System.Collections.Generic;
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
        public string name;
        public string level;
        public string pName;
        public string fullName;
        public float lng;
        public float lat;
        public TimeZoneInfo timeZone;   //时区
        public Dictionary<Language, string> transName;  //翻译后的名字
    }
    List<FirstCityInfo> firstCityInfos = new List<FirstCityInfo>();
    //获取所有国内一级城市信息
    public void GetAllDomesticFirstCity()
    {
        firstCityInfos.Clear();
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

    //获取所有国外一级城市信息
    public void GetAllInternationalFirstCity()
    {
        firstCityInfos.Clear();
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

}
