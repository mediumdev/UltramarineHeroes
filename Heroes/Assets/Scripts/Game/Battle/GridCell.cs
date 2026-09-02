using System.Collections.Generic;
using System.Linq;
using Configs;
using Enums;
using Game.Summons;
using Game.Units;
using UnityEngine;

namespace Game.Battle
{
    public class GridCell : MonoBehaviour
    {
        [SerializeField] private Color _colorGizmoObject;
        [SerializeField] private GridCellElement[] _gridCellElements;
        [SerializeField] public List<UnitController> CellUnitCollection;
        public List<Obstacle> CellObstacleCollection;
        private List<bool> _obstacleOffset;

        public void AddToCellUnitCollection(UnitController unitController)
        {
            if (CanAddNewUnit(unitController.UnitConfig))
            {
                CellUnitCollection.Add(unitController);
                _gridCellElements.FirstOrDefault(x => !x.ContainsUnit())?.AddUnit(unitController);
            }
        }

        public void AddToCellObstacleCollection(Obstacle obstacle)
        {
            CellObstacleCollection.Add(obstacle);
            _gridCellElements.FirstOrDefault(x => !x.ContainsObstacle())?.AddObstacle(obstacle);
        }

        public void RemoveFromCellUnitCollection(UnitController unitController)
        {
            CellUnitCollection.Remove(unitController);
            _gridCellElements.FirstOrDefault(x => x.CellUnit == unitController)?.RemoveUnit();
        }

        public void RemoveFromCellObstacleCollection(Obstacle obstacle)
        {
            CellObstacleCollection.Remove(obstacle);
            _gridCellElements.FirstOrDefault(x => x.CellObstacle == obstacle)?.RemoveObstacle();
        }

        private void Start()
        {
            CellUnitCollection = new List<UnitController>();
            CellObstacleCollection = new List<Obstacle>();

            _obstacleOffset = new List<bool>();
            for (var i = 0; i <= 2; i++)
            {
                _obstacleOffset.Add(false);
            }
        }

        public bool CanAddNewUnit(UnitConfig unitConfig)
        {
            var emptyCells = CountEmptyCells();
            switch (unitConfig.UnitSize)
            {
                case UnitSize.Big:
                    if (emptyCells == 3 || (CellUnitCollection.All(x => x.UnitConfig.AttackType == AttackType.Range) &&
                                            unitConfig.AttackType != AttackType.Range))
                        return true;
                    break;
                case UnitSize.Small:
                    if (emptyCells >= 1 && CellUnitCollection.All(x => x.UnitConfig.UnitSize != UnitSize.Big)
                        || (CellUnitCollection.Any(x => x.UnitConfig.AttackType == AttackType.Range)
                            && unitConfig.AttackType != AttackType.Range))
                        return true;
                    break;
                default:
                    return false;
                    break;
            }

            return false;
        }

        public int CountEmptyCells()
        {
            return _gridCellElements.ToList().FindAll(x => !x.ContainsUnit()).Count;
        }

        public bool ContainsUnit()
        {
            return CellUnitCollection.Count > 0;
        }

        public bool ContainsObstacle(UnitController unit)
        {
            return CellObstacleCollection.Count > 0 &&
                   CellObstacleCollection.FirstOrDefault(obstacle => obstacle.PlayerType != unit.PlayerType);
        }

        public GridCellElement EmptyGridCellElement()
        {
            return _gridCellElements.FirstOrDefault(x => x.CellUnit == null);
        }

        public Obstacle GetObstacle(UnitController unit)
        {
            return CellObstacleCollection.FirstOrDefault(obstacle => obstacle.PlayerType != unit.PlayerType);
        }

        public int SetObstacleOffset()
        {
            for (var i = 0; i <= _obstacleOffset.Count; i++)
            {
                var pos = i;
                if (_obstacleOffset[pos]) continue;
                _obstacleOffset[pos] = true;
                return pos;
            }

            return 0;
        }

        public void RemoveObstacleOffset(int place)
        {
            _obstacleOffset[place] = false;
        }

        private void OnDrawGizmos()
        {
            var selfTransform = transform;
            var position = selfTransform.position;
            var localScale = selfTransform.localScale;
            Gizmos.color = _colorGizmoObject;
            Gizmos.DrawCube(new Vector3(position.x, position.y + localScale.y / 2, position.z), localScale);
        }
    }
}