using Game.Units;

namespace Configs.TowerAbilities
{
    public class TestTowerAbility : AbilityConfig
    {
        public override void Cast(UnitController source)
        {
            
        }
        
#if UNITY_EDITOR
        [UnityEditor.MenuItem("Assets/Create/Configs/TowerAbilities/TestTowerAbility")]
        private static void Create()
        {
            CreateAsset<TestTowerAbility>();
        }
#endif
    }
}
