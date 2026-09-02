using Configs;
using Enums;
using Game.Controllers;
using Packages.CoreUtils.Utils;
using Photon.Pun;
using UnityEngine;

namespace Game.Units
{
    public class Tower : MonoBehaviour, IPunObservable
    {
        [SerializeField] private TowerConfig _config;
        [SerializeField] private float _xOffset;
        [SerializeField] private float _yOffset;
        [SerializeField] private ObjectButton _button;
        [SerializeField] private bool _isMaster;
        [SerializeField] private FactionConfig _faction;
        [SerializeField] private Animation _animation;
        private float _timerBoost = 0f;
        public float ActiveAbilityCooldown { get; private set; }
        public float PassiveAbilityCooldown { get; private set; }
        public int ManaDiscount { get; set; }
        public bool IsDiscountActive { get; set; }
        public Animation Animation => _animation;

        private void OnEnable()
        {
            var player = _isMaster && PhotonNetwork.IsMasterClient || !_isMaster && !PhotonNetwork.IsMasterClient;
            if (!player)
                Destroy(_button);
        }

        public void StartActiveAbilityCooldown()
        {
            ActiveAbilityCooldown = 0;
        }
        
        public void StartPassiveAbilityCooldown()
        {
            PassiveAbilityCooldown = 0;
        }

        public void Cast(PlayerType player, PlayerController controller)
        {
            if (controller.TowerAbilityIsCasting || ActiveAbilityCooldown < Faction.ActiveAbilityCooldown
                || controller.Mana < Faction.ActiveAbilityManaCost) return;
            controller.TowerAbilityIsCasting = true;
            
            StartActiveAbilityCooldown();
            var abilityRange = Faction.ActiveAbilityRange;
            GameController.Instance.GameMachine.CastTowerAbility(
                player == PlayerType.Player ? 0 : GameController.Instance.GameMachine.HorizontalCellCount - abilityRange, 
                player == PlayerType.Player ? abilityRange : GameController.Instance.GameMachine.HorizontalCellCount,  
                (int) _faction.FactionType,
                controller,
                true);
        }

        public void SetTimerBoost(float percentBonus)
        {
            _timerBoost = percentBonus;
        }

        private void Update()
        {
            if (_faction == null) return;
            
            if (_faction.PassiveAbilityCooldown > PassiveAbilityCooldown)
                PassiveAbilityCooldown += Time.deltaTime;
            
            if (_faction.ActiveAbilityCooldown > ActiveAbilityCooldown)
                ActiveAbilityCooldown += Time.deltaTime * (1 + _timerBoost);
        }

        public int ResourcePerTick => _config.ResourcePerTick;
        public LineType LineType => _config.LineType;
        public Vector3 Offset => new Vector3(_xOffset, _yOffset);
        public FactionConfig Faction
        {
            get => _faction;
            set
            {
                _faction = value;
                ActiveAbilityCooldown = _faction.ActiveAbilityCooldown - 1;
            }
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting && PhotonNetwork.IsMasterClient)
            {
                stream.SendNext(ActiveAbilityCooldown);
            }
            else if (stream.IsReading)
            {
                ActiveAbilityCooldown = (float)stream.ReceiveNext();
            }
        }
    }
}