using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SCN;

public class ItemLevelJigsaw : MonoBehaviour
{
    [SerializeField] private Image content;
    [SerializeField] private ButtonBase button;
    [SerializeField] private GameObject lockAds;
    private int index;
    private JigsawType jigsawType;
    private bool isLockAds = false;
    private void Start()
    {
        button.onClick.AddListener(OnButtonClick);
    }
    public void InitItem(int index_,Sprite spr, JigsawType jigsawType_)
    {
        index = index_;
        content.sprite = spr;
        jigsawType = jigsawType_;
    }
    private void OnButtonClick()
    {
        EventDispatcher.Instance.Dispatch(new EventKey.PlayLevelJigsaw
        {
            index = index,
            jigsawType = jigsawType
        });
    }
    public void SetLockAds(bool isLockAds_)
    {
        isLockAds = isLockAds_;
        lockAds.SetActive(isLockAds);
    }
}
