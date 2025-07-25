using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using CI.HttpClient;
using UnityEngine;
using static C2SArticleGetNewDBInfo;

public class C2SGetTimeZone
{
    //https://learn.microsoft.com/en-us/rest/api/maps/timezone/get-timezone-by-coordinates?view=rest-maps-2025-01-01&tabs=HTTP
    //GET https://atlas.microsoft.com/timezone/byCoordinates/json?api-version=1.0&query={query}
    public static void GetCommunityDicDataAzure(string word, Func<string, object> callback)
    {


    }
    [Serializable]
    public class TimeZoneJsonData
    {
        public float longitude;
        public float latitude;
        public string location;
        public string country_iso;
        public string iana_timezone;
        public string timezone_abbreviation;
        public string dst_abbreviation;
        public string offset;
        public string dst_offset;
        public DateTime current_local_datetime;
        public DateTime current_utc_datetime;
    }
    //{"longitude":24.0,"latitude":65.0,"location":null,"country_iso":null,"iana_timezone":null,"timezone_abbreviation":null,"dst_abbreviation":null,"offset":"UTC+2","dst_offset":null,"current_local_datetime":"2025-07-16T01:24:48.389","current_utc_datetime":"2025-07-15T23:24:48.389Z"}
    //https://api.geotimezone.com/public/timezone?latitude=65&longitude=24
    public static void GetCommunityDicData(float lat, float lon, Func<TimeZoneJsonData,object> callback)
    {
        HttpClient client = new HttpClient();
        Debug.LogError(string.Format(@"https://api.geotimezone.com/public/timezone?latitude={0}&longitude={1}", lat, lon));
        client.Get(new System.Uri(string.Format(@"https://api.geotimezone.com/public/timezone?latitude={0}&longitude={1}", lat, lon)),
            HttpCompletionOption.StreamResponseContent, (r) =>
            {
                byte[] responseData = r.ReadAsByteArray();
                string json = Encoding.UTF8.GetString(responseData);
                Debug.LogError(json);
                TimeZoneJsonData timeZoneData = JsonUtility.FromJson<TimeZoneJsonData>(json);
                callback(timeZoneData);
            });

    }
}
