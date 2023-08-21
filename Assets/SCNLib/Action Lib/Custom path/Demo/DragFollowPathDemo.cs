using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using SCN.Common;
using UnityEngine.UI;

namespace SCN.ActionLib
{
    public class DragFollowPathDemo : MonoBehaviour
    {
        public DragFollowPath DragFollowPath;

		public Text progressText;

		private void Awake()
		{
			DragFollowPath.OnProgress += progress =>
			{
				progressText.text = "Progress: " + progress * 100 + "%";
			};
		}

		private void Start()
		{
			DragFollowPath.Init();
		}

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.R))
			{
				DragFollowPath.Reset();
			}
		}
	}
}