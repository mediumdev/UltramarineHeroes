using System.Collections.Generic;
using Configs;
using Game.Controllers;
using Game.Shop;
using Structs;
using TMPro;
using UI.Windows;
using UnityEngine;
using UnityEngine.UI;

namespace UI.UIWindows.Lobby
{
    public class LobbyCurrency : MonoBehaviour
    {
        [SerializeField] private CurrencyConfig _currency;
        [SerializeField] private TextMeshProUGUI _countText;
        [SerializeField] private Image _currencyIcon;
    
        public CurrencyConfig Currency => _currency;

        public void Init(CurrencyConfig currencyConfig)
        {
            _currency = currencyConfig;
            Repaint();
        }

        private void Start()
        {
            Repaint();
            CurrencyManager.Instance.CurrencyChangedEvent += RepaintSubscriber;
        }

        private void RepaintSubscriber(List<CurrencyWithCount> obj)
        {
            Repaint();
        }

        private void Repaint()
        {
            if (_currency == null) return;
        
            _countText.text = CurrencyManager.Instance.CurrencyValue(_currency).ToString();
        
            if (_currencyIcon == null) return;

            _currencyIcon.sprite = _currency.Icon;
        }

        private void OnDisable()
        {
            CurrencyManager.Instance.CurrencyChangedEvent -= RepaintSubscriber;
        }

        public void OpenShop(int section = 1)
        {
            WindowManager.Instance.CloseAll();
            var shopWindow = WindowManager.Instance.Open(GameSettings.Instance.Settings.ShopWindow) as ShopWindow;
            if (shopWindow != null) shopWindow.OpenSection(section);
        }
    }
}
