using System.Collections.Generic;
using System.Linq;
using Configs;
using Enums;
using Game.Units;
using Network;
using PhotonUtils;
using Pool;
using UnityEngine;

namespace Game.Pool
{
    public class UnitPool : TransformPool<UnitController, UnitPool>
    {
        private List<UnitController> _spawnedUnits = new List<UnitController>();
        
        public override UnitController Pop(UnitController origin)
        {
            var obj = PopInternal(origin);
            obj.transform.SetParent(null);
            obj.gameObject.SetActive(true);
            _spawnedUnits.Add(obj);
            return obj;
        }
        
        public override UnitController Pop(UnitController origin, Transform parent, bool show = true)
        {
            var obj = base.Pop(origin, parent, show);
            _spawnedUnits.Add(obj);
            return obj;
        }
        
        public override void Push(UnitController obj)
        {
            var pushedObj = PushInternal(obj);
            pushedObj.transform.SetParent(transform);
            _spawnedUnits.Remove(pushedObj);
            // Destroy(pushedObj);
        }

        public void Push(int viewId)
        {
            var target = _spawnedUnits.FirstOrDefault(x => x.View.ViewID == viewId);
            if (target != null)
                Push(target);
        }

        public void Spawn(UnitConfig unitConfig, PlayerType playerType, LineType lineType, float health = 1, int position = -1, bool isSummon = false)
        {
            object[] data = { unitConfig.Uid, playerType, lineType, health, position, isSummon };
            PhotonSingleton.Instance.RaiseEvent((byte) NetworkEvents.Spawn, data);
        }
    }
}
