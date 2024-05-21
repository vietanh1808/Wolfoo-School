using Firebase.Messaging;
using SCN.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SCN.FirebaseLib.FCM
{
    [CreateAssetMenu(fileName = AssetName, menuName = "SCN/Scriptable Objects/FCM settings")]
    public class FirebaseMessageManager : ScriptableObject
    {
        public const string AssetName = "FCM setting";

        static FirebaseMessageManager instance;
        public static FirebaseMessageManager Instance
        {
            get
            {
                if (instance == null)
                {
                    InitializeFCM();
                }
                return instance;
            }
        }

        [SerializeField] string userToken;
        bool isTokenReady;

        public string UserToken => userToken;
        public bool IsTokenReady => isTokenReady;

        static void InitializeFCM()
        {
            instance = LoadSource.LoadObject<FirebaseMessageManager>(AssetName);
            if (instance == null)
            {
                Debug.LogError("Missing FCM setting SO in Resources folder");
            }

            DDOL.Instance.StartCoroutine(instance.GetTokenAsync());
        }

        public void Preload()
        {

        }

        public void CheckAndGetToken(System.Action<string> onTokenReady)
		{
            DDOL.Instance.StartCoroutine(WaitTokenAsync(onTokenReady));
		}

        IEnumerator WaitTokenAsync(System.Action<string> onTokenReady)
		{
            yield return new WaitUntil(() => isTokenReady);
            onTokenReady?.Invoke(userToken);
		}

        IEnumerator GetTokenAsync()
        {
            isTokenReady = false;

            var task = FirebaseMessaging.GetTokenAsync();
            yield return new WaitUntil(() => task.IsCompleted);

            //yield return null;
            userToken = task.Result;
            isTokenReady = true;

        }
    }
}