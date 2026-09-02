using UnityEngine;

namespace CoreUtils.Utils
{
	public class AutoDestroy : MonoBehaviour
	{ 
		[SerializeField] private float _time;

		private void Awake()
		{
			Destroy(gameObject, _time);
		}
	}
}
