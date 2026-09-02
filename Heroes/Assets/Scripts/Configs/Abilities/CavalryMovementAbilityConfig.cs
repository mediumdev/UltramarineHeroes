using System;
using Game.Units;
using UnityEngine;

namespace Configs.Abilities
{
    public class CavalryMovementAbilityConfig : AbilityConfig
    {
        [SerializeField] private int _minSpeed;
        [SerializeField] private int _maxSpeed;
        [SerializeField] private float _chargeTime;

        public override void Cast(UnitController source)
        {
            var movementTime = (DateTime.Now - source.MovementStarted).TotalSeconds;
            if (movementTime > 0.1f * _chargeTime) 
                StartFx(source.transform);

            var targetSpeed = movementTime / _chargeTime * _maxSpeed;
            source.Speed = Math.Min(_maxSpeed, Math.Max((int) targetSpeed, _minSpeed));
        }
        
#if UNITY_EDITOR
        [UnityEditor.MenuItem("Assets/Create/Configs/Abilities/Cavalry Movement")]
        private static void Create()
        {
            CreateAsset<CavalryMovementAbilityConfig>();
        }
#endif
    }
}