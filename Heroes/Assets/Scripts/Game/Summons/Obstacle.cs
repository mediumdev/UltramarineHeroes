using System.Collections;
using Configs;
using Enums;
using ExitGames.Client.Photon;
using Game.Controllers;
using Game.Units;
using Network;
using Photon.Pun;
using Photon.Realtime;
using PhotonUtils;
using Pool;
using UnityEngine;

namespace Game.Summons
{
    [RequireComponent(typeof(PhotonView), typeof(PhotonTransformView))]
    public class Obstacle : MonoBehaviour, ISummon, IPoolObject
    {
        [SerializeField] private FxPlayer _fx;
        [SerializeField] private ObstacleConfig _obstacleConfig;
        [SerializeField] private PhotonView _photonView;
        public PhotonView View => _photonView;
        public ObstacleConfig ObstacleConfig => _obstacleConfig;
        private int _obstacleOffset;
        public PlayerType PlayerType { get; private set; }
        private int _radius;
        private int _cellX;
        private int _cellY;
        private int _damage;
        private int _counter;
        
        public void Create(PlayerType playerType, int cellX, int line, int radius, int damage)
        {
            PlayerType = playerType;
            _cellX = cellX;
            _cellY = line;
            _radius = radius;
            _damage = damage;
            _counter = _obstacleConfig.Counter; 
            
            transform.position = GameController.Instance.GameMachine.Cells[_cellX, _cellY].transform.position;
            
            _obstacleOffset = GameController.Instance.GameMachine.Cells[_cellX, _cellY].SetObstacleOffset();
               
            var placeOffset = 0f;
            switch (_obstacleOffset)
            {
                case 0:
                    placeOffset = 0;
                    break;
                case 1:
                    placeOffset = 0.55f;
                    break;
                case 2:
                    placeOffset = -0.55f;
                    break;
            }

            var obstacleTransform = transform;
            var pos = obstacleTransform.localPosition;
            obstacleTransform.localPosition = new Vector3(pos.x, pos.y, pos.z + placeOffset);

            switch (_obstacleConfig.ObstacleType)
            {
                case ObstacleType.Bomb:
                    break;
                case ObstacleType.Dot:
                    StartCoroutine(nameof(Timer));
                    break;
            }
            
            AddObstacle();
        }
        
        private void AddObstacle()
        {
            for (var i = 0; i < _radius; i++)
            {
                GameController.Instance.GameMachine.AddObstacle(this, _cellX, _cellY);
            }
        }

        public virtual void Impact(UnitController unit)
        {
            StartFx(unit.transform);
            unit.TakeDamage(_damage);

            if (_obstacleConfig.ObstacleType == ObstacleType.Bomb)
                Bomb();
        }

        private void Bomb()
        {
            _counter--;
            if (_counter <= 0)
                RemoveObstacle();
        }

        private IEnumerator Timer()
        {
            yield return new WaitForSeconds(_counter);
            RemoveObstacle();
        }

        private void RemoveObstacle()
        {
            for (var i = 0; i < _radius; i++)
            {
                GameController.Instance.GameMachine.RemoveObstacle(this, _cellX, _cellY);
                GameController.Instance.GameMachine.Cells[_cellX, _cellY].RemoveObstacleOffset(_obstacleOffset);
            }

            Push();
        }

        private void StartFx(Transform target)
        {
            if (!_fx) return;
            _fx.Create(target, Vector3.zero, Vector3.one, Vector3.zero);
        }

        public IPoolObject Origin { get; set; }
        public IPoolObject LoadObject(IPoolObject origin)
        {
            var obstacle = Instantiate((Obstacle) origin);
            obstacle.Origin = origin;

            PhotonView photonView = obstacle.GetComponent<PhotonView>();
            
            if (PhotonNetwork.AllocateViewID(photonView))
            {
                var obstacleTransform = obstacle.transform;
                object[] data = {
                    obstacle._obstacleConfig.Uid, obstacleTransform.position, obstacleTransform.rotation, photonView.ViewID 
                };

                RaiseEventOptions raiseEventOptions = new RaiseEventOptions
                {
                    Receivers = ReceiverGroup.Others,
                    CachingOption = EventCaching.AddToRoomCache
                };

                SendOptions sendOptions = new SendOptions
                {
                    Reliability = true
                };

                PhotonNetwork.RaiseEvent((byte)NetworkEvents.InstantiateSummon, data, raiseEventOptions, sendOptions);
            }
            return obstacle;
        }

        public int PreloadCount => _obstacleConfig.PreloadCount;
        public void OnPop()
        {
        }

        public void OnPush()
        {
        }
        
        private void Push()
        {
            object[] data = {
                _photonView.ViewID
            };
            
            PhotonSingleton.Instance.RaiseEvent((byte) NetworkEvents.DestroySummon, data);
        }
    }
}