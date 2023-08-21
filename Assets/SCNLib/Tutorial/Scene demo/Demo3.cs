using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Demo3 : MonoBehaviour
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
		SCN.Tutorial.TutorialManager.Instance.StartPointer(pos[0].position, pos[1].position);
	}
}
