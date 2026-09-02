using Configs;
using Enums;
using Game.Controllers;
using JetBrains.Annotations;
using Network;
using Photon.Pun;
using PhotonUtils;
using RTLTMPro;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [RequireComponent(typeof(PhotonView))]
    public class TowerAbilityIconItem : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _costText;
        [SerializeField] private RTLTextMeshPro _abilityTitle;
        [SerializeField] private Image _icon;
        [SerializeField] private Image _greyShade;
        [SerializeField] private Button _button;
        [SerializeField, HideInInspector] private PhotonView _photonView; //TODO: это можно спрятать в инспекторе, аннотация HideInInspector
        [SerializeField] private TextMeshProUGUI _cooldownTimer;
        
        private AbilityConfig _abilityConfig;
        private int _type;
        private PlayerType _player;
        private PlayerController _controller;

        public void Init(AbilityConfig ability, LineType type, int cost)
        {
            _abilityConfig = ability;
            _costText.text = cost.ToString();
            _abilityTitle.text = _abilityConfig.Name;
            _icon.sprite = _abilityConfig.Icon;
            _type = (int) type;
            _player = PhotonNetwork.IsMasterClient ? PlayerType.Player : PlayerType.Enemy; //TODO: это можно не прикапывать
            _controller = _player == PlayerType.Player //TODO: а вот это сравнение можно прикопать, чтобы не сравнивать каждый раз
                ? GameController.Instance.Player
                : GameController.Instance.Enemy;
        }
        
        private void Update()
        {
            var tower = _controller.Towers[_type];
            var isActive = tower.ActiveAbilityCooldown > tower.Faction.ActiveAbilityCooldown &&
                           _controller.Mana >= tower.Faction.ActiveAbilityManaCost;
            
            _button.interactable = isActive;
            _greyShade.gameObject.SetActive(!isActive);
            // _greyShade.fillAmount = 1 - tower.ActiveAbilityCooldown / tower.Faction.ActiveAbilityCooldown;
            
            if (tower.Faction.ActiveAbilityCooldown <= 0) return;

            var cooldown = (int) (tower.Faction.ActiveAbilityCooldown - tower.ActiveAbilityCooldown);
            _cooldownTimer.text = cooldown > 0 ? cooldown.ToString() : null;
        }

        [UsedImplicitly]
        public void OnClick()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                var tower = _controller.Towers[_type];
                tower.Cast(PlayerType.Player, _controller);
            }
            else
            {
                object[] data = { _type };
                PhotonSingleton.Instance.RaiseEvent((byte) NetworkEvents.AbilityClick, data);
            }
            
            foreach (var unitIconItem in transform.parent.GetComponentsInChildren<UnitIconItem>())
            {
                unitIconItem.RepaintCounter();
            }
        }

        private void OnValidate()
        {
            if (_photonView == null)
                _photonView = GetComponent<PhotonView>();
        }
    }
}