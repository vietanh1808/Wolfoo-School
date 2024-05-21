using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SCN;
using SCN.Ads;

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
        if (isLockAds)
        {
            if (AdsManager.Instance.HasRewardVideo)
            {
                AdsManager.Instance.ShowRewardVideo(() =>
                {
                    //PlayerData.ChangeData(Config.JIGSAW, jigsawType==SessionJigsaw.JigsawType.Alphabet? Config.JIGSAW_ALPHABET:
                    //Config.JIGSAW_PICTURE, index);
                    //isLockAds = false;
                    //lockAds.SetActive(isLockAds);
                    EventDispatcher.Instance.Dispatch(new EventKey.PlayLevelJigsaw
                    {
                        index = index,
                        jigsawType = jigsawType
                    });
                    //FirebaseManager.Instance.WatchReward("jigsaw_" + jigsawType + "_" + index);
                });
            }
        }
        else
        {
            EventDispatcher.Instance.Dispatch(new EventKey.PlayLevelJigsaw { index = index, jigsawType = jigsawType });
        }
    }
    public void SetLockAds(bool isLockAds_)
    {
        isLockAds = isLockAds_;
        lockAds.SetActive(isLockAds);
    }
}
