using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
namespace SCN.Ads.CrossPromo
{
    public class WoaAdsIconManager : MonoBehaviour
    {
        [SerializeField]
        ItemIconCrossPromo itemPre;
        [SerializeField]
        Transform contentIcons;
        [SerializeField]
        Sprite[] defaultIcons;
        [SerializeField]
        string[] defaultLink;
        private List<ItemIconCrossPromo> itemIcons;
        private void Start()
        {
            itemIcons = new List<ItemIconCrossPromo>();
            AdsManager.Instance.WoaAdsControl.UpdateAdsAssets += InitListIcons;
           InitListIcons();
        }
        private void InitListIcons()
        {
            Clear();
            if (AdsManager.Instance.WoaAdsControl.IsAdsAssetsAble) {
                var ListAdAssets = AdsManager.Instance.WoaAdsControl.ListAdAssets;
                var count = 0;
                DOVirtual.DelayedCall(.1f, () =>
                {
                    foreach (var Asset in ListAdAssets)
                    {
                        var item = Instantiate(itemPre, contentIcons);
                        item.InitializeItem(Asset.Icon, Asset.UrlGame);
                        itemIcons.Add(item);
                        count++;
                        if (count >= 3) break;
                    }
                });
            }
            else
            {
                LoadDefault();
            }
            
        }
        private void LoadDefault()
        {
            for (int i = 0; i < defaultIcons.Length; i++)
            {
                var item = Instantiate(itemPre, contentIcons);
                item.InitializeItem(defaultIcons[i], defaultLink[i]);
                itemIcons.Add(item);
            }
        }
        private void Clear()
        {
            foreach (var item in itemIcons)
            {
                DestroyImmediate(item.gameObject);
            }
            itemIcons= new List<ItemIconCrossPromo>();
        }
    }
}

