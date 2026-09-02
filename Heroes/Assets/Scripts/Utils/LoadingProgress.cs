using UI.Windows;
using UnityEngine;

namespace Utils
{
    public class LoadingProgress : Window
    {
        [SerializeField] private Window _serializedPrefab;
        
        private static Window _loadingPrefab;
        private static Window _cached;

        private void Awake()
        {
            _loadingPrefab = _serializedPrefab;
            Destroy(gameObject);
        }

        public static void Open()
        {
            if (_cached != null)
                return;
            
            _cached = WindowManager.Instance.Open(_loadingPrefab, true, false);
        }

        public static void Close()
        {
            if (_cached != null)
                _cached.Close();

            _cached = null;
        }
    }
}
