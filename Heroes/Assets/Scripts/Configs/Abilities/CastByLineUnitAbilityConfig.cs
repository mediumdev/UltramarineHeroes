using System.Collections.ObjectModel;
using Enums;
using Game.Controllers;
using Game.Units;
using UnityEngine;

namespace Configs.Abilities
{
    public class CastByLineUnitAbilityConfig : AbilityConfig
    {
        [SerializeField] private AbilityConfig _ability;
        [SerializeField] private int _range;
        [SerializeField] private bool _fullLine;
        public override void Cast(UnitController source)
        {
            if (_type == AbilityType.Death) return;
            DoAction(source);
        }
        
        public override void CastDeath(UnitController source)
        {
            if (_type != AbilityType.Death) return;
            StartFx(source.transform);
            DoAction(source);
        }

        private void DoAction(UnitController source)
        {
            int minX;
            int maxX;
            var border = GameController.Instance.GameMachine.HorizontalCellCount - 1;
            if (source.PlayerType == PlayerType.Player)
            {
                var rangePoint = source.CurrentCellX + _range;
                minX = _fullLine? 0 : source.CurrentCellX;
                maxX = _fullLine? border : (rangePoint < border)? rangePoint : border;
            }
            else
            {
                var rangePoint = source.CurrentCellX - _range;
                minX = _fullLine? 0 : (rangePoint > 0)? rangePoint : 0;
                maxX = _fullLine? border : source.CurrentCellX;
            }
             
            var affectedUnits = new Collection<UnitController>();
            for (var j = minX; j < maxX; j++)
            {
                if (!GameController.Instance.GameMachine.Cells[j, (int) source.CurrentLine].ContainsUnit()) continue;
                foreach (var unit in GameController.Instance.GameMachine.Cells[j, (int) source.CurrentLine].CellUnitCollection)
                {
                    switch (_ability.TargetType)
                    {
                        case AbilityTargetType.All:
                            affectedUnits.Add(unit);
                            break;
                        case AbilityTargetType.Enemy:
                        case AbilityTargetType.EnemyCloser:
                        {
                            if (unit.PlayerType != source.PlayerType) affectedUnits.Add(unit);
                            break;
                        }
                        case AbilityTargetType.Self:
                        {
                            if (unit.PlayerType == source.PlayerType) affectedUnits.Add(unit);
                            break;
                        }
                    }
                }
            }

            if (_ability.TargetType == AbilityTargetType.NoTarget)
            {
                _ability.CastNoTarget(source.PlayerType == PlayerType.Player? GameController.Instance.Player : GameController.Instance.Enemy, source.CurrentLine);
                return;
            }

            foreach (var unit in affectedUnits)
            {
                _ability.Cast(unit);
            }
        }
        
#if UNITY_EDITOR
        [UnityEditor.MenuItem("Assets/Create/Configs/Abilities/CastAbilityByLine")]
        private static void Create()
        {
            CreateAsset<CastByLineUnitAbilityConfig>();
        }
#endif
    }
}