using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;
using static ArticleController;
using static SpeechManager;

public class CommonTool
{
    #region 宏
    public static string COLOR_BLUE = "#5895FF";
    public static string COLOR_BLACK = "#1B1B1B";
    public static string COLOR_BROWN = "#A52A2A";
    public static string COLOR_DARK_BLUE = "#005794";
    public static string COLOR_DARK_BLUE2 = "#0039E5";
    public static string COLOR_DARK_YELLOW = "#E3C822";
    public static string COLOR_DARK_GREEN = "#3ABA1F";

    public static string COLOR_BLUE_FLAG = "<color=#5895FF>";
    public static string COLOR_BLACK_FLAG = "<color=#1B1B1B>";
    public static string COLOR_BROWN_FLAG = "<color=#A52A2A>";

    public static string COLOR_END_FLAG = "</color>";

    public static string TPM_RICHTEXT_LINK_FLAG = "<link=\"{0}\">{1}</link>";
    public static string RICHTEXT_COLOR_FLAG = "<color={0}>{1}</color>";
    #endregion

    //返回翻译标题
    //todo：不同语言 标题不同
    public static string GetBookTranslateName(Book book)
    {
        if (string.IsNullOrEmpty(book.translateName))
            return book.toc;
        else
            return book.translateName;
    }


    public static string CopyAndroidPathToPersistent(string datebasePath)
    {

        string path;


        path = Application.persistentDataPath + "/" + datebasePath;

        //如果查找该文件路径
        if (File.Exists(path))
        {
            Debug.LogError("找到了！" + path);
            //返回该数据库路径
            return path;
        }
        //Debug.LogError("1111111111wwww");
        //Debug.LogError("复制数据库路径" + "jar:file://" + Application.dataPath + "!/assets/" + datebasePath);

        // jar:file:是安卓手机路径的意思  
        // Application.dataPath + "!/assets/"   即  Application.dataPath/StreamingAssets  
        var request = UnityWebRequest.Get("jar:file://" + Application.dataPath + "!/assets/" + datebasePath);
        request.SendWebRequest(); //读取数据
        while (!request.downloadHandler.isDone) { }
        // 因为安卓中streamingAssetsPath路径下的文件权限是只读，所以获取之后把他拷贝到沙盘路径中
        File.WriteAllBytes(path, request.downloadHandler.data);
        //Debug.LogError("复制完了" + path);
        //Debug.LogError("22222222222wwww");

        return path;
    }

    public static string CopyIOSPathToPersistent(string datebasePath)
    {

        string path;


        path = Application.persistentDataPath + "/" + datebasePath;

        //如果查找该文件路径
        if (File.Exists(path))
        {
            Debug.LogError("找到了！" + path);
            //返回该数据库路径
            return path;
        }

        File.Copy(Application.streamingAssetsPath + "/" + datebasePath, path);

        return path;
    }

    /// <summary>
    /// 对相机截图
    /// </summary>
    /// <param name="camera">Camera.要被截屏的相机</param>
    /// <param name="rect">Rect.截屏的区域</param>
    /// <returns>The screenshot2.</returns>
    public static Texture2D CaptureCamera(Camera camera, Rect rect, Vector3 cameraPos)
    {
        camera.transform.position = cameraPos;
        RenderTexture rt = new RenderTexture((int)rect.width, (int)rect.height, 0);//创建一个RenderTexture对象
        camera.targetTexture = rt;//临时设置相关相机的targetTexture为rt, 并手动渲染相关相机
        camera.Render();
        //ps: --- 如果这样加上第二个相机，可以实现只截图某几个指定的相机一起看到的图像。
        //ps: camera2.targetTexture = rt;
        //ps: camera2.Render();
        //ps: -------------------------------------------------------------------

        RenderTexture.active = rt;//激活这个rt, 并从中中读取像素。
        Texture2D screenShot = new Texture2D((int)rect.width, (int)rect.height, TextureFormat.RGB24, false);
        screenShot.ReadPixels(rect, 0, 0);//注：这个时候，它是从RenderTexture.active中读取像素
        screenShot.Apply();

        //重置相关参数，以使用camera继续在屏幕上显示
        camera.targetTexture = null;
        //ps: camera2.targetTexture = null;

        RenderTexture.active = null; //JC: added to avoid errors
        GameObject.Destroy(rt);

        //byte[] bytes = screenShot.EncodeToPNG();//最后将这些纹理数据，成一个png图片文件
        //string filename = Application.dataPath + "/Screenshot.png";
        //System.IO.File.WriteAllBytes(filename, bytes);
        //Debug.Log(string.Format("截屏了一张照片: {0}", filename));

        return screenShot;
    }

