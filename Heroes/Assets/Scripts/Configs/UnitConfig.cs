using System;
using System.Linq;
using CoreConfigs.Configs;
using Enums;
using Game.Units;
using UnityEngine;
using SoundPool;

namespace Configs
{
    [Serializable]
    public class UnitConfig : ConfigBase
    {
        [SerializeField] private UnitController _unitPrefab;
        [SerializeField] private Sprite _icon;
        [SerializeField] private string _name;
        [SerializeField] private string _description;
        [SerializeField] private string _abilityDescription;
        [SerializeField] private string _loreText;
        [SerializeField] private UnitConfig _unitSummon;
        [SerializeField] private bool _isMercenary = false;

        [Header("Unit line settings")]
        [SerializeField] private int _cost;
        [SerializeField] private int _maxCount;
        [SerializeField] private int _preloadCount = 10;
        [SerializeField] private bool _isInfinite;
        [SerializeField] private UnitSize _unitSize;
        [SerializeField] private SetupProperty[] _setupProperties;

        [Header("Char settings")] 
        [SerializeField] private UnitTag[] _unitTags;
        [SerializeField] private int _moveSpeed;
        [Tooltip("For melee = 1")] 
        [Range(1, 20)] [SerializeField] private int _attackRange = 1;
        [SerializeField] private AttackType _attackType;
        [SerializeField] private Projectile _projectile;
        [SerializeField] private FxPlayer _attackFx;
        [SerializeField] private Vector3 _attackFxPosition = Vector3.zero;

        [SerializeField] private int _damage;
        [SerializeField] private int _hitPoints;
        [SerializeField] private float _immortalTime;
        [SerializeField] private AbilityConfig[] _abilities;

        [Header("Precast settings")] 
        [Range(0, 10)] [SerializeField] private float _precastTime = 0.33f;
        [SerializeField] private SoundConfig _precastSound;
        [Range(0, 1)][SerializeField] private float _precastDelay;
        [Range(0, 1)][SerializeField] private float _precastDuration;
        [Header("Attack settings")] 
        [Range(0, 10)] [SerializeField] private float _cavalryAttackTime = 0.33f;
        [Range(0, 10)] [SerializeField] private float _attackTime = 0.33f;
        [SerializeField] private SoundConfig _attackSound;
        [Range(0, 1)][SerializeField] private float _attackDelay;
        [Range(0, 1)][SerializeField] private float _attackDuration;
        [Header("Reload settings")] 
        [Range(0, 10)] [SerializeField] private float _reloadTime = 0.33f;
        [SerializeField] private SoundConfig _reloadSound;
        [Range(0, 1)][SerializeField] private float _reloadDelay;
        [Range(0, 1)][SerializeField] private float _reloadDuration;
        [Header("HealthBar offset")]
        [SerializeField] private int _positionX;
        [SerializeField] private int _positionY;
        [Header("Target Point offset"), Tooltip("Смещение относительно позиции юнита для точки, в которую будет лететь снаряд")]
        [SerializeField] private Vector2 _targetPointOffset = new Vector2(0f, 0f);
        
        
        public UnitController UnitPrefab => _unitPrefab;
        public Sprite Icon => _icon;
        public string Name => _name;
        public string AbilityDescription => _abilityDescription;
        public string LoreText => _loreText;
        public bool IsMercenary => _isMercenary;
        public int Cost => _cost;
        public int MaxCount
        {
            get => _maxCount;
            set
            {
                if (!_isMercenary)
                    return;
                _maxCount = value;
            }
        }
        public int PreloadCount => _preloadCount;
        public bool IsInfinite => _isInfinite;
        public UnitSize UnitSize => _unitSize;
        public SetupProperty[] SetupProperties => _setupProperties;

        public UnitTag[] UnitTags => _unitTags;
        public int MoveSpeed => _moveSpeed;
        public int AttackRange => _attackRange;
        public AttackType AttackType => _attackType;
        public int Damage => _damage;
        public float CavalryAttackTime => _cavalryAttackTime;
        public float PrecastTime => _precastTime;
        public float AttackTime => _attackTime;
        public float ReloadTime => _reloadTime;
        public Projectile Projectile => _projectile;
        public FxPlayer AttackFx => _attackFx;
        public SoundConfig PrecastSound => _precastSound;
        public float PrecastDelay => _precastDelay;
        public float PrecastDuration => _precastDuration;
        public SoundConfig AttackSound => _attackSound;
        public float AttackDelay => _attackDelay;
        public float AttackDuration => _attackDuration;
        public SoundConfig ReloadSound => _reloadSound;
        public float ReloadDelay => _reloadDelay;
        public float ReloadDuration => _reloadDuration;
        public Vector3 AttackFxPosition => _attackFxPosition;
        public int HitPoints => _hitPoints;
        public float ImmortalTime => _immortalTime;
        public AbilityConfig[] Abilities => _abilities.Where(x => x != null).ToArray();
        public float PositionX => _positionX;
        public float PositionY => _positionY;
        public Vector2 TargetPointOffset => _targetPointOffset;
        public UnitConfig UnitSummon => _unitSummon;


#if UNITY_EDITOR
        [UnityEditor.MenuItem("Assets/Create/Configs/UnitConfig")]
        private static void Create()
        {
            CreateAsset<UnitConfig>();
        }
#endif
    }

    [Serializable]
    public struct SetupProperty
    {
        [SerializeField] private  LineType _setupType;
        [SerializeField] private  LineType[] _attackLineType;
        
        public LineType SetupType => _setupType;
        public LineType[] AttackLineType => _attackLineType;
    }
}