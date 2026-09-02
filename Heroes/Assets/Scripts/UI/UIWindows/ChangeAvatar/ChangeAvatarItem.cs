using Configs;
using Game.Controllers;
using UnityEngine;
using UnityEngine.UI;

namespace UI.UIWindows.ChangeAvatar
{
    public class ChangeAvatarItem : MonoBehaviour
    {
        [SerializeField] private Image _unitImage;
        [SerializeField] private Image _lockedMask;
        [SerializeField] private GameObject _border;
        
        private UnitConfig _config;
        private bool _enableRepaint;
        private bool _canBeActivated;

        private void OnEnable()
        {
            if (_enableRepaint) Repaint();
        }

        public void Init(UnitConfig config, bool canBeActivated)
        {
            _config = config;
            _canBeActivated = canBeActivated;
            
            Repaint();
        }

        private void Repaint()
        {
            _unitImage.sprite = _config.Icon;
            _lockedMask.gameObject.SetActive(!_canBeActivated);
            _border.SetActive(CustomizationManager.Instance.CurrentAvatarImage == _config.Icon);
            _enableRepaint = true;
        }

        public void SetAvatarImage()
        {
            if (!_canBeActivated) return;
            
            CustomizationManager.Instance.SetAvatarImage(_config);
        }
    }
}