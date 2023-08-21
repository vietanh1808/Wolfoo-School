using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SCN.ActionLib
{
    public class DrawPath : MonoBehaviour
    {
        [SerializeField] LineRenderer line;

        [Space(5)]
        [Header("Custom")]
        [SerializeField] CustomPath path;
		[Range(2, 1000)]
		[SerializeField] int pointNumb;

        public void RendererPath()
		{
			var distance = (float)1 / pointNumb;

			line.positionCount = pointNumb;

			line.SetPosition(0, path.GetPos(0));
			for (int i = 1; i <= pointNumb - 2; i++)
			{
				line.SetPosition(i, path.GetPos(distance * i));
			}
			line.SetPosition(pointNumb - 1, path.GetPos(1));
		}
    }
}