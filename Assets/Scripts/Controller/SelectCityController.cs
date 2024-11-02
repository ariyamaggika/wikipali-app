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
    //����ʽ������.�ڵ�һ�ε��õ�ʱ��ʵ�����Լ� 
    private SelectCityController() { }
    private static SelectCityController controller = null;
    //��̬�������� 
    public static SelectCityController Instance()
    {
        if (controller == null)
        {
            controller = new SelectCityController();
        }
        return controller;
    }
    public DBManager dbManager = DBManager.Instance();
    //���ԣ�Firstȫ����ȡ���棬second��third����ʽ�� �㵽Firstʱһ���ȡ��
    //��ȡ�󴢴����ڴ棬�´ε�ѡʱ���ж����޶�ȡ��
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
        public TimeZoneInfo timeZone;   //ʱ��
        public Dictionary<Language, string> transName;  //����������
    }
    List<FirstCityInfo> firstCityInfos = new List<FirstCityInfo>();
    //��ȡ���й���һ��������Ϣ
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
        //        //�����Լ����ԵĴʵ䣬�Ҳ�������Ӣ�ģ����Ҳ�����������
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
        Debug.LogError("�����ܡ���ѯ����һ�����к�ʱ��" + sw.ElapsedMilliseconds);
#endif
    }

    //��ȡ���й���һ��������Ϣ
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
        //        //�����Լ����ԵĴʵ䣬�Ҳ�������Ӣ�ģ����Ҳ�����������
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
        Debug.LogError("�����ܡ���ѯ����һ�����к�ʱ��" + sw.ElapsedMilliseconds);
#endif
    }

}
