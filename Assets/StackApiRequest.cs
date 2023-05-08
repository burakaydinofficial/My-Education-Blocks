using System;
using System.Collections;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

public class StackApiRequest
{
    [Serializable]
    public class StackApiDataElement
    {
        public int id { get; set; }
        public string subject { get; set; }
        public string grade { get; set; }
        public int mastery { get; set; }
        public string domainid { get; set; }
        public string domain { get; set; }
        public string cluster { get; set; }
        public string standardid { get; set; }
        public string standarddescription { get; set; }
    }


    private const string _apiUrl = "https://ga1vqcu3o1.execute-api.us-east-1.amazonaws.com/Assessment/stack";
    public bool Started = false;
    public bool Finished = false;
    public bool Failed = false;

    public bool Success => Started && Finished && !Failed && Data != null;

    public StackApiDataElement[] Data;
    public IEnumerator Start()
    {
        Failed = false;
        Finished = false;
        Data = null;

        Started = true;
        using (UnityWebRequest webRequest = UnityWebRequest.Get(_apiUrl))
        {
            yield return webRequest.SendWebRequest();
            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                try
                {
                    var json = webRequest.downloadHandler.text;
                    Data = JsonConvert.DeserializeObject<StackApiDataElement[]>(json);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                    Failed = true;
                }
            }
            else
            {
                Failed = true;
            }
        }
        Finished = true;
    }
}