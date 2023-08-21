using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SCN.Common;
using DG.Tweening;

namespace SCN.ActionLib
{
	public class DrawPathDemo : MonoBehaviour
	{
		[SerializeField] DrawPath drawPath;
		[SerializeField] RectTransform point;

		private void Start()
		{
			DOTweenManager.Instance.TweenMoveAnchorTime(point, new Vector2(300, 300),
				new Vector2(-300, -300), 3).SetLoops(-1, LoopType.Yoyo);
		}

		private void Update()
		{
			drawPath.RendererPath();
		}
	}
}