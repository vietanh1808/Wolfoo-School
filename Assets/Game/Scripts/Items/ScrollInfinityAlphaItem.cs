using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SCN.Common;
using DG.Tweening;
using UnityEngine.EventSystems;
using SCN.UIExtend;
using System;
using SCN;

[RequireComponent(typeof(EventTrigger))]
public class ScrollInfinityAlphaItem : ScrollItemBase
{
	/// <summary>
	/// Nhac tay len sau khi da Drag item
	/// </summary>
	public static System.Action<ScrollInfinityAlphaItem> OnPointerUpAfterDrag;

	/// <summary>
	/// Khi item dang duoc Drag
	/// </summary>
	public static System.Action<ScrollInfinityAlphaItem> OnItemDragging;

	[SerializeField] Image image;
    [SerializeField] Text alphaTxt;
	[SerializeField] Button lockBtn;

	bool isDrag;
	Vector3 startScale;
    private int id;
    private bool isLower;
    private Tweener punchTween;

    private void Start()
    {
		lockBtn.onClick.AddListener(OnLockClick);
		EventManager.OnWatchAds += GetWatchAds;
	}
	private void OnDestroy()
	{
		GameManager.instance.curAlphaIdx = 0;
		if (punchTween != null) punchTween?.Kill();
		EventManager.OnWatchAds -= GetWatchAds;
	}

	private void GetWatchAds(int idx, PriceItem priceItem)
	{
		if (id != idx) return;

		DataMainManager.instance.LocalDataStorage.unlockAlphas[id] = true;
		DataMainManager.instance.SaveItem(DataMainManager.StorageKey.Data);
		Unlock();
	}
	public void Unlock()
	{
		image.raycastTarget = true;
		lockBtn.gameObject.SetActive(false);
	}

	private void OnLockClick()
    {
		EventDispatcher.Instance.Dispatch(new EventKey.InitAdsPanel
		{
			textStr = order <= 25 ? ((char)(order + 65) + "") : ((char)(order + 97-26) + ""),
			instanceID = gameObject.GetInstanceID(),
			idxItem = order
		});
		//EventManager.InitAdsPanelWithNoCoin?.Invoke(id, image.sprite);
		EventManager.OpenPanel?.Invoke(PanelType.Ads);
	}

    protected override void Setup(int order)
	{
		alphaTxt.color = Color.HSVToRGB(0.6f + order / 100f, 1f, 1f);
		id = order;
		if (order <= 25)
		{
			isLower = true;
			alphaTxt.text = (char)(order + 65) + "";
		}
		else
        {
			isLower = false;
			order -= 26;
			alphaTxt.text = (char)(order + 97) + "";
		}

		image.raycastTarget = DataMainManager.instance.LocalDataStorage.unlockAlphas[order];
		lockBtn.gameObject.SetActive(!DataMainManager.instance.LocalDataStorage.unlockAlphas[order]);

		Master.AddEventTriggerListener(EventTrigger, EventTriggerType.PointerDown, OnPointerDown);
		Master.AddEventTriggerListener(EventTrigger, EventTriggerType.PointerUp, OnPointerUp);
		Master.AddEventTriggerListener(EventTrigger, EventTriggerType.PointerClick, OnPointerClick);

	}

	protected override void OnStartDragOut()
	{
	}

	protected override void OnDragOut()
	{
		// OnItemDragging?.Invoke(this);
	}

	void OnPointerDown(BaseEventData data)
	{
	}

	void OnPointerUp(BaseEventData data)
	{
	}
	void OnPointerClick(BaseEventData data)
	{
		if (GameManager.instance.curAlphaIdx == id) return;
		if (punchTween != null && punchTween.IsActive()) return;

		GameManager.instance.curAlphaIdx = id;
		punchTween = transform.DOPunchRotation(Vector3.forward * -10, 1f, 2);
		EventManager.OnClickAlpha?.Invoke(id > 25 ? id -= 26 : id, !isLower);
	}
}