using Enums;
using Game.Controllers;
using UnityEngine;

namespace Configs.TowerAbilities
{
    public class ChangeManaAbility : AbilityConfig
    {
        [SerializeField] private int _value;
        public override void CastNoTarget(PlayerController controller, LineType lineType)
        {
            var target = controller;
            switch (_targetType)
            {
                case AbilityTargetType.Self:
                    target = controller.PlayerType == PlayerType.Player 
                        ? GameController.Instance.Player 
                        : GameController.Instance.Enemy;
                    break;
                case AbilityTargetType.Enemy:
                    target = controller.PlayerType == PlayerType.Player 
                        ? GameController.Instance.Enemy 
                        : GameController.Instance.Player;
                    break;
            }
            Debug.Log(target + "Mana changed by" + _value);
            target.ChangeMana(_value);
        }

#if UNITY_EDITOR
        [UnityEditor.MenuItem("Assets/Create/Configs/TowerAbilities/ChangeManaAbility")]
        private static void Create()
        {
            CreateAsset<ChangeManaAbility>();
        }
#endif
    }
}