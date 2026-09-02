using System;
using Enums;
using Game.Pool;
using Pool;
using UnityEngine;

namespace Game.Units
{
    public class Projectile : MonoBehaviour, IPoolObject
    {
        [SerializeField] private int _preloadCount;
        [SerializeField] private bool _isBallistic;
        [SerializeField] private float _defaultSpeed = 4; 
        [SerializeField] private float _speedIncreaseFactor = 1.2f; 
        [SerializeField] private float _speedDecreaseFactor = 0.8f;

        [Header("Ballistic Settings")]
        [Range(0.01f, 1.0f)] [SerializeField] private float _positionFactor; 
        [Range(0.01f, 1.0f)] [SerializeField] private float _heightFactor = 1;
        
        [Header("Projectile Displacement")]
        [SerializeField] private float _positionX;
        [SerializeField] private float _positionY;
        
        [Header("FX Settings")]
        [SerializeField] private FxPlayer _fx;
        [SerializeField] protected Vector3 _fxPosition = Vector3.zero;
        [SerializeField] protected Vector3 _fxScale = Vector3.one;
        private GameObject _target;
        private GameObject _source;
        private EntryFX _trailFx;
        private bool _isHit;
        private Vector3 _sourceStartPosition;
        private float _distance;
        private GameObject _supportingPoint;
        private Vector3 _lastPos;
        private bool _isPlayer;

        private bool TargetIsUnit => _targetController != null;
        private UnitController _targetController;
        
        #region Pool

        public IPoolObject Origin { get; private set; }
        public IPoolObject LoadObject(IPoolObject origin)
        {
            var obj = Instantiate((Projectile) origin);
            obj.Origin = origin;
            return obj;
        }

        public int PreloadCount => _preloadCount;

        public void OnPop()
        {
            _isHit = false;
        }
        
        public void OnPush()
        {
        }

        #endregion

        private void OnValidate()
        {
            var trail = GetComponent<EntryFX>();
            if (trail)
                _trailFx = trail;
        }

        public void Init(GameObject target, GameObject source, PlayerType playerType, bool ballistic, Vector3 specialSpawnPosition)
        {
            _source = source;
            
            // Если изначально это баллистическая стрела, то она может перестать лететь по баллистической траектории
            // в случае выстрела вверх или вниз
            if (_isBallistic)
                _isBallistic = ballistic;

            _isPlayer = playerType == PlayerType.Player;
            if (!_isPlayer)
                _positionX = -_positionX;
            
            _sourceStartPosition = specialSpawnPosition.magnitude == 0 ? GetCenter(source.transform.position) : specialSpawnPosition;
            transform.position = _sourceStartPosition;
            
            transform.rotation = _isBallistic 
                ? Quaternion.Euler(180f, 180f,!_isPlayer ? 150f : 30f)
                : Quaternion.Euler(180f, 180f,!_isPlayer ? 0f : 180f);
            
            _target = target;
            _targetController = _target.GetComponent<UnitController>();

            if (!_isPlayer)
                _positionX = -_positionX;
            var targetTransformPosition = GetCenter(_target.transform.position);
            _distance = Vector3.Distance(_sourceStartPosition, targetTransformPosition);
            
            var supportingPointPositionX = _sourceStartPosition.x < targetTransformPosition.x ?
                _sourceStartPosition.x + _distance * _positionFactor :
                _sourceStartPosition.x - _distance * _positionFactor;
            var supportingPointPositionY = _sourceStartPosition.y + _distance * _heightFactor;
            
            _supportingPoint = new GameObject();
            _supportingPoint.transform.position = new Vector3(supportingPointPositionX, supportingPointPositionY, 0);
            _lastPos = _supportingPoint.transform.position;
        }
        
        private Vector3 GetCenter(Vector3 position)
        {
            return new Vector3(position.x + _positionX, position.y + _positionY, position.z);
        }

        private void Push()
        {
            if (_trailFx != null) _trailFx.StopFx();
            ProjectilePool.Instance.Push(this);
        }

        private void Update()
        {
            if (_source == null || _target == null)
            {
                Push();
                return;
            }

            var projectilePosition = transform.position;
            var targetPosition = TargetIsUnit 
                ? _targetController.GetTargetPoint() 
                : GetCenter(_target.transform.position);
            
            if (_isBallistic)
            {
                var supportingPointPosition = _supportingPoint.transform.position;
                var speed = projectilePosition.y <= supportingPointPosition.y ? _defaultSpeed * _speedDecreaseFactor : _defaultSpeed * _speedIncreaseFactor;
                var maxDistanceDelta = speed * Time.deltaTime;
                
                var direction = (new Vector2(projectilePosition.x, projectilePosition.y) - new Vector2(_lastPos.x,_lastPos.y)).normalized;
                var angle = Mathf.Acos(direction.x / direction.magnitude) * Mathf.Rad2Deg * (direction.y <= 0 ? -1f : 1f)+ 180f;
                var newRotation = Quaternion.Euler(180f, 180f, angle);
                transform.rotation = newRotation;
                
                _lastPos = supportingPointPosition;
                supportingPointPosition = Vector3.MoveTowards(supportingPointPosition, targetPosition, maxDistanceDelta);
                _supportingPoint.transform.position = supportingPointPosition;
                projectilePosition = Vector3.MoveTowards(projectilePosition, supportingPointPosition, maxDistanceDelta);
                transform.position = projectilePosition;
            }
            else
            {
                var dir = new Vector2(targetPosition.x, targetPosition.y) - new Vector2(transform.position.x,transform.position.y);
                var angle = Mathf.Acos(dir.x / dir.magnitude) * Mathf.Rad2Deg * (dir.y <= 0 ? -1f : 1f) + 180f;
                transform.rotation = Quaternion.Euler(0f, 0f, angle);
                
                transform.position = Vector3.MoveTowards(projectilePosition, targetPosition, Time.deltaTime * _defaultSpeed);
            }

            if (_isHit) return;
            if (Vector3.Distance(targetPosition, transform.position) < 0.1f)
            {
                _isHit = true;
                if (_fx != null)
                {
                    var rotation = _fx.transform.rotation.eulerAngles;
                    _fx.Create(_target.transform, _fxPosition, _fxScale, rotation);
                }
                Destroy(_supportingPoint);
                Push();
            }
        }
    }
}