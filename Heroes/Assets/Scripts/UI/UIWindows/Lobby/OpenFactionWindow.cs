using Configs;
using UI.UIWindows.Faction;
using UI.Windows;
using UnityEngine;

namespace UI.UIWindows.Lobby
{
    public class OpenFactionWindow : MonoBehaviour
    {
        [SerializeField] private FactionConfig _config;
        [SerializeField] private FactionCastleWindow _window;

        public void Init()
        {
            var window = WindowManager.Instance.Open(_window) as FactionCastleWindow;

            if (window != null) 
                window.Init(_config);
        }
    }
}
