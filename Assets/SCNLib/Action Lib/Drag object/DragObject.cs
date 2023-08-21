using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using SCN.Common;

namespace SCN.ActionLib 
{
	public class DragObject : MonoBehaviour
	{
		[Header("Ref")]
		/// <summary>
		/// EventTrigger của object được Drag
		/// </summary>
		[SerializeField] EventTrigger eventTrigger;

		[Space]
		[Header("Properties")]

		bool isInit;
		[SerializeField] bool isDragable = true;
		[SerializeField] bool horizontal = true;
		[SerializeField] bool vertical = true;

		public EventTrigger EventTrigger => eventTrigger;
		public bool IsDragable { get => isDragable; set => isDragable = value; }
		public bool Horizontal { get => horizontal; set => horizontal = value; }
		public bool Vertical { get => vertical; set => vertical = value; }

		Vector3 deltaVt;
		Vector3 pos;

		public void Init()
		{
			if (isInit)
			{
				return;
			}

			isInit = true;

			pos.z = transform.position.z;

			Master.AddEventTriggerListener(eventTrigger, EventTriggerType.PointerDown, OnPointerDown);
			Master.AddEventTriggerListener(eventTrigger, EventTriggerType.Drag, OnDrag);
		}

		public void RemoveAllListener()
		{
			isInit = false;

			Master.RemoveEventTriggerListener(eventTrigger, EventTriggerType.PointerDown, OnPointerDown);
			Master.RemoveEventTriggerListener(eventTrigger, EventTriggerType.Drag, OnDrag);
		}

		void OnPointerDown(BaseEventData data)
		{
			if (!isDragable)
			{
				return;
			}

			var vt = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			vt.z = transform.position.z;
			deltaVt = transform.position - vt;
		}

		void OnDrag(BaseEventData data)
		{
			if (!isDragable)
			{
				return;
			}

			var vt = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			vt.z = transform.position.z;

			pos.x = horizontal ? (vt.x + deltaVt.x): transform.position.x;
			pos.y = vertical ? (vt.y + deltaVt.y) : transform.position.y;
			pos.z = transform.position.z;

			transform.position = pos;
		}
	}
}