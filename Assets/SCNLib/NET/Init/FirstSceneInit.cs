using SCN.Common;
using SCN.FirebaseLib.FA;
using SCN.FirebaseLib.FCM;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

namespace SCN.Common
{
    public class FirstSceneInit : MonoBehaviour
    {
        [SerializeField] string nextScene;

        [SerializeField] UnityEvent onSetup;

        public UnityEvent OnSetup => onSetup;
         
        bool isDone = false;

        private void Start()
        {
            Application.targetFrameRate = 60;

            DDOL.Instance.Preload();
            onSetup?.Invoke();

            Analytics.initializeOnStartup = false;
            Analytics.enabled = false;
            PerformanceReporting.enabled = false;
            Analytics.limitUserTracking = false;
            Analytics.deviceStatsEnabled = false;

            InitFirebase();
            StartCoroutine(WaitInitDone());
        }

        void InitFirebase()
        {
            Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
            {
                var dependencyStatus = task.Result;

                Debug.Log($"Init Firebase: {dependencyStatus}");
                if (dependencyStatus == Firebase.DependencyStatus.Available)
                {
                    // Create and hold a reference to your FirebaseApp,
                    // where app is a Firebase.FirebaseApp property of your application class.
                    var app = Firebase.FirebaseApp.DefaultInstance;

                    // Set a flag here to indicate whether Firebase is ready to use by your app.
                }
                else
                {
                    UnityEngine.Debug.LogError(System.String.Format(
                      "Could not resolve all Firebase dependencies: {0}", dependencyStatus));
                    // Firebase Unity SDK is not safe to use here.
                }

                isDone = true;
            });
        }

        IEnumerator WaitInitDone()
        {
            yield return new WaitUntil(() => isDone);

            GAManager.Instance.Preload();
            FirebaseMessageManager.Instance.Preload();

            if (nextScene != "")
            {
                SceneManager.LoadScene(nextScene);
            }
        }
    }
}