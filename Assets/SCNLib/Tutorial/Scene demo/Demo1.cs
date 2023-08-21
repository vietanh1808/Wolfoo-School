using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Demo1 : MonoBehaviour
{
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
        SCN.Tutorial.TutorialManager.Instance.StartPointer(btn.transform.position);
    }
}