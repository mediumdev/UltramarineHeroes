using Enums;
using Game.Controllers;
using UnityEngine;

namespace Configs.TowerAbilities
{
    public class ManaDiscountTowerAbility : AbilityConfig
    {
        [SerializeField] private int _manaDiscount;
        public override void CastNoTarget(PlayerController controller, LineType lineType)
        {
            var unitPlayer = controller.PlayerType == PlayerType.Player 
                ? GameController.Instance.Player 
                : GameController.Instance.Enemy;
            var tower = unitPlayer.GetTower(lineType);
            if (tower.IsDiscountActive) return;
            tower.IsDiscountActive = true;
            tower.ManaDiscount = _manaDiscount;
        }

#if UNITY_EDITOR
        [UnityEditor.MenuItem("Assets/Create/Configs/TowerAbilities/ManaDiscountTowerAbility")]
        private static void Create()
        {
            CreateAsset<ManaDiscountTowerAbility>();
        }
#endif
    }
}