    //todo ios代码
    /// <summary>
    /// 保存图片
    /// </summary>
    /// <param name="texture"></param>
    /// <returns></returns>
    public static string SaveImages(Texture2D texture)
    {
        string path = Application.streamingAssetsPath;
#if UNITY_EDITOR
        path = Application.dataPath;
#endif
#if UNITY_ANDROID && !UNITY_EDITOR
            path = "/sdcard/DCIM/Camera"; //设置图片保存到设备的目录.
#endif
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
        string savePath = path + "/" + texture.name + ".png";
        try
        {
            Application.HasUserAuthorization(UserAuthorization.Microphone);
            //byte[] data = DeCompress(texture).EncodeToPNG();
            byte[] data = texture.EncodeToPNG();
            File.WriteAllBytes(savePath, data);
            OnSaveImagesPlartform(savePath);
        }
        catch
        {
        }
        return savePath;
    }
    public static string SaveImages2PersistentDataPath(Texture2D texture)
    {
        string path = Application.persistentDataPath;
        //if (!Directory.Exists(path))
        //    Directory.CreateDirectory(path);
        string savePath = path + "/" + texture.name + ".png";
        try
        {
            Application.HasUserAuthorization(UserAuthorization.Microphone);
            //byte[] data = DeCompress(texture).EncodeToPNG();
            byte[] data = texture.EncodeToPNG();
            File.WriteAllBytes(savePath, data);
            OnSaveImagesPlartform(savePath);
        }
        catch
        {
        }
        return savePath;
    }
    /// <summary>
    /// 刷新相册（不需要单独创建原生aar或jar）
    /// </summary>
    /// <param name="path"></param>
    static void OnSaveImagesPlartform(string filePath)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
            string[] paths = new string[1];
            paths[0] = filePath; 
            using (AndroidJavaClass PlayerActivity = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                AndroidJavaObject playerActivity = PlayerActivity.GetStatic<AndroidJavaObject>("currentActivity");
                using (AndroidJavaObject Conn = new AndroidJavaObject("android.media.MediaScannerConnection", playerActivity, null))
                {
                    Conn.CallStatic("scanFile", playerActivity, paths, null, null);
                }
            }
#endif
    }
    /// <summary>
    /// 压缩图片
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    static Texture2D DeCompress(Texture2D source)
    {
        RenderTexture renderTex = RenderTexture.GetTemporary(
                    source.width,
                    source.height,
                    0,
                    RenderTextureFormat.Default,
                    RenderTextureReadWrite.Linear);

        Graphics.Blit(source, renderTex);
        RenderTexture previous = RenderTexture.active;
        RenderTexture.active = renderTex;
        Texture2D readableText = new Texture2D(source.width, source.height);
        readableText.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
        readableText.Apply();
        RenderTexture.active = previous;
        RenderTexture.ReleaseTemporary(renderTex);
        return readableText;
    }
    /// <summary>
    /// 从外部指定文件中加载图片
    /// </summary>
    /// <returns></returns>
    public static Texture2D LoadTextureByIO(string path)
    {
        FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
        fs.Seek(0, SeekOrigin.Begin);//游标的操作，可有可无
        byte[] bytes = new byte[fs.Length];//生命字节，用来存储读取到的图片字节
        try
        {
            fs.Read(bytes, 0, bytes.Length);//开始读取，这里最好用trycatch语句，防止读取失败报错

        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
        fs.Close();//切记关闭

        int width = 2048;//图片的宽（这里两个参数可以提到方法参数中）
        int height = 2048;//图片的高（这里说个题外话，pico相关的开发，这里不能大于4k×4k不然会显示异常，当时开发pico的时候应为这个问题找了大半天原因，因为美术给的图是6000*3600，导致出现切几张图后就黑屏了。。。
        Texture2D texture = new Texture2D(width, height);
        if (texture.LoadImage(bytes))
        {
            //print("图片加载完毕 ");
            return texture;//将生成的texture2d返回，到这里就得到了外部的图片，可以使用了

        }
        else
        {
            //print("图片尚未加载");
            return null;
        }
    }

    //交换数组奇偶位
    public static byte[] SwapArray(byte[] bytes)
    {
        for (int i = 0; i < bytes.Length; i += 2)
        {
            if (i + 1 < bytes.Length)
            {
                byte temp = bytes[i];
                bytes[i] = bytes[i + 1];
                bytes[i + 1] = temp;
            }
        }
        return bytes;
    }
    //交换string奇偶位
    public static string SwapString(string str)
    {
        char[] chars = str.ToCharArray();
        for (int i = 0; i < chars.Length; i += 2)
        {
            if (i + 1 < chars.Length)
            {
                char temp = chars[i];
                chars[i] = chars[i + 1];
                chars[i + 1] = temp;
            }
        }
        str = new string(chars);
        return str;
    }

    #region 加密解密 序列化文件
    /// <summary>
    /// 把对象序列化到文件(AES加密)
    /// </summary>
    /// <param name="keyString">密钥(16位)</param>
    public static void SerializeObjectToFile(string fileName, object obj, string keyString)
    {
        using (AesCryptoServiceProvider crypt = new AesCryptoServiceProvider())
        {
            crypt.Key = Encoding.ASCII.GetBytes(keyString);
            crypt.IV = Encoding.ASCII.GetBytes(keyString);
            using (ICryptoTransform transform = crypt.CreateEncryptor())
            {
                FileStream fs = new FileStream(fileName, FileMode.Create);
                using (CryptoStream cs = new CryptoStream(fs, transform, CryptoStreamMode.Write))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(cs, obj);
                }
            }
        }
    }
    /// <summary>
    /// 把文件反序列化成对象(AES加密)
    /// </summary>
    /// <param name="keyString">密钥(16位)</param>
    public static object DeserializeObjectFromFile(string fileName, string keyString)
    {
        using (AesCryptoServiceProvider crypt = new AesCryptoServiceProvider())
        {
            crypt.Key = Encoding.ASCII.GetBytes(keyString);
            crypt.IV = Encoding.ASCII.GetBytes(keyString);
            using (ICryptoTransform transform = crypt.CreateDecryptor())
            {
                FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                using (CryptoStream cs = new CryptoStream(fs, transform, CryptoStreamMode.Read))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    object obj = formatter.Deserialize(cs);
                    return obj;
                }
            }
        }
    }
    #endregion
    //https://discussions.unity.com/t/base64-encode-decoding/12507
    public static string DecodeBase64(string encodedText)
    {
        byte[] decodedBytes = Convert.FromBase64String(encodedText);
        string decodedText = Encoding.UTF8.GetString(decodedBytes);
        return decodedText;
    }

    public static void DeepCopyStringList(List<string> origin, List<string> copy)
    {
        if (origin == null || copy == null)
            return;
        origin.Clear();
        int c = copy.Count;
        for (int i = 0; i < c; i++)
        {
            origin.Add(copy[i]);
        }
    }

    public static class ByteToJsonUtil
    {
        public static string ByteToJson(byte[] bytes)
        {
            if (bytes == null)
            {
                return string.Empty;
            }
            return JsonConvert.SerializeObject(bytes);
        }
        //public static byte[] JsonToByte(string json)
        //{
        //    if (string.IsNullOrEmpty(json))
        //    {
        //        return null;
        //    }
        //    return JsonConvert.DeserializeObject(json);
        //}
    }
    public static byte[] ObjectToByteArray(object obj)
    {
        BinaryFormatter bf = new BinaryFormatter();
        using (var ms = new MemoryStream())
        {
            bf.Serialize(ms, obj);
            return ms.ToArray();
        }
    }

    //判断首字母是否是中文
    public static bool CheckStringIsChinese(string str)
    {
        if (string.IsNullOrEmpty(str))
            return false;
        if (str[0] >= 0x4e00 && str[0] <= 0x9fbb)
            return true;
        return false;
    }
    //判断首字母是否是缅语
    public static bool CheckStringIsMyanmar(string str)
    {
        if (string.IsNullOrEmpty(str))
            return false;
        if (str[0] >= 0x1000 && str[0] <= 0x109F)
            return true;
        return false;
    }
    //通过GPS定位判断是否在国内
    //检查经度是否在中国的经度范围内，即判断经度是否在 73.66 度到 135.05 度之间。
    //检查纬度是否在中国的纬度范围内，即判断纬度是否在 3.86 度到 53.55 度之间。
    public static bool CheckGPSIsInChina()
    {
        float longitude = Input.location.lastData.longitude;
        float latitude = Input.location.lastData.latitude;
        if (longitude == 0 && latitude == 0)
            return true;
        if ((longitude >= 73.66f && longitude <= 135.05f) && (latitude >= 3.86f && latitude <= 53.55f))
            return true;
        else
            return false;
    }

    public static Color32 TintRGB(Color32 c1, float tint)
    {
        byte r = (byte)(Mathf.Clamp(c1.r / 255f * tint * 255, 0, 255));
        byte g = (byte)(Mathf.Clamp(c1.g / 255f * tint * 255, 0, 255));
        byte b = (byte)(Mathf.Clamp(c1.b / 255f * tint * 255, 0, 255));
        //byte a = (byte)(Mathf.Clamp(c1.a / 255f * tint * 255, 0, 255));

        return new Color32(r, g, b, c1.a);
    }
    public static Color32 TintUpArticleColorRGB(Color32 c1, float tint)
    {
        //红字
        byte r = (byte)(Mathf.Clamp(c1.r / 255f * tint * 255, 0, 255));
        byte g = (byte)(Mathf.Clamp(c1.g / 255f * tint * 255, 0, 255));
        byte b = (byte)(Mathf.Clamp(c1.b / 255f * tint * 255, 0, 255));
        //黑字
        if (c1.r < 40)
        {
            r = (byte)(Mathf.Clamp(c1.r + 150, 0, 255));
            g = (byte)(Mathf.Clamp(c1.g + 150, 0, 255));
            // b = (byte)(Mathf.Clamp(c1.b / 255f * tint * 255, 0, 255));
        }


        //byte a = (byte)(Mathf.Clamp(c1.a / 255f * tint * 255, 0, 255));

        return new Color32(r, g, b, c1.a);
    }
    public static Color32 TintDownArticleColorRGB(Color32 c1, float tint)
    {
        //红字
        byte r = (byte)(Mathf.Clamp(c1.r / 255f * tint * 255, 0, 255));
        byte g = (byte)(Mathf.Clamp(c1.g / 255f * tint * 255, 0, 255));
        byte b = (byte)(Mathf.Clamp(c1.b / 255f * tint * 255, 0, 255));
        //黑字
        if (c1.g > 140)
        {
            r = (byte)(Mathf.Clamp(c1.r - 150, 0, 255));
            g = (byte)(Mathf.Clamp(c1.g - 150, 0, 255));
            // b = (byte)(Mathf.Clamp(c1.b / 255f * tint * 255, 0, 255));
        }

        return new Color32(r, g, b, c1.a);
    }
    #region 获取时间距离
    public static string DateFormatToString(DateTime dt)

    {

        //TimeSpan表示时间间隔

        TimeSpan span = (DateTime.Now - dt).Duration();//表示取timespan绝对值
        if (span.TotalDays > 60)
        {
            return dt.ToString("yyyy-MM-dd");
        }
        else if (span.TotalDays > 30)
        {
            return "1个月前";
        }
        else if (span.TotalDays > 14)
        {
            return "2周前";
        }
        else if (span.TotalDays > 7)
        {
            return "1周前";
        }
        else if (span.TotalDays > 1)
        {
            return string.Format("{0}天前", (int)Math.Floor(span.TotalDays));
        }
        else if (span.TotalHours > 1)
        {
            return string.Format("{0}小时前", (int)Math.Floor(span.TotalHours));
        }
        else if (span.TotalMinutes > 1)
        {
            return string.Format("{0}分钟前", (int)Math.Floor(span.TotalMinutes));
        }
        else if (span.TotalSeconds >= 1)
        {
            return string.Format("{0}秒前", (int)Math.Floor(span.TotalSeconds));
        }
        else
        {
            return "1秒前";
        }

    }
    public static string FormatDate(DateTime dt)
    {
        var byTime = new long[] { 24 * 60 * 60, 60 * 60, 60, 1 };
        var unit = new string[] { "天", "小时", "分钟", "秒" };

        var ct = (DateTime.Now - dt).TotalSeconds;
        if (ct < 0)
        {
            return "";
        }

        var sb = new System.Text.StringBuilder();
        for (var i = 0; i < byTime.Length; i++)
        {
            if (ct < byTime[i])
            {
                continue;
            }
            var temp = Math.Floor(ct / byTime[i]);
            ct = ct % byTime[i];
            if (temp > 0)
            {
                sb.Append(temp + unit[i]);
            }


            /*一下控制最多输出几个时间单位：
                一个时间单位如：N分钟前
                两个时间单位如：M分钟N秒前
                三个时间单位如：M年N分钟X秒前
            以此类推
            */
            if (sb.Length >= 1)
            {
                break;
            }
        }
        return sb.ToString() + "前";
    }
    #endregion

    #region AES加密 string 复制口令使用
    //打开文章编码格式:bookID_bookParagraph_bookChapterLen_channelId)
    //int bookID, int bookParagraph, int bookChapterLen, string channelId)
    //口令格式:复制整句话[sdhfuw8rekdkhfhoiedf24]后打开wikipaliApp跳转到文章【文章标题。。。。】+网页版网址
    //口令格式:复制整句话[sdhfuw8rekdkhfhoiedf24]后打开wikipaliApp跳转到词典【单词名。。。。】+网页版网址
    public const string AES_STRING_KEY = "wpa_copy_command";
    public const string AES_STRING_IV = "wpa_copy_command";
    //字符串转字节数组
    private static byte[] StringToBytes(string str, int length)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(str);
        Array.Resize(ref bytes, length);
        return bytes;
    }

    //字节数组转字符串
    private static string BytesToString(byte[] bytes)
    {
        string str = Encoding.UTF8.GetString(bytes);
        return str;
    }

    //加密-String转String
    private static string Encrypt(string str, string key, string iv)
    {
        return Convert.ToBase64String(Encrypt(StringToBytes(str, str.Length + 4), key, iv));//注意长度+4
    }

    //加密-Bytes转Bytes
    private static byte[] Encrypt(byte[] bytes, string key, string iv)
    {
        using (Aes aes = Aes.Create())
        {
            aes.Key = StringToBytes(key, 32);
            aes.IV = StringToBytes(iv, 16);

            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            byte[] encryptedBytes;

            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    csEncrypt.Write(bytes, 0, bytes.Length);
                }
                encryptedBytes = msEncrypt.ToArray();
            }
            return encryptedBytes;
        }
    }

    //解密-String转String
    private static string Decrypt(string str, string key, string iv)
    {
        return BytesToString(Decrypt(Convert.FromBase64String(str), key, iv));
    }

    //解密-Bytes转Bytes
    private static byte[] Decrypt(byte[] bytes, string key, string iv)
    {
        using (Aes aes = Aes.Create())
        {
            aes.Key = StringToBytes(key, 32);
            aes.IV = StringToBytes(iv, 16);

            ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

            using (MemoryStream msDecrypt = new MemoryStream(bytes))
            {
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    using (MemoryStream msOutput = new MemoryStream())
                    {
                        csDecrypt.CopyTo(msOutput);
                        return msOutput.ToArray();
                    }
                }
            }
        }
    }

    //外部接口
    public static string Value2Token(string value)
    {
        return Encrypt(value, AES_STRING_KEY, AES_STRING_IV);
    }
    public static string Token2Value(string token)
    {
        return Decrypt(token, AES_STRING_KEY, AES_STRING_IV);
    }
    static Regex reg = new Regex(@"\[(.+?)\]");

    public static string GetArticleCommandByValue(int bookID, int bookParagraph, int bookChapterLen, string channelId, string title)
    {
        string temp = bookID + "_" + bookParagraph + "_" + bookChapterLen + "_" + channelId;
        string token = Value2Token(temp);
        string res = string.Format("复制整句话[{0}]后打开wikipaliApp跳转到文章【{1}】", token, title);
        return res;
    }
    public static string GetDicCommandByValue(string word)
    {
        string token = Value2Token(word);
        string res = string.Format("复制整句话[{0}]后打开wikipaliApp跳转到词典【{1}】", token, word);
        return res;
    }
    public static string GetValueByCommand(string command)
    {
        MatchCollection mcs = reg.Matches(command);
        Match[] mArr = mcs.ToArray();
        if (mArr == null || mArr.Length == 0)
            return "";

        string value = mArr[0].Value.Replace("[", "");
        value = value.Replace("]", "");

        value = Token2Value(value);
        //去除解码后的NUL值
        value = value.Replace("\0", "");

        return value;
    }
    #endregion
    #region 剪切板操作
    /// <summary>
    /// 写入剪切板
    /// </summary>
    public static void WriteToClipboard(string str)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidJavaObject androidObject = new AndroidJavaObject("com.wikipali.apkupdatelibrary.ClipboardTools");
        AndroidJavaObject activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
        if (activity == null)
        {
            Debug.LogError("activity == null");
            return ;
        }
        // 复制到剪贴板
        //androidObject.Call("copyTextToClipboard", activity, str);
        androidObject.CallStatic("copyTextToClipboard", activity, str);
