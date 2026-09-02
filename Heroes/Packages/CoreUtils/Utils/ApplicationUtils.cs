using System;

namespace CoreUtils.Utils
{
	public class ApplicationUtils : MonoSingleton<ApplicationUtils>
	{
		protected override void Init()
		{
			DontDestroyOnLoad(gameObject);
		}

		public event Action<bool> OnApplicationPauseEvent;

		private void OnApplicationPause(bool pauseStatus)
		{
			if (OnApplicationPauseEvent != null)
				OnApplicationPauseEvent(pauseStatus);
		}

		public event Action OnApplicationQuitEvent;

		private void OnApplicationQuit()
		{
			if (OnApplicationQuitEvent != null)
				OnApplicationQuitEvent();
		}
	}
}
