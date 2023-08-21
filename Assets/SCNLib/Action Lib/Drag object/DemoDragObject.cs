using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SCN.ActionLib
{
	public class DemoDragObject : MonoBehaviour
	{
		[SerializeField] DragObject dragObject0;
		[SerializeField] DragObject dragObject1;

		private void Start()
		{
			dragObject0.Init();
			dragObject1.Init();
		}
	}
}