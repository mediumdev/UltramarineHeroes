using Game.Controllers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.UIWindows.ChangeAvatar
{
    public class LobbyWindowPlayerData : MonoBehaviour
    {
        [SerializeField] private Image _avatarImage;
        [SerializeField] private TextMeshProUGUI _nameText;

        public void Init()
        {
            CustomizationManager.Instance.AvatarChangedEvent += Repaint;
            CustomizationManager.Instance.PlayerNameChangedEvent += Repaint;   
        }

        private void OnDisable()
        {
            CustomizationManager.Instance.AvatarChangedEvent -= Repaint;
            CustomizationManager.Instance.PlayerNameChangedEvent -= Repaint;
        }

        protected void Repaint()
        {
            _avatarImage.sprite = CustomizationManager.Instance.GetAvatarImage();
            _nameText.text = CustomizationManager.Instance.GetPlayerName();
        }
    }
}