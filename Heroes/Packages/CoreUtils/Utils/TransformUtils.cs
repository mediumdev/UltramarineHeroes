using System.Linq;
using UnityEngine;

namespace CoreUtils.Utils
{
	public static class TransformUtils {

		public static void Clear(this Transform transform)
		{
			foreach (Transform child in transform.Cast<Transform>().ToArray())
			{
				child.SetParent(null, false);
				Object.Destroy(child.gameObject);
			}
		}
	}
}
