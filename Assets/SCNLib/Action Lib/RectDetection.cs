using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SCN.ActionLib 
{
	/// <summary>
	/// Phát hiện vật thể nằm trong khoảng nào đó, dựa vào Recttransform
	/// </summary>
	public class RectDetection
	{
		Rect rect;
		float canvasScaler;

		#region Constructor
		public RectDetection(RectTransform rectTrans, float canvasScaler)
		{
			this.canvasScaler = canvasScaler;
			ConvertRect(rectTrans);
		}

		public void ChangeRect(RectTransform rectTrans)
		{
			ConvertRect(rectTrans);
		}

		void ConvertRect(RectTransform rectTrans)
		{
			var w = rectTrans.sizeDelta.x * canvasScaler;
			var h = rectTrans.sizeDelta.y * canvasScaler;
			rect = new Rect(new Vector2(rectTrans.position.x - w / 2
				, rectTrans.position.y - h / 2), new Vector2(w, h));
		}
		#endregion

		public bool CheckIn(RectTransform trans)
		{
			return rect.Contains(trans.position);
		}

		public bool CheckIn(Vector3 pos)
		{
			return rect.Contains(pos);
		}

		/// <summary>
		/// The nearest pos in rect
		/// </summary>
		public Vector3 NearPos(Vector3 pos)
		{
			return new Vector3(Mathf.Clamp(pos.x, rect.min.x, rect.max.x)
				, Mathf.Clamp(pos.y, rect.min.y, rect.max.y), pos.z);
		}
	}
}