#elif UNITY_EDITOR

#endif

    }
    public static string GetClipboard()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidJavaObject androidObject = new AndroidJavaObject("com.wikipali.apkupdatelibrary.ClipboardTools");
        AndroidJavaObject activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
        if (activity == null)
        {
            Debug.LogError("activity == null");
            return "";
        }
        // 从剪贴板中获取文本
        // string text = androidObject.Call<string>("getTextFromClipboard");
        string text = androidObject.CallStatic<string>("getTextFromClipboard");
        return text;
#elif UNITY_EDITOR

#endif
        return "";
    }

    #endregion
    #region string转码
    //转为UTF-8
    public static string ToUTF8(string input)
    {
        if (string.IsNullOrEmpty(input))
            return "";
        byte[] bytes = Encoding.Default.GetBytes(input);
        string output = Encoding.UTF8.GetString(bytes);
        return output;
    }
    #endregion
    #region 计算时区相关
    /* 根据经度获取时区；例如121：+8;-121：-8;返回值为字符串（返回正数时候带+符号）
    *https://download.csdn.net/blog/column/11112744/124842449
    * @param currentLon
    * @return
    */
    public static int CaculateTimeZone(float currentLon)
    {
        int timeZone;
        int shangValue = (int)(currentLon / 15);
        float yushuValue = Math.Abs(currentLon % 15);
        if (yushuValue <= 7.5)
        {
            timeZone = shangValue;
        }
        else
        {
            timeZone = shangValue + (currentLon > 0 ? 1 : -1);
        }
        return timeZone >= 0 ? Math.Abs(timeZone) : -Math.Abs(timeZone);
    }

    #endregion
}
