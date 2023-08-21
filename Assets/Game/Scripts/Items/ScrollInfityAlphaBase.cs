using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using SCN.Common;
using SCN.UIExtend;

public abstract class ScrollInfinityAlphaBase : ScrollInfinityBase
{
	private void Awake()
	{
	}

    void SetupCustom(int itemCount, MonoBehaviour mono = null)
	{
		//for (int i = 65; i < 90; i++)
		//{
		//	var item = Instantiate(prefab.gameObject, maskTrans).GetComponent<ScrollItemBase>();
		//	item.name = i.ToString();
		//	item.Init(i, this);

		//	var itemNew = Instantiate(prefab.gameObject, maskTrans).GetComponent<ScrollItemBase>();
		//	itemNew.name = i.ToString();
		//	itemNew.Init(i + 32, this);

		//	// item dau
		//	if (i == 0)
		//	{
		//		SetFirstPosition(item);
		//		SetItemAsLast(itemNew);
		//	}
		//	else
		//	{
		//		SetItemAsLast(item);
		//		SetItemAsLast(itemNew);
		//	}
		//}

	}
}
