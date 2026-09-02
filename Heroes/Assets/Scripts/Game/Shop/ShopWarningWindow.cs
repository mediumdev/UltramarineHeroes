using JetBrains.Annotations;
using UI.Windows;
using UnityEngine;

namespace Game.Shop
{
    public class ShopWarningWindow : Window
    {
        [SerializeField] private ShopWindow _shopWindow;

        private int _shopSection;
        private bool _forced;
        private Window _parentToClose;

        public void OpenShopWindow()
        {
            if (!(_parentToClose is null)) _parentToClose.Close();
            Close();
            if (WindowManager.Instance.LastOpened is ShopWindow)
            {
                var prevShopWindow = WindowManager.Instance.LastOpened as ShopWindow;
                if (prevShopWindow != null)
                    prevShopWindow.OpenSection(_shopSection);
            }
            else
            {
                var shopWindow = WindowManager.Instance.Open(_shopWindow, _forced) as ShopWindow;
                if (shopWindow != null)
                {
                    shopWindow.Init(_shopSection);
                }
            }
        }

        public void Init(int shopSection, bool forced = false, Window parentToClose = null)
        {
            _shopSection = shopSection;
            _forced = forced;
            _parentToClose = parentToClose;
        }

        [UsedImplicitly]
        public void Back()
        {
            Close();
        }
    }
}