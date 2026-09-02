using System.Collections.Generic;
using CoreConfigs.Configs;
using Enums;
using Game.Controllers;
using Game.Units;
using SoundPool;
using UnityEditor;
using UnityEngine;

namespace Configs
{
    public class AbilityConfig : ConfigBase
    {
        [Header("Bot AI Settings")] 
        [SerializeField] protected BotAiType _botAiType;
        [SerializeField] protected int _BotAiUnitLowerBoundCounter;
        [SerializeField] protected int _BotAiUnitUpperBoundCounter;
        [SerializeField] protected int _botAiUnitLowerBoundOfMinimumCost;
        [SerializeField] protected int _botAiUnitUpperBoundOfMinimumCost;
        [Space]
        [SerializeField] protected AbilityType _type;
        [SerializeField] protected string _name;
        [SerializeField] protected Sprite _icon;
        [SerializeField] protected AbilityTargetType _targetType;
        [SerializeField] protected FxPlayer _fx;
        [SerializeField] protected Vector3 _fxPosition = Vector3.zero;
        [SerializeField] protected Vector3 _fxScale = Vector3.one;
        [SerializeField] protected bool _customAnimation = false;
        [SerializeField] protected int _intervalDuration;
        [SerializeField] protected int _intervalCount;
        [SerializeField] private SoundConfig _towerAbilitySound;

        public int IntervalDuration => _intervalDuration;
        public int IntervalCount => _intervalCount;

        public AbilityType Type => _type;
        public AbilityTargetType TargetType => _targetType;
        public string Name => _name;
        public Sprite Icon => _icon;
        public bool CustomAnimation => _customAnimation;
        public BotAiType BotAiType => _botAiType;
        public int BotAiUnitLowerBoundCounter => _BotAiUnitLowerBoundCounter;
        public int BotAiUnitUpperBoundCounter => _BotAiUnitUpperBoundCounter;
        public int BotAiUnitLowerBoundOfMinimumCost => _botAiUnitLowerBoundOfMinimumCost;
        public int BotAiUnitUpperBoundOfMinimumCost => _botAiUnitUpperBoundOfMinimumCost;
        public SoundConfig TowerAbilitySound => _towerAbilitySound;

        public virtual void StartFx(Transform target, bool rotateFx = false)
        {
            StartFx(new List<Transform> {target}, rotateFx);
        }

        public virtual void StartFx(IEnumerable<Transform> targets, bool rotateFx = false)
        {
            if (_fx is null) return;

            foreach (var target in targets)
            {
                var position = _fxPosition;
                var rotation = _fx.transform.rotation.eulerAngles;
                if (rotateFx)
                {
                    rotation.y += 180;
                    position.x *= -1;
                }

                var fx = _fx.Create(target, position, _fxScale, rotation);
                Debug.Log($"Cast FX {_fx.gameObject.name} to transform {target.gameObject.name}. " +
                          $"rotateFx = {rotateFx}, position = {position.ToString("F3")}, rotation = {rotation.ToString("F3")}");
            }
        }

        public virtual void Cast(UnitController source)
        {
            // Логика способности используется и применяется при наступлении условий
            // А не включается/выключается как в случае эффектов
        }

        public virtual void CastDeath(UnitController source)
        {
            // Откат действия способности, действия при смерти и прочие OnDisable действия
        }

        public virtual void CastAfterDeath(UnitController source)
        {
            // Действия после смерти юнита, например респаун
        }

        public virtual void CastNoTarget(PlayerController controller, LineType lineType)
        {
            // Для общих способностей, не требующих наличия цели
        }

#if UNITY_EDITOR
        [MenuItem("Assets/Create/Configs/Ability Config")]
        private static void Create()
        {
            CreateAsset<AbilityConfig>();
        }
#endif
    }
}