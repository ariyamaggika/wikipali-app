using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
//�汾����
public class UpdateManager
{
    private UpdateManager() { }
    private static UpdateManager manager = null;
    //��̬�������� 
    public static UpdateManager Instance()
    {
        if (manager == null)
        {
            manager = new UpdateManager();
        }
        return manager;
    }
    public bool InstallApk(string apkPath)
    {
        AndroidJavaClass javaClass = new AndroidJavaClass("com.wikipali.apkupdatelibrary.Install");
        return javaClass.CallStatic<bool>("InstallApk", apkPath);
    }
    //void Start()
    //{
    //    StartCoroutine(Test());
    //}

    public IEnumerator Test()
    {
        if (!File.Exists(Application.persistentDataPath + "/a.apk"))
        {
            UnityWebRequest request = UnityWebRequest.Get(Application.streamingAssetsPath + "/a.apk");
            yield return request.SendWebRequest();
            File.WriteAllBytes(Application.persistentDataPath + "/a.apk", request.downloadHandler.data);
            InstallApk(Application.persistentDataPath + "/a.apk");
        }
        else
        {
            //print("�Ѿ����ڣ�");
        }
    }
    public UpdateInfo currentUInfo;
    public bool CheckUpdate(MonoBehaviour ui)
    {
        if (!NetworkMangaer.Instance().PingNetAddress())
        {
            UITool.ShowToastMessage(ui, "����������", 35);
            return false;
        }
        UpdateInfo uInfo = GetUpdateInfo();
        if (uInfo.version == Application.version)
        {
            UITool.ShowToastMessage(ui, "��ǰ�������°汾", 35);
            return false;
        }
        else
        {
            currentUInfo = uInfo;
            return true;
        }
    }
    public class UpdateInfo
    {
        public string version;
        public string downLoadUrl1;//����
        public string downLoadUrl2;//����
        public string updateContent;//��������
    }
    const string UPDATE_ONFO_URl_1 = "https://gitee.com/wolf96/wikipali-app/blob/main/version.txt";//����
    const string UPDATE_ONFO_URl_2 = "https://github.com/ariyamaggika/wikipali-app/blob/main/version.txt";//����

    /// <summary>
    /// ��ȡ������Ϣ
    /// ����վ�ϻ�ȡ�汾��&���ڹ������ص�ַ
    /// </summary>
    public UpdateInfo GetUpdateInfo()
    {
        UpdateInfo uInfo = new UpdateInfo();
        DownloadManager.Instance().DownLoad("", UPDATE_ONFO_URl_1, OnDownLoadVersionOver, "version.txt");
        //���ذ汾��Ϣ
        return uInfo;
    }
    object OnDownLoadVersionOver(object _realSavePath)
    {
        string realSavePath = _realSavePath.ToString();

        if (File.Exists(realSavePath))
        {
            string[] lines = null;
            lines = File.ReadAllLines(realSavePath);
            if (lines.Length >= 3)
            {
                UpdateInfo uInfo = new UpdateInfo();
                uInfo.version = lines[0];
                uInfo.downLoadUrl1 = lines[1];
                uInfo.downLoadUrl2 = lines[2];
                for (int i = 3; i < lines.Length; i++)
                {
                    uInfo.updateContent += lines[i] + "\r\n";
                }
                currentUInfo = uInfo;
            }
        }

        return null;
    }
    public void UpdateAPK()
    {
        DownloadManager.Instance().DownLoad("", "", OnDownLoadApkOver, "");
    }
    object OnDownLoadApkOver(object obj)
    {


        return null;
    }
}
