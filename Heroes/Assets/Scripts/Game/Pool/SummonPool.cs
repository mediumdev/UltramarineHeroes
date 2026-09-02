using System.Collections.Generic;
using System.Linq;
using Configs;
using Enums;
using Game.Summons;
using Network;
using PhotonUtils;
using Pool;
using UnityEngine;

namespace Game.Pool
{
    public class SummonPool : TransformPool<Obstacle, SummonPool>
    {
        private readonly List<Obstacle> _spawnedObstacles = new List<Obstacle>();
        
        public override Obstacle Pop(Obstacle origin)
        {
            var obj = PopInternal(origin);
            obj.transform.SetParent(null);
            obj.gameObject.SetActive(true);
            _spawnedObstacles.Add(obj);
            return obj;
        }
        
        public override Obstacle Pop(Obstacle origin, Transform parent, bool show = true)
        {
            var obj = base.Pop(origin, parent, show);
            _spawnedObstacles.Add(obj);
            return obj;
        }
        
        public override void Push(Obstacle obj)
        {
            var pushedObj = PushInternal(obj);
            pushedObj.transform.SetParent(transform);
            _spawnedObstacles.Remove(pushedObj);
        }

        public void Push(int viewId)
        {
            var target = _spawnedObstacles.FirstOrDefault(x => x.View.ViewID == viewId);
            if (target != null)
                Push(target);
        }
        public void Spawn(ObstacleConfig obstacle, int cellX, PlayerType playerType, LineType lineType, int radius, int damage)
        {
            object[] data = { obstacle.Uid, cellX, playerType, lineType, radius, damage };
            PhotonSingleton.Instance.RaiseEvent((byte) NetworkEvents.Summon, data);
        }
    }
}
