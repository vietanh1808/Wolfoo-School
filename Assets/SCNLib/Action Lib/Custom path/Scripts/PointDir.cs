using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SCN.ActionLib
{
	public class PointDir : MonoBehaviour
	{
		public enum DirOption
		{
			None = 0,
			Up = 1,
			Down = 2,
			Left = 3,
			Right = 4,
		}

		[SerializeField] DirOption dir = DirOption.None;
		public DirOption Dir => dir;
	}
}