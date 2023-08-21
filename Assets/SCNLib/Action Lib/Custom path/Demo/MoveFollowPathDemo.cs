using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SCN.Common;
using DG.Tweening;

namespace SCN.ActionLib 
{
	public class MoveFollowPathDemo : MonoBehaviour
	{
		public Transform trans;
		public CustomPath path;

		Tweener moveTween;
		const int totalPoint = 30;

		int count = 0;
		private void Update()
		{
			// Spam A de set vi tri tren duong cong
			if (Input.GetKeyDown(KeyCode.A))
			{
				StopMove();
				trans.position = path.GetPos((float)count / totalPoint);

				count++;
				if (count > totalPoint)
				{
					count = 0;
				}
			}
			// Di chuyen vat theo duong cong
			else if (Input.GetKeyDown(KeyCode.M))
			{
				StopMove();

				Vector3 startPos = Vector3.zero;
				Vector3 endPos = Vector3.zero;
				int order = 0;

				MoveElement(startPos, endPos, order);
			}
			// Dung vat dang di chuyen
			else if (Input.GetKeyDown(KeyCode.S))
			{
				StopMove();
			}
		}

		void MoveElement(Vector3 startPos, Vector3 endPos, int order)
		{
			if (order >= totalPoint)
			{
				return;
			}

			startPos = path.GetPos((float)order / totalPoint);
			endPos = path.GetPos((float)(order + 1) / totalPoint);

			moveTween = DOTweenManager.Instance.TweenMoveVelocity(trans, startPos, endPos, 15, false)
				.OnComplete(() =>
				{
					order++;
					MoveElement(startPos, endPos, order);
				});
		}

		void StopMove()
		{
			DOTweenManager.Instance.KillTween(moveTween);
		}
	}
}