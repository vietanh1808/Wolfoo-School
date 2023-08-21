using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class CrossPromo : MonoBehaviour
{
    [SerializeField] private ItemCrossPromo itemPre;
    [SerializeField] private Transform content;

    [Header("Import Icons Below")]
    [SerializeField] private Sprite[] icons;

	private void Start()
	{
        // ThanhTT: Init random toan bo cac item
        var randomNoRepeat = new RandomNoRepeat<Sprite>(icons);
		for (int i = 0; i < content.childCount; i++)
		{
            content.GetChild(i).GetComponent<ItemCrossPromo>().InitItem(randomNoRepeat.Random());
		}
	}

    // ThanhTT: Class random khong lap lai
    public class RandomNoRepeat<T>
    {
        readonly List<T> listRandom;
        public List<T> ListTemp { get; set; }

        public RandomNoRepeat(IEnumerable<T> listR)
        {
            listRandom = new List<T>(listR);
            ListTemp = new List<T>(listRandom);
        }

        public T Random()
        {
            if (ListTemp.Count > 0)
            {
                var tempObj = RandomInList(ListTemp);
                _ = ListTemp.Remove(tempObj);
                return tempObj;
            }
            ListTemp = new List<T>(listRandom);
            return Random();
        }

        public static T1 RandomInList<T1>(List<T1> listRandom)
        {
            if (listRandom.Count == 0) return default;

            var indexRandom = UnityEngine.Random.Range(0, listRandom.Count);
            return listRandom[indexRandom];
        }
    }

    // ThanhTT: a tam thoi bo phan nay nhe
    //public void SpawnIconEditor()
    //{
    //    for (int i = 0; i < icons.Length; i++)
    //    {
    //        var item = Instantiate(itemPre, content);
    //        item.InitItem(icons[i]);
    //    }
    //}
}
#if UNITY_EDITOR
[CustomEditor(typeof(CrossPromo))]
public class CrossPromoEditor : Editor
{
    private CrossPromo crossPromo;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        //if (!crossPromo)
        //    crossPromo = target as CrossPromo;
        //if (!crossPromo)
        //    return;
        //GUILayout.Space(20);
        //var headerStyle = new GUIStyle();
        //headerStyle.fontStyle = FontStyle.Bold;
        //GUILayout.Label("CrossPromo editor", headerStyle);
        //EditorGUILayout.BeginVertical();
        //{
        //    if (GUILayout.Button("Spawn Object Icon"))
        //    {
        //        //crossPromo.SpawnIconEditor();
        //    }
        //}
        //EditorGUILayout.EndVertical();
    }
}
#endif
