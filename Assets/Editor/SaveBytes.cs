using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using UnityEditor;
using UnityEngine;
using ZXing.Common;
using static C2SArticleGetNewDBInfo;
//保存二进制文件的工具
public class SaveBytes : MonoBehaviour
{
    //key做加密，所以转换后的bytes所有奇数位偶数位互换，读取后也要互换
    [MenuItem("Assets/Tools/SaveBytes")]
    public static void SaveBytesFile()
    {
        string str = "05ac08cbb3f04141bd987078441c5526,southeastasia";
        str = CommonTool.SwapString(str);
        //todo 隐藏密码
        CommonTool.SerializeObjectToFile(@"Assets/Editor/font.font", str, "wikipaliapp12345");
    }

    [MenuItem("Assets/Tools/LoadBytes")]
    public static void LoadBytesFile()
    {

        string str = (string)CommonTool.DeserializeObjectFromFile("Assets/Editor/font.font", "wikipaliapp12345");
        str = CommonTool.SwapString(str);
        Debug.LogError(str);
    }
    [MenuItem("Assets/Tools/SwapString")]
    public static void SwapString()
    {
        string str = "s2nQrKq=ah@";
        str = CommonTool.SwapString(str);
        Debug.LogError(str);
    }
    [MenuItem("Assets/Tools/Test")]
    public static void Test()
    {
        string str = "{{term|eyJ3b3JkIjoiYmhhZ2F2YW50dSIsInBhcmVudENoYW5uZWxJZCI6IjAwYWUyYzQ4LWMyMDQtNDA4Mi1hZTc5LTc5YmEyNzQwZDUwNiIsInBhcmVudFN0dWRpb0lkIjoiYmE1NDYzZjMtNzJkMS00NDEwLTg1OGUtZWFkZDEwODg0NzEzIiwiaWQiOiJmMThjY2Q2OS0wZjNiLTQ0MzMtODUxYy05MmUxMjA2ZmNkNWIiLCJtZWFuaW5nIjoiXHU0ZTE2XHU1YzBhIiwiY2hhbm5lbCI6IjAwYWUyYzQ4LWMyMDQtNDA4Mi1hZTc5LTc5YmEyNzQwZDUwNiIsImlubmVySHRtbCI6Ilx1NGUxNlx1NWMwYShiaGFnYXZhbnR1KSJ9}}123{{term|eyJ3b3JkIjoiYmhhZ2F2YW50dSIsInBhcmVudENoYW5uZWxJZCI6IjAwYWUyYzQ4LWMyMDQtNDA4Mi1hZTc5LTc5YmEyNzQwZDUwNiIsInBhcmVudFN0dWRpb0lkIjoiYmE1NDYzZjMtNzJkMS00NDEwLTg1OGUtZWFkZDEwODg0NzEzIiwiaWQiOiJmMThjY2Q2OS0wZjNiLTQ0MzMtODUxYy05MmUxMjA2ZmNkNWIiLCJtZWFuaW5nIjoiXHU0ZTE2XHU1YzBhIiwiY2hhbm5lbCI6IjAwYWUyYzQ4LWMyMDQtNDA4Mi1hZTc5LTc5YmEyNzQwZDUwNiIsImlubmVySHRtbCI6Ilx1NGUxNlx1NWMwYShiaGFnYXZhbnR1KSJ9}}456{{term|eyJ3b3JkIjoiYmhhZ2F2YW50dSIsInBhcmVudENoYW5uZWxJZCI6IjAwYWUyYzQ4LWMyMDQtNDA4Mi1hZTc5LTc5YmEyNzQwZDUwNiIsInBhcmVudFN0dWRpb0lkIjoiYmE1NDYzZjMtNzJkMS00NDEwLTg1OGUtZWFkZDEwODg0NzEzIiwiaWQiOiJmMThjY2Q2OS0wZjNiLTQ0MzMtODUxYy05MmUxMjA2ZmNkNWIiLCJtZWFuaW5nIjoiXHU0ZTE2XHU1YzBhIiwiY2hhbm5lbCI6IjAwYWUyYzQ4LWMyMDQtNDA4Mi1hZTc5LTc5YmEyNzQwZDUwNiIsImlubmVySHRtbCI6Ilx1NGUxNlx1NWMwYShiaGFnYXZhbnR1KSJ9}}789{{term|eyJ3b3JkIjoiYmhhZ2F2YW50dSIsInBhcmVudENoYW5uZWxJZCI6IjAwYWUyYzQ4LWMyMDQtNDA4Mi1hZTc5LTc5YmEyNzQwZDUwNiIsInBhcmVudFN0dWRpb0lkIjoiYmE1NDYzZjMtNzJkMS00NDEwLTg1OGUtZWFkZDEwODg0NzEzIiwiaWQiOiJmMThjY2Q2OS0wZjNiLTQ0MzMtODUxYy05MmUxMjA2ZmNkNWIiLCJtZWFuaW5nIjoiXHU0ZTE2XHU1YzBhIiwiY2hhbm5lbCI6IjAwYWUyYzQ4LWMyMDQtNDA4Mi1hZTc5LTc5YmEyNzQwZDUwNiIsImlubmVySHRtbCI6Ilx1NGUxNlx1NWMwYShiaGFnYXZhbnR1KSJ9}}101112";
        Debug.LogError(ArticleMarkdownManager.Instance().PrefilterSentenceSent(str));

    }
    [MenuItem("Assets/Tools/TestHttpC2S")]
    public static void TestHttpC2S()
    {
        //  System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        // sw.Start();
        // C2SDicGetInfo.GetCommunityDicData("eka", (json) => { sw.Stop(); Debug.LogError("【性能】耗时：" + sw.ElapsedMilliseconds); C2SCallback(json); return null; });
        C2SArticleGetNewDBInfo.GetSentenceData(66, "7fea264d-7a26-40f8-bef7-bc95102760fb",1182,1183, null);

    }
    public static void C2SCallback(string json)
    {
        ;

    }
}
