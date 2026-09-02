using System.Collections.Generic;
using Enums;
using Game.Controllers;
using Game.Units;
using UnityEngine;

namespace Configs.TowerAbilities
{
    public class PushBackTowerAbility : AbilityConfig
    {
        [SerializeField] private int _force;
        [SerializeField] private FxPlayer _fxOnTarget;
        [SerializeField] private Vector3 _fxOnTargetPosition = Vector3.zero;
        public override void Cast(UnitController source)
        {
            StartFxOnUnit(new List<Transform> {source.transform}, source.PlayerType == PlayerType.Player);
            GameController.Instance.GameMachine.Cells[source.CurrentCellX, (int) source.CurrentLine].RemoveFromCellUnitCollection(source);
            
            if (source.PlayerType == PlayerType.Player)
                if (source.CurrentCellX - _force > 0)
                    source.CurrentCellX -= _force;
                else
                {
                    source.CurrentCellX = 0;
                }
            else
            {
                if (source.CurrentCellX + _force < GameController.Instance.GameMachine.HorizontalCellCount - 1)
                    source.CurrentCellX += _force;
                else
                {
                    source.CurrentCellX = GameController.Instance.GameMachine.HorizontalCellCount - 1;
                }
            }
            source.PredictedPositionX = source.CurrentCellX;
            GameController.Instance.GameMachine.Cells[source.PredictedPositionX, (int) source.CurrentLine].AddToCellUnitCollection(source);
            
            GameController.Instance.GameMachine.ForcedMoveUnit(source);
        }
        
        private void StartFxOnUnit(IEnumerable<Transform> targets, bool rotateFx = false)
        {
            if (_fxOnTarget is null) return;

            foreach (var target in targets)
            {
                var position = _fxOnTargetPosition;
                var rotation = _fxOnTarget.transform.rotation.eulerAngles;
                if (rotateFx)
                {
                    rotation.y += 180;
                    position.x *= -1;
                }

                var fx = _fxOnTarget.Create(target, position, _fxScale, rotation);
                Debug.Log($"Cast FX {_fxOnTarget.gameObject.name} to transform {target.gameObject.name}. " +
                          $"rotateFx = {rotateFx}, position = {position.ToString("F3")}, rotation = {rotation.ToString("F3")}");
            }
        }

        
#if UNITY_EDITOR
        [UnityEditor.MenuItem("Assets/Create/Configs/TowerAbilities/PushBackTowerAbility")]
        private static void Create()
        {
            CreateAsset<PushBackTowerAbility>();
        }
#endif
    }
}
