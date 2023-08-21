using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Analytics;
using System.Linq;

public class FirebaseManager : MonoBehaviour
{
    [SerializeField] bool isTest;
    string logString;

    public static FirebaseManager instance;
    string name2;

    private void Awake()
    {
        if (instance == null) instance = this;
    }

    void Start()
    {

        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
            var dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventLevelStart);

                var app = FirebaseApp.DefaultInstance;
            }
            else
            {
                UnityEngine.Debug.LogError(System.String.Format(
                  "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
            }
        });

    }
    private void OnDestroy()
    {
        FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventLevelEnd);
    }
    public string HasSpecialChar(string input, string chars)
    {
        string word = string.Empty;
        string specialChar = @"|!#$%&/()=?»«@£§€{}.-;~`'<>_, ";
        foreach (var item in input)
        {
            if (specialChar.Contains(item))
            {
                word += chars;
            }
            else
            {
                word += item;
            }
        }
        return word;
    }
    public void LogEndMode(string name_)
    {
        if (Application.internetReachability == NetworkReachability.NotReachable) return;
        string name1 = HasSpecialChar(name_, "_");
        name2 = name1.Replace("_Clone", "");
        logString = "Hoan_Thanh_Mode_" + name2;
        Debug.Log("Firebase Event: " + logString);

        if (isTest) return;
        FirebaseAnalytics.LogEvent(logString);
    }
    public void LogWatchAds(string name_)
    {
        if (Application.internetReachability == NetworkReachability.NotReachable) return;
        string name1 = HasSpecialChar(name_, "_");
        name2 = name1.Replace("_Clone", "");
        logString = "Xem_Quang_cao_" + name2;
        Debug.Log("Firebase Event: " + logString);

        if (isTest) return;
        FirebaseAnalytics.LogEvent(logString);
    }
    public void LogBuyIAP(string name_)
    {
        if (Application.internetReachability == NetworkReachability.NotReachable) return;
        string name1 = HasSpecialChar(name_, "_");
        name2 = name1.Replace("_Clone", "");
        logString = "Mua_IAP_" + name2;
        Debug.Log("Firebase Event: " + logString);

        if (isTest) return;
        FirebaseAnalytics.LogEvent(logString);
    }
    public void LogBeginMode(string name_)
    {
        if (Application.internetReachability == NetworkReachability.NotReachable) return;
        string name1 = HasSpecialChar(name_, "_");
        name2 = name1.Replace("_Clone", "");
        logString = "Bat_dau_choi_" + name2;
        Debug.Log("Firebase Event: " + logString);

        if (isTest) return;
        FirebaseAnalytics.LogEvent(logString);
    }
    public void LogOpenPanel(string name_)
    {
        if (Application.internetReachability == NetworkReachability.NotReachable) return;
        string name1 = HasSpecialChar(name_, "_");
        name2 = name1.Replace("_Clone", "");
        logString = "Mo_Popup_" + name2;
        Debug.Log("Firebase Event: " + logString);

        if (isTest) return;
        FirebaseAnalytics.LogEvent(logString);
    }

}
