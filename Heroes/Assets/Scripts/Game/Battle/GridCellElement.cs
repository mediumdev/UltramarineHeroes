using Game.Summons;
using Game.Units;
using UnityEngine;

namespace Game.Battle
{
    public class GridCellElement : MonoBehaviour
    {
        [SerializeField] private Color _colorGizmosObject;
        [SerializeField] private UnitController _cellUnit;
        [SerializeField] private  Obstacle _cellObstacle;
        private bool _obstacleOffset;
        private int _index;

        public UnitController CellUnit => _cellUnit;
        public Obstacle CellObstacle => _cellObstacle;

        public bool ContainsUnit()
        {
            return _cellUnit != null;
        }

        public bool ContainsObstacle()
        {
            return _cellObstacle != null;
        }

        public void AddUnit(UnitController unitController)
        {
            _cellUnit = unitController;
        }

        public void AddObstacle(Obstacle obstacle)
        {
            _cellObstacle = obstacle;
        }

        public bool CompareUnit(UnitController unitController)
        {
            return _cellUnit == unitController;
        }

        public bool CompareObstacle(Obstacle obstacle)
        {
            return _cellObstacle == obstacle;
        }

        public void RemoveObstacle()
        {
            _cellObstacle = null;
        }

        public void RemoveUnit()
        {
            _cellUnit = null;
        }

        private void OnDrawGizmos()
        {
            var selfTransform = transform;
            var position = selfTransform.position;
            var localScale = selfTransform.localScale;
            Gizmos.color = _colorGizmosObject;
            Gizmos.DrawCube(new Vector3(position.x, position.y + localScale.y / 2, position.z), localScale);
        }
    }
}