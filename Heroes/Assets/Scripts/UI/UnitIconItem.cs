using System;
using System.Collections;
using Configs;
using DG.Tweening;
using Enums;
using Game.Controllers;
using Game.Pool;
using Game.Units;
using Network;
using Photon.Pun;
using RTLTMPro;
using TMPro;
using UI.UIWindows.Faction;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [RequireComponent(typeof(PhotonView))]
    public class UnitIconItem : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _cost;
        [SerializeField] private TextMeshProUGUI _limit;
        [SerializeField] private RTLTextMeshPro _unitTitle;
        [SerializeField] private Image _icon;
        [SerializeField] private Image _greyShade;
        [SerializeField] private Image _glowBorder;
        [SerializeField] private Button _button;
        [SerializeField] private PhotonView _photonView;
        [SerializeField] private Image _unavailable;
        [SerializeField] private FactionItem _factionItem;

        public event Action SpawnEvent;
        
        private UnitConfig _unitConfig;
        private LineType _type;
        private PlayerType _player;
        private Tower _tower;
        private Sequence _seq;
        private PlayerController _playerController;
        private bool _isActive = true;
        private float _time = 0f;
        private readonly float _cooldowntime = 1f;
        private Coroutine _coroutine;

        public void Init(UnitConfig unit, LineType type)
        {
            _player = PhotonNetwork.IsMasterClient ? PlayerType.Player : PlayerType.Enemy;
            _playerController = _player == PlayerType.Player 
                ? GameController.Instance.Player 
                : GameController.Instance.Enemy;
            _playerController.ManaChangeEvent += OnManaChange;
            _tower = _playerController.GetTower(type);
            _unitConfig = unit;
            
            _unitTitle.text = _unitConfig.Name;
            _icon.sprite = _unitConfig.Icon;
            _type = type;

            Repaint();
            OnManaChange(0);
        }

        private void OnEnable()
        {
            RoomServer.Instance.SpawnEvent += RepaintCounter;
        }

        private void OnDisable()
        {
            RoomServer.Instance.SpawnEvent -= RepaintCounter;
            if (_playerController != null)
                _playerController.ManaChangeEvent -= OnManaChange;

            if (_seq != null && _seq.IsPlaying())
                _seq.Kill();
        }

        private void Repaint()
        {
            RepaintCounter();
            
            if (_factionItem == null) return;
            _factionItem.RepaintBorders();
        }

        private void OnManaChange(int i)
        {
            if (!CheckAvailable())
                return;
            
            var controller = _player == PlayerType.Player
                ? GameController.Instance.Player
                : GameController.Instance.Enemy;
            var value = controller.Mana;
            
            var realCost = _unitConfig.Cost - (_tower.IsDiscountActive ? _tower.ManaDiscount : 0);
            _button.interactable = realCost <= value;
            _greyShade.gameObject.SetActive(realCost > value);

            SetButton(realCost <= value);
        }

        public void RepaintCounter()
        {
            _limit.text = _unitConfig.IsInfinite ? "∞" : GameController.Instance.LimitController.GetValue(_player, _unitConfig).ToString();
            var price = _unitConfig.Cost - (_tower.IsDiscountActive ? _tower.ManaDiscount : 0);
            _cost.text = (price > 0 ? price : 0).ToString();
        }
        
        private bool CheckAvailable()
        {
            if ((_unitConfig.IsInfinite ||
                 GameController.Instance.LimitController.GetValue(_player, _unitConfig) > 0) && _isActive) return true;

            _button.interactable = false;
            _greyShade.gameObject.SetActive(true);
            SetButton(false);
            return false;
        }

        private void SetButton(bool active)
        {
            _seq = DOTween.Sequence();
            _seq.Append(_glowBorder.DOFade(active ? 1f : 0f, 0.2f));
            _seq.Insert(0,_button.gameObject.transform.DOLocalMoveY(active ? 10f : 0, 0.2f));
        }

        public void SpawnClick()
        {
            UnitPool.Instance.Spawn(_unitConfig, _player, _type);
            RepaintCounter();
            SpawnEvent?.Invoke();
            OnManaChange(0);
        }

        public void Cooldown()
        {
            _time += Time.deltaTime;
            _isActive = false;
            OnManaChange(0);
            _coroutine = StartCoroutine( GetCooldown());
            var seq = DOTween.Sequence();
            seq.AppendInterval(1f).AppendCallback(() =>
            {
                _isActive = true;
                OnManaChange(0);
            });
        }

        private IEnumerator GetCooldown()
        {
            _time = _cooldowntime;
            
            while (_time > 0)
            {
                _time -= Time.deltaTime;
                _unavailable.fillAmount = _time / _cooldowntime;
                yield return null;
            }
        }
        
        private void OnValidate()
        {
            if (_photonView == null)
                _photonView = GetComponent<PhotonView>();
        }
    }
}
