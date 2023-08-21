using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Demo2 : MonoBehaviour
{
	[SerializeField] Transform[] pos;
	[SerializeField] Button btn;

	private void Awake()
	{
		btn.onClick.AddListener(() =>
		{
			SCN.Tutorial.TutorialManager.Instance.Stop();
		});
	}

	private void Start()
	{
		SCN.Tutorial.TutorialManager.Instance.StartPointer(btn.transform);

		Move();
	}

	void Move()
	{
		btn.transform.DOMove(pos[0].position, 2).OnComplete(() =>
		{
			btn.transform.DOMove(pos[1].position, 2).OnComplete(() =>
			{
				Move();
			});
		});
	}
}