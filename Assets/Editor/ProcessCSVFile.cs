using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class ProcessCSVFile
{
    //去掉chapter文件title变量 换行
    [MenuItem("Assets/Tools/ProcessCSVChapter")]
    public static void ProcessCSV()
    {

        //支持多选
        string[] guids = Selection.assetGUIDs;//获取当前选中的asset的GUID
                                              //for (int i = 0; i < guids.Length; i++)
                                              //{
        string assetPath = AssetDatabase.GUIDToAssetPath(guids[0]);//通过GUID获取路径
                                                                   //TextAsset textAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(assetPath);

        string[] textTxt = File.ReadAllLines(assetPath);

        List<string> res = new List<string>();
        int l = textTxt.Length;
        res.Add(textTxt[0]);
        for (int i = 1; i < l; i++)
        {
            while (i < l)
            {
                if (string.IsNullOrEmpty(textTxt[i]) || textTxt[i][textTxt[i].Length - 1] != '"')
                {
                    //textTxt[i] += textTxt[i+1];
                    textTxt[i + 1] = textTxt[i] + textTxt[i + 1];
                    ++i;
                }
                else
                    break;
            }
            res.Add(textTxt[i]);
        }


        //File.WriteAllLines(assetPath, res.ToArray());
        File.WriteAllLines("Assets/Editor/chapter - bak_P.csv", res.ToArray());






        //}
    }
    //去掉csv文件某些不需要的列(变量)
    [MenuItem("Assets/Tools/FilterCSV")]
    public static void FilterCSV()
    {

        //支持多选
        string[] guids = Selection.assetGUIDs;//获取当前选中的asset的GUID
                                              //for (int i = 0; i < guids.Length; i++)
                                              //{
        string assetPath = AssetDatabase.GUIDToAssetPath(guids[0]);//通过GUID获取路径
                                                                   //TextAsset textAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(assetPath);

        string[] textTxt = File.ReadAllLines(assetPath);

        List<string> res = new List<string>();
        int l = textTxt.Length;
        for (int i = 0; i < l; i++)
        {
            string[] split = textTxt[i].Split(',');
            string resT = "";
            for (int j = 0; j < split.Length; j++)
            {
                if (j != 0 && j != 1)//tag_map
                                     //if (j != 5 && j != 7 && j != 8 && j != 10)//pali_text
                                     //if (j != 0 && j != 6 && j != 7 && j != 10 && j != 9 && j != 10 && j != 11)//sentence_translation
                {
                    //if (j != 0)
                    //if (j != 1)
                    if (j != 2)
                        resT += ("," + split[j]);
                    else
                        resT += split[j];
                }
            }
            res.Add(resT);
        }


        //File.WriteAllLines(assetPath, res.ToArray());
        File.WriteAllLines("Assets/Editor/tag_map - bak_P.csv", res.ToArray());






        //}
    }
    //输出日期文字格式
    [MenuItem("Assets/Tools/PrintDateString")]
    public static void PrintDateString()
    {
        Debug.LogError(DateTime.Now.ToString());
    }
    //json转class文字
    [MenuItem("Assets/Tools/PrintJson2Class")]
    public static void PrintJson2Class()
    {
        string str = "{\r\n  \"ok\": true,\r\n  \"message\": {\r\n    \"title\": \"string\",\r\n    \"toc\": \"string\",\r\n    \"book\": \"string\",\r\n    \"para\": \"string\",\r\n    \"path\": \"string\",\r\n    \"tags\": {\r\n      \"id\": \"string\",\r\n      \"name\": \"string\",\r\n      \"description\": \"string\"\r\n    },\r\n    \"channel\": {\r\n      \"name\": \"string\",\r\n      \"owner_uid\": \"string\"\r\n    },\r\n    \"studio\": {\r\n      \"id\": \"string\",\r\n      \"nickName\": \"string\",\r\n      \"studioName\": \"string\",\r\n      \"realName\": \"string\",\r\n      \"avatar\": \"string\"\r\n    },\r\n    \"channel_id\": \"string\",\r\n    \"summary\": \"string\",\r\n    \"view\": 0,\r\n    \"like\": 0,\r\n    \"status\": 0,\r\n    \"progress\": 0,\r\n    \"progress_line\": 0,\r\n    \"created_at\": \"string\",\r\n    \"updated_at\": \"string\"\r\n  }\r\n}";
        string[] strSplit = str.Split("\r\n");
        string res = "";
        for (int i = 0; i < strSplit.Length; i++)
        {
            if (strSplit[i].Contains(":"))
            {
                string[] childStrSplit = strSplit[i].Split(":");
                string name = childStrSplit[0].Replace("\"", "").Replace(",", "");
                string typeName = childStrSplit[1].Replace("\"", "");
                if (typeName.Contains("0"))
                    res += "public int" + name + ";";
                else if (typeName.Contains("true") || typeName.Contains("false"))
                    res += "public bool" + name + ";";
                else
                    res += "public " + typeName + name + ";";
            }
            else
                res += strSplit[i];
            res += "\r\n";
        }

        Debug.LogError(res);
    }

}
