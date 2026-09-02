using CoreUtils.Utils;
using Game.Controllers;
using RTLTMPro;
using TMPro;
using UI.Windows;
using UnityEngine;
using UnityEngine.UI;

namespace UI.UIWindows.ChangeAvatar
{
    public class ChangeAvatarWindow : Window
    {
        [SerializeField] private ChangeAvatarItem _avatarItem;
        [SerializeField] private RectTransform _avatarContainer;
        [SerializeField] private Image _uiAvatar;
        [SerializeField] private RTLTextMeshPro _nameField;
        [SerializeField] private TextMeshProUGUI _namePlaceholder;

        private void OnEnable()
        {
            CustomizationManager.Instance.AvatarChangedEvent += RepaintAvatars;
            CustomizationManager.Instance.PlayerNameChangedEvent += RepaintName;
            
            RepaintAvatars();
            RepaintName();
        }

        private void OnDisable()
        {
            CustomizationManager.Instance.AvatarChangedEvent -= RepaintAvatars;
            CustomizationManager.Instance.PlayerNameChangedEvent -= RepaintName;
        }

        private void RepaintAvatars()
        {
            _avatarContainer.Clear();
            foreach (var unit in CustomizationManager.Instance.UnlockedUnits)
            {
                var item = Instantiate(_avatarItem, _avatarContainer);
                item.Init(unit, true);
            }
            
            foreach (var unit in CustomizationManager.Instance.AllUnits)
            {
                if (CustomizationManager.Instance.UnlockedUnits.Contains(unit)) continue;
                
                var item = Instantiate(_avatarItem, _avatarContainer);
                item.Init(unit, false);
            }

            _uiAvatar.sprite = CustomizationManager.Instance.GetAvatarImage();
        }

        private void RepaintName()
        {
            _namePlaceholder.text = CustomizationManager.Instance.GetPlayerName();
        }

        public void SetPlayerName()
        {
            if (_nameField.text.Length == 0) return;
            
            CustomizationManager.Instance.SetPlayerName(_nameField.text);
        }

        public void CloseWindow()
        {
            Close();
        }
    }
}
