using CoreUtils.Utils;
using UnityEngine;

namespace UI.Windows
{
	public class UiManager : MonoSingleton<UiManager>
	{

		private Canvas _mainCanvas;
		public Canvas MainCanvas
		{
			get
			{
				if (_mainCanvas == null)
				{
					_mainCanvas = FindObjectOfType<Canvas>();
				}

				return _mainCanvas;
			}
		}

		protected override void Init()
		{
			base.Init();
			DontDestroyOnLoad(gameObject);
		}
	}
}
