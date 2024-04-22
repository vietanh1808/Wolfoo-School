using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    }
    private void OnDestroy()
    {
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
    }
    public void LogWatchAds(string name_)
    {
        if (Application.internetReachability == NetworkReachability.NotReachable) return;
        string name1 = HasSpecialChar(name_, "_");
        name2 = name1.Replace("_Clone", "");
        logString = "Xem_Quang_cao_" + name2;
        Debug.Log("Firebase Event: " + logString);

        if (isTest) return;
    }
    public void LogBuyIAP(string name_)
    {
        if (Application.internetReachability == NetworkReachability.NotReachable) return;
        string name1 = HasSpecialChar(name_, "_");
        name2 = name1.Replace("_Clone", "");
        logString = "Mua_IAP_" + name2;
        Debug.Log("Firebase Event: " + logString);

        if (isTest) return;
    }
    public void LogBeginMode(string name_)
    {
        if (Application.internetReachability == NetworkReachability.NotReachable) return;
        string name1 = HasSpecialChar(name_, "_");
        name2 = name1.Replace("_Clone", "");
        logString = "Bat_dau_choi_" + name2;
        Debug.Log("Firebase Event: " + logString);

        if (isTest) return;
    }
    public void LogOpenPanel(string name_)
    {
        if (Application.internetReachability == NetworkReachability.NotReachable) return;
        string name1 = HasSpecialChar(name_, "_");
        name2 = name1.Replace("_Clone", "");
        logString = "Mo_Popup_" + name2;
        Debug.Log("Firebase Event: " + logString);

        if (isTest) return;
    }

}
