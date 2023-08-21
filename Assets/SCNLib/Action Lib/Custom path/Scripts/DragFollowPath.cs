using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using SCN.Common;

namespace SCN.ActionLib
{
	public class DragFollowPath : MonoBehaviour
	{
		public System.Action<float> OnProgress;

		[SerializeField] EventTrigger dragObject;

		[Tooltip("Path ma object di chuyen theo")]
		[SerializeField] CustomPath movePath;

		[Space]
		[Header("Custom")]
		[Tooltip("So cang lon cang chinh xac, nhung se giam hieu nang")]
		[Range(10, 100)]
		[SerializeField] int breakPoint = 30;

		[Space]
		[Header("Detail")]
		[SerializeField] int currentPoint;
		[SerializeField] PointDir.DirOption currentDir = PointDir.DirOption.None;

		[Space(5)]
		// List cac chi so xac dinh vat dang di chuyen nhu nao tren Path
		[SerializeField] Vector3 startPos;
		[SerializeField] Vector3 endPos;
		[SerializeField] Vector3 currentMoveVt;

		// List cac chi so xac dinh drag
		[SerializeField] Vector3 lastPos;
		[SerializeField] Vector3 currentDragVt;
		[SerializeField] Vector3 currentConvertDragVt;

		[SerializeField] float progress;
		bool isInit;

		public EventTrigger DragObject { get => dragObject; set => dragObject = value; }
		public CustomPath MovePath { get => movePath; set => movePath = value; }
		public float Progress => progress;

		public void Init()
		{
			if (isInit)
			{
				return;
			}

			isInit = true;

			Master.AddEventTriggerListener(dragObject, EventTriggerType.PointerDown, OnPointerDown);
			Master.AddEventTriggerListener(dragObject, EventTriggerType.Drag, OnDrag);

			currentPoint = 0;
			progress = 0;

			UpdateMoveDetail();

			dragObject.transform.position = startPos;
		}

		public void Reset()
		{
			currentPoint = 0;
			progress = 0;

			UpdateMoveDetail();

			dragObject.transform.position = startPos;
		}

		void UpdateMoveDetail()
		{
			if (progress == 1)
			{
				return;
			}

			var dir = movePath.transform.GetChild(movePath.GetInforPath(
				(float)currentPoint / breakPoint).pathOrder).GetComponent<PointDir>().Dir;
			if (dir != PointDir.DirOption.None)
			{
				currentDir = dir;
			}

			startPos = movePath.GetPos((float)currentPoint / breakPoint);
			endPos = movePath.GetPos((float)(currentPoint + 1) / breakPoint);
			currentMoveVt = endPos - startPos;
		}

		void OnPointerDown(BaseEventData data)
		{
			var vt = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			vt.z = transform.position.z;

			lastPos = vt;
		}

		void OnDrag(BaseEventData data)
		{
			var vt = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			vt.z = transform.position.z;

			currentDragVt = vt - lastPos;
			//Debug.Log("current drag: " + currentDragVt);	

			var angle = Vector3.Angle(currentMoveVt, Vector3.up);
			dragObject.transform.localEulerAngles = new Vector3(0, 0, dragObject.transform.localPosition.x < 0 ? -angle : angle);

			lastPos = vt;
			MoveObject();
		}

		void MoveObject()
		{
			if (!CheckAbleDir())
			{
				return;
			}

			currentConvertDragVt = ConvertDragVt();
			if (currentConvertDragVt.x == 0 && currentConvertDragVt.y == 0)
			{
				return;
			}

			if (Vector3.Distance(dragObject.transform.position, endPos)
				> Vector3.Distance(Vector3.zero, currentConvertDragVt))
			{
				dragObject.transform.position += currentConvertDragVt;
			}
			else
			{
				if (progress == 1)
				{
					return;
				}

				dragObject.transform.position = endPos;
				currentPoint++;

				progress = Mathf.Clamp((float)currentPoint / breakPoint, 0, 1);
				OnProgress?.Invoke(progress);

				UpdateMoveDetail();
			}
		}

		Vector3 ConvertDragVt()
		{
			if (currentMoveVt.x == 0)
			{
				return new Vector3(0, currentDragVt.y, 0);
			}
			else if (currentMoveVt.y == 0)
			{
				return new Vector3(currentDragVt.x, 0, 0);
			}

			var coeff = currentMoveVt.x / currentMoveVt.y;
			// Tinh vt mac dinh
			var realValue = Mathf.Sqrt((Mathf.Pow(currentDragVt.x, 2) + Mathf.Pow(currentDragVt.y, 2))
				/ (1 + Mathf.Pow(coeff, 2)));

			var y = currentMoveVt.y < 0 ? -realValue : realValue;
			var x = y * coeff;

			// Tinh vt thuc te
			var x0 = 0f;
			var y0 = 0f;
			if (currentDir == PointDir.DirOption.Up || currentDir == PointDir.DirOption.Down)
			{
				y0 = currentDragVt.y;
				x0 = y0 * coeff;
			}
			else if (currentDir == PointDir.DirOption.Left || currentDir == PointDir.DirOption.Right)
			{
				x0 = currentDragVt.x;
				y0 = x0 / coeff;
			}

			x0 = currentMoveVt.x > 0 ? Mathf.Abs(x0) : -Mathf.Abs(x0);
			y0 = currentMoveVt.y > 0 ? Mathf.Abs(y0) : -Mathf.Abs(y0);

			if (Mathf.Abs(x0) > Mathf.Abs(x))
			{
				return new Vector3(x, y, 0);
			}
			else
			{
				return new Vector3(x0, y0, 0);
			}
		}

		bool CheckAbleDir()
		{
			if (currentDir == PointDir.DirOption.Up
				&& currentDragVt.y > 0)
			{
				return true;
			}
			else  if (currentDir == PointDir.DirOption.Down
				&& currentDragVt.y < 0)
			{
				return true;
			}
			else if (currentDir == PointDir.DirOption.Left
				&& currentDragVt.x < 0)
			{
				return true;
			}
			else if (currentDir == PointDir.DirOption.Right
				&& currentDragVt.x > 0)
			{
				return true;
			}

			return false;
		}
	}
}