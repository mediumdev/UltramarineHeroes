using Game.Units;
using UnityEngine;

namespace UI
{
    public class UnitInfo : MonoBehaviour
    {
        [SerializeField] private HealthBar _healthBar;
        [SerializeField] private HealthBar _shieldBar;

        private UnitController _unitController;
        private PlayerPanel _playerPanel;
        private Camera _cam;
        private bool _subscribed;

        public void Init(UnitController unit)
        {
            _unitController = unit;
            _cam = Camera.main;
            _healthBar.SetMaxHealth(unit.MaxHealth);
            _unitController.MaxHealthChanged += RepaintMaxHealth;
            _unitController.HealthChanged += RepaintHealth;
            _unitController.ShieldChanged += ShieldChange;
            _unitController.DeathEvent += Death;
            RepaintPosition();
        }

        private void Update()
        {
            if (!_unitController.gameObject.activeSelf) Hide();
            RepaintPosition();
        }

        private void RepaintPosition()
        {
            var screenPos = _cam.WorldToScreenPoint(_unitController.transform.position);
            var x = screenPos.x;
            var y = screenPos.y;
            transform.position = new Vector2(x + _unitController.UnitConfig.PositionX, y + 100 + _unitController.UnitConfig.PositionY);
        }

        private void OnEnable()
        {
            if (_unitController == null || _subscribed)
                return;

            _subscribed = true;
            _unitController.MaxHealthChanged += RepaintMaxHealth;
            _unitController.HealthChanged += RepaintHealth;
            _unitController.ShieldChanged += ShieldChange;
            _unitController.DeathEvent += Death;
        }

        private void OnDisable()
        {
            _unitController.HealthChanged -= RepaintHealth;
            _unitController.DeathEvent -= Death;
            _unitController.MaxHealthChanged -= RepaintMaxHealth;
            _unitController.ShieldChanged -= ShieldChange;
            _subscribed = false;
        }

        private void RepaintHealth(int value)
        {
            _healthBar.SetHealth(value);
        }
        
        private void RepaintMaxHealth(int value)
        {
            _healthBar.SetMaxHealth(value);
        }

        public void ShieldInit(int value)
        {
            _shieldBar.gameObject.SetActive(true);
            _shieldBar.SetMaxHealth(value);
        }

        private void ShieldChange(int value)
        {
            _shieldBar.SetHealth(value);
            if (value <= 0)
                _shieldBar.gameObject.SetActive(false);
        }
        
        private void Death()
        {
            Destroy(gameObject);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }
    }
}
