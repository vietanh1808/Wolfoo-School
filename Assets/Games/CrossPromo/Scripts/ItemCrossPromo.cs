using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemCrossPromo : MonoBehaviour
{
    [SerializeField] private Button buttonIcon;
    [SerializeField] private Image icon;
    private const string PlayStoreUrl = "https://play.google.com/store/apps/details?id={0}";
    private string packageName;

    public void InitItem(Sprite img)
    {
        icon.sprite = img;
        packageName = icon.sprite.name;

        buttonIcon.onClick.RemoveAllListeners();
        buttonIcon.onClick.AddListener(OnIconClick);
    }
    private void OnIconClick()
    {
        Application.OpenURL(string.Format(PlayStoreUrl, packageName));
        Debug.Log(string.Format(PlayStoreUrl, packageName));
        Debug.Log(packageName);
    }
}
