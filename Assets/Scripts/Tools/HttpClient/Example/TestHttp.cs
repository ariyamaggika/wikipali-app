using CI.HttpClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using ZXing.Common;

public class TestHttp : MonoBehaviour
{
    public Button button;
    public InputField inputField;
    // Start is called before the first frame update
    void Start()
    {
        //GetChannel();
        //PostChannel();
        button.onClick.AddListener(OnBtnClick);
    }
    void OnBtnClick()
    {
        Debug.LogError(CommonTool.CaculateTimeZone(float.Parse(inputField.text)));
    }
    // Update is called once per frame
    void Update()
    {

    }
    public void GetChannel()
    {
        HttpClient client = new HttpClient();

        //ProgressSlider.value = 100;
        //client.Get(new System.Uri("http://staging.wikipali.org/api/v2/channel-io"), HttpCompletionOption.StreamResponseContent, (r) =>
        client.Get(new System.Uri("https://staging.wikipali.org/api/v2/channel-io?view=public&limit=30&offset=0"), HttpCompletionOption.StreamResponseContent, (r) =>
        {
            //RightText.text = "Download: " + r.PercentageComplete.ToString() + "%";
            //ProgressSlider.value = 100 - r.PercentageComplete;
            byte[] responseData = r.ReadAsByteArray();
            //!!!以前是Encoding.Default.GetString(responseData);在MAC/IOS苹果机器上中文会出现乱码，所以改成UTF8格式
            string json = Encoding.UTF8.GetString(responseData);
            // string json = CommonTool.ByteToJsonUtil.ByteToJson(responseData);
            Debug.LogError(json);
            //תΪjson
        });
    }
    [Serializable]
    public class TestP
    {
        public string view = "public";
        public string update_at = "";
        public int limit = 30;
        int offset = 0;
    }
    public void PostChannel()
    {
        HttpClient client = new HttpClient();
        TestP t = new TestP();

        byte[] buffer = CommonTool.ObjectToByteArray(t);
        //new System.Random().NextBytes(buffer);

        ByteArrayContent content = new ByteArrayContent(buffer, "application/bytes");

        client.Post(new System.Uri("http://staging.wikipali.org/api/v2/channel-io"), content, HttpCompletionOption.StreamResponseContent, (r) =>
        {
            byte[] responseData = r.ReadAsByteArray();
            string json = CommonTool.ByteToJsonUtil.ByteToJson(responseData);
            Debug.LogError(json);
        }, (u) =>
        {
            //LeftText.text = "Upload: " + u.PercentageComplete.ToString() + "%";
            //ProgressSlider.value = u.PercentageComplete;
        });
    }
}
