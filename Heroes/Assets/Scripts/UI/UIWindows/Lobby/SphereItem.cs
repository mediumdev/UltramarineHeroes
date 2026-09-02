using Configs;
using Game.Controllers;
using Game.Shop;
using TMPro;
using UI.Windows;
using UnityEngine;
using UnityEngine.UI;

namespace UI.UIWindows.Lobby
{
    public class SphereItem : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        [SerializeField] private TextMeshProUGUI _count;
        [SerializeField] private TextMeshProUGUI _title;
        [SerializeField] private ShopWindow _shopWindow;

        private CurrencyConfig _config;
        private CurrencyManager _manager;

        public void Init(CurrencyConfig config, int value)
        {
            _config = config;

            _icon.sprite = _config.Icon;
            _count.text = value.ToString();
            _title.text = _config.Title;
        }

        public void BuyMoreSpheres()
        {
            WindowManager.Instance.CloseAll();
            WindowManager.Instance.Open(_shopWindow);
        }
    }
}