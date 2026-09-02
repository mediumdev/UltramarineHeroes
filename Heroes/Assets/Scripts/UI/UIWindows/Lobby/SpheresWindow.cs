using System.Collections.Generic;
using Configs;
using CoreUtils.Utils;
using Enums;
using Game.Controllers;
using UI.Windows;
using UnityEngine;

namespace UI.UIWindows.Lobby
{
    public class SpheresWindow : Window
    {
        [SerializeField] private RectTransform _topSpheresContainer;
        [SerializeField] private RectTransform _botSpheresContainer;
        [SerializeField] private SphereItem _item;
    
        private readonly List<CurrencyConfig> _list = new List<CurrencyConfig>();
    
        private void OnEnable()
        {
            CreateList();
            CreateSectionPanel();
        }
    
        private void CreateList()
        {
            _list.Clear();
        
            foreach (var sphere  in CurrencyManager.Instance.CurrencyConfigsWithType(CurrencyType.Sphere))
            {
                _list.Add(sphere);
            }
        }
    
        private void CreateSectionPanel()
        {
            _topSpheresContainer.Clear();
            _botSpheresContainer.Clear();

            for (var i = 0; i <= _list.Count; i++)
            {
                var value = CurrencyManager.Instance.CurrencyValue(_list[i]);
                if (i <= 3)
                {
                    var item = Instantiate(_item, _topSpheresContainer);
                    item.Init(_list[i], value);
                }
                else
                {
                    var item = Instantiate(_item, _botSpheresContainer);
                    item.Init(_list[i], value);
                }
            }
        }
    }
}
