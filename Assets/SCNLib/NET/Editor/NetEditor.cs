using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SCN.Common;
using SCN.FirebaseLib.FA;
using SCN.FirebaseLib.FCM;
using SCN.Ads.CrossPromo;

namespace SCN.Ads
{
    public static class NetEditor
    {
        [MenuItem("SCN/Net/Admob settings android")]
        static void SelectAdmobAndroidConfig()
        {
            Master.CreateAndSelectAssetInResource<AdmobConfig>(AdmobConfig.AssetNameAndroid);
        }

        [MenuItem("SCN/Net/Admob settings ios")]
        static void SelectSettingAdmobIOSConfig()
        {
            Master.CreateAndSelectAssetInResource<AdmobConfig>(AdmobConfig.AssetNameIOS);
        }

        [MenuItem("SCN/Net/Woa settings")]
        static void SelectSettingWoaConfig()
        {
            Master.CreateAndSelectAssetInResource<WoaAdsConfig>(WoaAdsConfig.AssetName);
        }

        [MenuItem("SCN/Net/GA setting")]
        static void SelectSettingGAConfig()
		{
            Master.CreateAndSelectAssetInResource<GAManager>(GAManager.AssetName);
        }

        [MenuItem("SCN/Net/FCM setting")]
        static void SelectSettingFCMConfig()
        {
            Master.CreateAndSelectAssetInResource<FirebaseMessageManager>(FirebaseMessageManager.AssetName);
        }
        [MenuItem("GameObject/CrossPromo/VerticleIcons", false, 10)]
        static void CreateVerticleIcons(MenuCommand menuCommand)
        {
            var path= ("Assets/SCNLib/NET/Ads/WoaAds/Prefabs/GridIconCrossPromo/Verticle_CrossPromo.prefab");
            var obj = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject));
            Debug.Log(obj);
            GameObject go = PrefabUtility.InstantiatePrefab(obj) as GameObject;
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
            Selection.activeObject = go;
        }
        [MenuItem("GameObject/CrossPromo/HorizontalIcons", false, 10)]
        static void CreateHorizontalIcons(MenuCommand menuCommand)
        {
            var path = ("Assets/SCNLib/NET/Ads/WoaAds/Prefabs/GridIconCrossPromo/Horizontal_CrossPromo.prefab");
            var obj = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject));
            Debug.Log(obj);
            GameObject go = PrefabUtility.InstantiatePrefab(obj) as GameObject;
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
            Selection.activeObject = go;
        }
        [MenuItem("GameObject/CrossPromo/NativeAds", false, 10)]
        static void CreateNativeAds(MenuCommand menuCommand)
        {
            var path = ("Assets/SCNLib/NET/Ads/WoaAds/Prefabs/NativeCrossPromo/NativeCrossPromo.prefab");
            var obj = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject));
            Debug.Log(obj);
            GameObject go = PrefabUtility.InstantiatePrefab(obj) as GameObject;
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
            Selection.activeObject = go;
        }
    }
}