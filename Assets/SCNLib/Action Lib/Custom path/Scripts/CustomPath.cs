using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using DG.Tweening.Plugins.Core.PathCore;

namespace SCN.ActionLib
{
	public class CustomPath : MonoBehaviour
	{
		[Range(0, 100)]
		[SerializeField] int division = 5;

		const float delta = 0.05f;

		[SerializeField] float[] pathLengths;
		[SerializeField] float totalLength;

		public float[] PathLengths => pathLengths;
		public float TotalLength => totalLength;

		public Transform GetPoint(int order)
		{
			return transform.GetChild(order);
		}

		/// <summary>
		/// Lay toa do cua diem xac dinh boi 'totalDelta'
		/// </summary>
		/// <param name="totalDelta">0: diem dau, 0.5: diem giua, 1: diem cuoi, tinh tren toan bo 'Path'</param>
		public Vector3 GetPos(float totalDelta)
		{
			(var pathOrder, var delta) = GetInforPath(totalDelta);
			return GetPos(pathOrder, delta);
		}

		/// <summary>
		/// Lay thong tin cua 'Path' ma chua diem xac dinh moi 't'
		/// </summary>
		/// <param name="t">0: diem dau, 0.5: diem giua, 1: diem cuoi, tinh tren toan bo 'Path'</param>
		/// <returns></returns>
		public (int pathOrder, float delta) GetInforPath(float t)
		{
			var temp = t * totalLength;
			for (int i = 0; i < pathLengths.Length; i++)
			{
				var index = i;
				if (temp > pathLengths[i])
				{
					temp -= pathLengths[i];
				}
				else
				{
					return (index, temp / pathLengths[index]);
				}
			}

			return (pathLengths.Length - 1, 1); // last point
		}

		/// <summary>
		/// Lay toa do cua 1 diem nam tren 'Path'
		/// </summary>
		/// <param name="pathOrder">so thu tu cua 'Path' ma diem can lay nam tren</param>
		/// <param name="delta">0: diem dau, 0.5: diem giua, 1: diem cuoi</param>
		public Vector3 GetPos(int pathOrder, float delta)
		{
			var startPoint = transform.GetChild(pathOrder);
			var endPoint = transform.GetChild(pathOrder + 1);

			return GetPos(startPoint, endPoint, delta);
		}

		/// <summary>
		/// Lay toa do cua 1 diem nam tren 'Path'
		/// </summary>
		/// <param name="startPoint">diem bat dau cua Path</param>
		/// <param name="endPoint">diem ket thuc cua Path</param>
		/// <param name="delta">0: diem dau, 0.5: diem giua, 1: diem cuoi</param>
		public static Vector3 GetPos(Transform startPoint, Transform endPoint, float delta)
		{
			return Mathf.Pow(1 - delta, 3) * startPoint.position
				+ 3 * Mathf.Pow(1 - delta, 2) * delta * startPoint.GetChild(1).position
				+ 3 * (1 - delta) * Mathf.Pow(delta, 2) * endPoint.GetChild(0).position
				+ Mathf.Pow(delta, 3) * endPoint.position;
		}

#if UNITY_EDITOR
		void SetupLength()
		{
			totalLength = 0;
			pathLengths = new float[transform.childCount - 1];
			for (int i = 0; i < pathLengths.Length; i++)
			{
				pathLengths[i] = GetLength(transform.GetChild(i), transform.GetChild(i + 1));
				totalLength += pathLengths[i];
			}
		}

		float GetLength(Transform startPoint, Transform endPoint)
		{
			var length = 0f;

			var lastPos = startPoint.position;
			var distance = 1f / division;

			for (int i = 0; i < division - 1; i++)
			{
				var pos = GetPos(startPoint, endPoint, distance * (i + 1));

				length += Vector3.Distance(pos, lastPos);
				lastPos = pos;
			}

			length += Vector3.Distance(lastPos, endPoint.position);

			return length;
		}

		private void OnDrawGizmos()
		{
			if (UnityEditor.Selection.activeGameObject == null
				|| !UnityEditor.Selection.activeGameObject.transform.IsChildOf(transform))
			{
				return;
			}

			// cannot draw path with < 2 points
			if (transform.childCount < 2)
			{
				return;
			}

			// n child => n-1 path
			for (int i = 0; i < transform.childCount - 1; i++)
			{
				var startPoint = transform.GetChild(i);
				var endPoint = transform.GetChild(i + 1);

				Gizmos.color = Color.red;
				Gizmos.DrawSphere(startPoint.position, 0.1f);
				Gizmos.DrawSphere(endPoint.position, 0.1f);

				Vector3 lastPos = Vector3.zero;
				// draw path
				var pointCount = (int)(1 / delta);
				for (int j = 0; j <= pointCount; j++)
				{
					var delta = (float)j / pointCount;
					var pos = GetPos(startPoint, endPoint, delta);

					if (j != 0)
					{
						Gizmos.color = new Color(0, 1, 1);
						Gizmos.DrawLine(pos, lastPos);
					}

					lastPos = pos;
				}
			}

			SetupLength();

			if (UnityEditor.Selection.activeGameObject != null
				&& UnityEditor.Selection.activeGameObject.transform.IsChildOf(transform))
			{
				for (int i = 0; i < transform.childCount; i++)
				{
					if (UnityEditor.Selection.activeGameObject == transform.GetChild(i).gameObject
						|| UnityEditor.Selection.activeGameObject.transform.IsChildOf(transform.GetChild(i)))
					{
						if (i != 0)
						{
							Gizmos.color = new Color(0.5f, 1, 0.5f);
							Gizmos.DrawLine(transform.GetChild(i).position,
								transform.GetChild(i).GetChild(0).position);

							Gizmos.DrawSphere(transform.GetChild(i).GetChild(0).position, 0.075f);
						}
						if (i != transform.childCount - 1)
						{
							Gizmos.color = new Color(1, 1, 0.5f);
							Gizmos.DrawLine(transform.GetChild(i).position,
								transform.GetChild(i).GetChild(1).position);

							Gizmos.DrawSphere(transform.GetChild(i).GetChild(1).position, 0.075f);
						}
						break;
					}
				}
			}
		}
#endif
	}
}