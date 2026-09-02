using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Configs;
using Configs.Effects;
using DG.Tweening;
using Enums;
using ExitGames.Client.Photon;
using Game.Controllers;
using Game.Effects;
using Game.Environment;
using Game.Pool;
using Network;
using Photon.Pun;
using Photon.Realtime;
using PhotonUtils;
using Pool;
using SoundPool;
using UI;
using UI.Windows;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Units
{
    [RequireComponent(typeof(PhotonView), typeof(PhotonTransformView))]
    public class UnitController : MonoBehaviour, IPoolObject, IPunObservable
    {
        [Header("Animation Settings")] private static readonly int StateAttack = Animator.StringToHash("Attack");
        private static readonly int StatePrecast = Animator.StringToHash("Precast");
        private static readonly int StateReload = Animator.StringToHash("Reload");
        private static readonly int StateRandomState = Animator.StringToHash("RandomState");
        private static readonly int StateIsDeath = Animator.StringToHash("IsDeath");
        private static readonly int StateCanMove = Animator.StringToHash("CanMove");
        private static readonly int MovementSpeedMultiplier = Animator.StringToHash("MovementSpeedMultiplier");
        private static readonly int StateAttackAbility = Animator.StringToHash("AttackAbility");
        private static readonly int AttackAnimationMultiplier = Animator.StringToHash("AttackSpeedMultiplier");
        private static readonly int StateCavalryAttack = Animator.StringToHash("CavalryAttack");
        private static readonly int StateAlternativeAttack = Animator.StringToHash("AlternativeAttack");
        private static readonly int StateAlternativePrecast = Animator.StringToHash("AlternativePrecast");

        [SerializeField] private Animator _animator;

        [Space] [SerializeField] private UnitConfig _unitConfig;
        [SerializeField] private PhotonView _photonView;
        [SerializeField] private Transform _projectileDisplacement;
        private ProjectileActivator _projectileActivator;

        private List<ActiveEffect> _effects = new List<ActiveEffect>();
        private PlayerType _playerType;
        private int _health;
        private int _speed;
        private int _maxHealth;
        private float _progress;
        private Coroutine _moveCoroutine;
        private Coroutine _immortalCoroutine;
        private bool _isAttack;
        private bool _isMoving;
        private bool _isDead;
        private float _attackSpeedMultiplier;
        private float _immortalTimer;

        public bool CanAttack { get; set; }

        public bool IsStunned
        {
            get => _isStunned;
            set
            {
                _isStunned = value;
                if (_isStunned)
                {
                    StopMoveUnit();
                    LastAction = ActionType.None;
                }
            }
        }

        public ActionType LastAction { get; private set; }
        public PhotonView View => _photonView;
        public UnitConfig UnitConfig => _unitConfig;
        public PlayerType PlayerType => _playerType;
        public UnitController Target { get; set; }
        public UnitController LastTarget { get; set; }
        public int PredictedPositionX { get; set; }
        public int CurrentCellX { get; set; }
        public int DeathPositionX { get; private set; }
        public LineType CurrentLine { get; private set; }

        public float AttackSpeedMultiplier
        {
            get => _attackSpeedMultiplier;
            set
            {
                _attackSpeedMultiplier = value;
                SetDefaultAttackSpeed(1 / _attackSpeedMultiplier);
            }
        }

        public float PrecastTime { get; private set; }
        public float AttackTime { get; private set; }
        public float ReloadTime { get; private set; }
        public float CavalryAttackTime { get; private set; }

        public bool IsAttack
        {
            get => _isAttack;
            set
            {
                if (value) LastAction = ActionType.Attack;
                _isAttack = value;
            }
        }

        public bool IsMoving
        {
            get => _isMoving;
            private set
            {
                if (value) LastAction = ActionType.Movement;
                _isMoving = value;
            }
        }

        public int MaxHealth
        {
            get => _maxHealth;
            set
            {
                _maxHealth = value;
                MaxHealthChanged?.Invoke(value);
            }
        }

        public bool HaveTemporaryShield { get; set; }
        public int MaxSpeed => _unitConfig.MoveSpeed;
        public int MaxDamage => _unitConfig.Damage;
        public DateTime MovementStarted { get; set; }
        public int Damage { get; set; }

        public event Action<int> HealthChanged;
        public event Action<int> MaxHealthChanged;
        public event Action<int> ShieldChanged;
        public event Action DeathEvent;
        private UnitInfo _uiInfo;
        private bool _isStunned;

        public int Health
        {
            get => _health;
            set
            {
                if (_health <= 0) return;
                _health = value >= MaxHealth ? MaxHealth : value;
                if (_health < MaxHealth)
                {
                    CreateHealthBar();
                    if (_uiInfo != null)
                        if (!_uiInfo.isActiveAndEnabled)
                            _uiInfo.Show();
                }

                HealthChanged?.Invoke(_health);
                if (_health <= 0) Death();
            }
        }

        public int Speed
        {
            get => _speed;
            set
            {
                if (value <= 0)
                {
                    StopMoveUnit();
                    IsMoving = true;
                    LastAction = ActionType.None;
                }
                else
                {
                    if (_speed < 0)
                        IsMoving = false;
                    else
                    {
                        var multiplier = (float) _speed / MaxSpeed;
                        _animator.SetFloat(MovementSpeedMultiplier, multiplier);
                    }
                }

                _speed = value;
            }
        }

        public int AttackNumber { get; set; }


        #region Pool

        public IPoolObject Origin { get; set; }

        public IPoolObject LoadObject(IPoolObject origin)
        {
            var player = Instantiate((UnitController) origin);
            player.Origin = origin;

            PhotonView photonView = player.GetComponent<PhotonView>();

            if (PhotonNetwork.AllocateViewID(photonView))
            {
                var unitTransform = player.transform;
                object[] data =
                {
                    unitTransform.position, unitTransform.rotation, photonView.ViewID, player._unitConfig.Uid
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

                PhotonNetwork.RaiseEvent((byte) NetworkEvents.InstantiateObject, data, raiseEventOptions, sendOptions);
            }

            return player;
        }

        public int PreloadCount => UnitConfig.PreloadCount;

        public void OnPop()
        {
            //initialize
            _animator.SetBool(StateIsDeath, false);
            SetDefault();
        }

        public void OnPush()
        {
            //de-initialize
            _animator.SetInteger(StateRandomState, 0);
            _animator.SetBool(StateCanMove, false);
            if (_projectileActivator != null)
                _projectileActivator.ProjectileSpawnMoment -= OnProjectileSpawn;
        }

        #endregion

        public void Init(UnitConfig config, PlayerType playerType, LineType lineType, int spawnPosition = -1)
        {
            _unitConfig = config;
            _playerType = playerType;
            CurrentLine = lineType;

            SetDefault();

            if (config.AttackType == AttackType.Range)
            {
                if (_projectileActivator == null)
                    _projectileActivator = GetComponentInChildren<ProjectileActivator>();
                
                _projectileActivator.ProjectileSpawnMoment += OnProjectileSpawn;
            }

            _isDead = false;

            var placeRandomizer = 1;
            var defaultCellX = _playerType == PlayerType.Player ? 0 : 20;
            CurrentCellX = spawnPosition == -1 ? defaultCellX : spawnPosition;

            switch (_playerType)
            {
                case PlayerType.Player:
                    if (config.UnitSize == UnitSize.Big)
                        GameController.Instance.PlayerStartPlaceRandomizer[(int) CurrentLine] = 1;
                    else
                        placeRandomizer = GameController.Instance.PlayerStartPlaceRandomizer[(int) CurrentLine];
                    break;
                
                case PlayerType.Enemy:
                    if (config.UnitSize == UnitSize.Big)
                        GameController.Instance.EnemyStartPlaceRandomizer[(int) CurrentLine] = 1;
                    else
                        placeRandomizer = GameController.Instance.EnemyStartPlaceRandomizer[(int) CurrentLine];
                    break;
            }

            GameController.Instance.IncreasePlacePosition(_playerType, (int) CurrentLine);

            PredictedPositionX = CurrentCellX;
            DeathPositionX = CurrentCellX;

            UnitCollection.Instance.GlobalUnitCollection.Add(this);
            GameController.Instance.GameMachine.AddNewUnit(this);

            transform.rotation = Quaternion.Euler(0, _playerType == PlayerType.Player ? 90 : 270, 0);

            var placeOffset = 0f;

            if (config.UnitSize != UnitSize.Big)
            {
                switch (placeRandomizer)
                {
                    case 0:
                        placeOffset = -0.55f;
                        break;
                    case 1:
                        placeOffset = 0;
                        break;
                    case 2:
                        placeOffset = 0.55f;
                        break;
                }
            }

            var unitTransform = transform;
            var pos = unitTransform.localPosition;
            unitTransform.localPosition = new Vector3(pos.x, pos.y, pos.z + placeOffset);
            if (_playerType == PlayerType.Enemy)
            {
                var animatorTransform = _animator.transform;
                var scale = animatorTransform.localScale;
                animatorTransform.localScale = new Vector3(-1 * scale.x, scale.y, scale.z);
            }
            
            if (PhotonNetwork.IsMasterClient)
                CastSpawnAbilities();
        }

        public void StartImmortalPhase()
        {
            _immortalTimer = _unitConfig.ImmortalTime;
            _immortalCoroutine = StartCoroutine(ImmortalTimeCounter());
        }

        private void CreateHealthBar()
        {
            if (_uiInfo != null || _isDead) return;
            var healthBar = _playerType == PlayerType.Player
                ? UIConfig.Instance.UnitInfo
                : UIConfig.Instance.EnemyUnitInfo;
            _uiInfo = Instantiate(healthBar, UiManager.Instance.MainCanvas.transform);
            _uiInfo.Init(this);
        }

        private void SetDefault()
        {
            LastAction = ActionType.None;
            Target = null;
            IsAttack = false;
            IsMoving = false;
            CanAttack = false;
            IsStunned = false;
            MaxHealth = _unitConfig.HitPoints;
            _health = MaxHealth;
            Speed = _unitConfig.MoveSpeed;
            Damage = _unitConfig.Damage;
            AttackSpeedMultiplier = 1;
            SetDefaultAttackSpeed(AttackSpeedMultiplier);
            _effects = new List<ActiveEffect>();
        }

        private void SetDefaultAttackSpeed(float multiplier)
        {
            CavalryAttackTime = _unitConfig.CavalryAttackTime * multiplier;
            PrecastTime = _unitConfig.PrecastTime * multiplier;
            AttackTime = _unitConfig.AttackTime * multiplier;
            ReloadTime = _unitConfig.ReloadTime * multiplier;
            _animator.SetFloat(AttackAnimationMultiplier, 1 / multiplier);
        }

        public void EffectsApplyAll()
        {
            var clone = _effects.ToArray();
            foreach (var effect in clone) // Возможно потом надо будет переработать для оптимизации
            {
                effect.Cast(this);
                UpdateEffect(effect, 0, GameController.Instance.Tick);
            }
        }

        public void AddEffect(EffectConfig effectConfig, PlayerType filter = PlayerType.None)
        {
            RemoveEffect(effectConfig, filter);
            var effect = new ActiveEffect(effectConfig);
            if (effectConfig is ShieldEffectConfig)
            {
                CreateHealthBar();
                _uiInfo.ShieldInit((int) effectConfig.Value);
                if (Health >= MaxHealth || Health <= 0 || MaxHealth <= 0)
                    _uiInfo.Hide();
            }

            _effects.Add(effect);
            effect.Enable(this, filter);
        }

        public void UpdateEffect(ActiveEffect effect, float value, float durationTick = 0)
        {
            if (effect == default) return;

            if (!effect.Config.EndlessDuration)
                effect.Duration -= durationTick;
            effect.Value += value;

            if (effect.IsEnded())
                RemoveEffect(effect);
        }

        public void UpdateEffect(EffectConfig config, float value, float durationTick)
        {
            var effect = _effects.FirstOrDefault(x => x.Config == config);
            UpdateEffect(effect, value, durationTick);
        }

        private void RemoveEffect(ActiveEffect effect, PlayerType filter = PlayerType.None)
        {
            if (effect == default) return;

            effect.Disable(this, filter);
            _effects.Remove(effect);
        }

        public void RemoveEffect(EffectConfig effectConfig, PlayerType filter = PlayerType.None)
        {
            RemoveEffect(_effects.FirstOrDefault(x => x.Config == effectConfig), filter);
        }

        public IEnumerable<ActiveEffect> GetEffects(Type type)
        {
            return _effects.Where(x => x.Config.GetType() == type);
        }

        public IEnumerable<ActiveEffect> GetEffects(EffectConfig config)
        {
            return _effects.Where(x => x.Config == config);
        }

        public void MovementState(bool condition)
        {
            _animator.SetBool(StateCanMove, Speed > 0 && condition);
        }

        private const string SPAWN_PROJECTILE = nameof(SpawnProjectile);
        
        public Vector3 GetTargetPoint()
        {
            var pos = transform.position;
            var offset = _unitConfig.TargetPointOffset;
            
            if (_playerType == PlayerType.Enemy) offset = new Vector2(-offset.x, offset.y);
            
            return new Vector3(pos.x + offset.x, pos.y + offset.y, pos.z);
        }

        private void PlaySound(SoundConfig sound, float interval, float duration)
        {
            if (sound == null)
            {
                //Debug.LogWarning($"SoundConfig is null in UnitConfig: {UnitConfig.Name}");
                return;
            }
            SoundPlayer soundPlayer  = null;

            var seq = DOTween.Sequence();
            seq.AppendInterval(interval);
            seq.AppendCallback(() => {soundPlayer = SoundManager.Instance.Play(sound); });
            seq.AppendInterval(duration);
            seq.AppendCallback(() => { SoundManager.Instance.Stop(soundPlayer); });
        }

        public void PlayCavalryAttackAnimation()
        {
            if (_isDead || AttackSpeedMultiplier <= 0 || IsStunned) return;
            _animator.SetInteger(StateRandomState, Random.Range(1, 100));
            _animator.SetTrigger(StateCavalryAttack);
        }

        public void PlayAttackPrecastAnimation(bool attackTower = false)
        {
            if (_isDead || AttackSpeedMultiplier <= 0 || IsStunned) return;
            
            if (!attackTower && Target != null && Target.CurrentLine != CurrentLine)
            {
                _animator.SetInteger(StateRandomState, Random.Range(1, 100));
                _animator.SetTrigger(StateAlternativePrecast);
                Debug.Log($"{name} attacks {Target.name} with Alternative Reload");
            }
            else
            {
                _animator.SetInteger(StateRandomState, Random.Range(1, 100));
                _animator.SetTrigger(StatePrecast);
                Debug.Log($"{name} attacks {(attackTower ? "Tower" : Target.name)} with usial Reload");
            }

            PlaySound(UnitConfig.PrecastSound, UnitConfig.PrecastDelay,
                UnitConfig.PrecastDuration);
        }

        public void PlayAttackAnimation(bool attackTower = false)
        {
            if (_isDead || AttackSpeedMultiplier <= 0 || IsStunned) return;
            
            if (!attackTower && Target != null && Target.CurrentLine != CurrentLine)
            {
                _animator.SetInteger(StateRandomState, Random.Range(1, 100));
                _animator.SetTrigger(StateAlternativeAttack);
                Debug.Log($"{name} attacks {Target.name} with Alternative Attack");
            }
            else
            {
                _animator.SetInteger(StateRandomState, Random.Range(1, 100));
                _animator.SetTrigger(StateAttack);
                Debug.Log($"{name} attacks {(attackTower ? "Tower" : Target.name)} with usial Attack");
            }

            PlaySound(UnitConfig.AttackSound, UnitConfig.AttackDelay,
                UnitConfig.AttackDuration);
        }

        public void PlayReloadAnimation()
        {
            if (_isDead || AttackSpeedMultiplier <= 0 || IsStunned) return;
            _animator.SetInteger(StateRandomState, Random.Range(1, 100));
            _animator.SetTrigger(StateReload);
            PlaySound(UnitConfig.ReloadSound, UnitConfig.ReloadDelay,
                UnitConfig.ReloadDuration);
        }

        public void PlayAttackAbilityAnimation()
        {
            if (_isDead || AttackSpeedMultiplier <= 0 || IsStunned) return;
            _animator.SetTrigger(StateAttackAbility);
        }

        public void PlayTowerDamagedAnimation()
        {
            if (_isDead || AttackSpeedMultiplier <= 0 || IsStunned) return;
            var controller = _playerType == PlayerType.Player
                ? GameController.Instance.Enemy
                : GameController.Instance.Player;
            var tower = controller.TowerVisual[(int) CurrentLine];
            var animator = tower.gameObject.GetComponentInChildren<Animator>();
            if (animator == null) return;
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("ScaleTower"))
                return;
            animator.Play("ScaleTower");

            var towerVisualChanger = tower.GetComponentInChildren<TowerVisualChanger>();
            if (towerVisualChanger == null) return;

            towerVisualChanger.PlayTowerFX();
        }

        public void TakeDamage(int damage)
        {
            if (_immortalTimer > 0) return;

            damage = Math.Abs(damage);
            foreach (var effect in GetEffects(typeof(ShieldEffectConfig)).ToArray())
            {
                if (effect.Value <= 0) continue;

                var damageDelta = (int) Math.Min(effect.Value, damage);
                var shieldValue = effect.Value - damageDelta;
                UpdateEffect(effect, -damageDelta);
                SetShieldDamage((int) shieldValue);
                damage -= damageDelta;
                if (damage <= 0) break;
            }

            if (damage > 0 && Health > 0)
                Health -= damage;
        }

        public void TakeHeal(int healValue)
        {
            Health += healValue;
        }

        private void Death()
        {
            _isDead = true;
            DeathPositionX = CurrentCellX;
            if (PhotonNetwork.IsMasterClient)
                CastDeathAbilities();

            UnitCollection.Instance.GlobalUnitCollection.Remove(this);
            GameController.Instance.GameMachine.RemoveDeadUnit(this);

            _animator.SetInteger(StateRandomState, Random.Range(1, 100));
            _animator.SetBool(StateIsDeath, true);

            // ToDo: FX activation
            DeathEvent?.Invoke();
            Invoke(nameof(DeathAnimationDelay), 0.5f);
        }

        private void DeathAnimationDelay()
        {
            var delay = _animator.GetCurrentAnimatorStateInfo(0).length - 0.5f;
            Invoke(nameof(Push), delay > 0 ? delay : 0);
        }

        private void Push()
        {
            if (PhotonNetwork.IsMasterClient)
                CastAfterDeathAbilities();

            Destroy(_uiInfo);

            object[] data =
            {
                _photonView.ViewID
            };

            PhotonSingleton.Instance.RaiseEvent((byte) NetworkEvents.Despawn, data);
        }

        [PunRPC]
        private void SpawnProjectile()
        {
            if (_unitConfig.Projectile == null) return;
            var projectile = ProjectilePool.Instance.Pop(_unitConfig.Projectile);

            if (Target != null)
            {
                var ballisticShot = Target.CurrentLine == CurrentLine;
                
                projectile.GetComponent<Projectile>().
                    Init(Target.gameObject, gameObject, _playerType, ballisticShot, 
                        _projectileDisplacement != null ? _projectileDisplacement.position : Vector3.zero);
            }
            else
            {
                var controller = _playerType == PlayerType.Player
                    ? GameController.Instance.Enemy
                    : GameController.Instance.Player;
                var tower = controller.GetTower(CurrentLine);
                var towerGameObject = tower.gameObject;
                projectile.GetComponent<Projectile>().
                    Init(towerGameObject, gameObject, _playerType, true, 
                        _projectileDisplacement != null ? _projectileDisplacement.position : Vector3.zero);
            }
        }

        private void OnProjectileSpawn()
        {
            Debug.Log($"{_unitConfig.name} spawns projectile");
            _photonView.RPC(SPAWN_PROJECTILE, RpcTarget.All);
        }

        private void OnValidate()
        {
            if (_photonView == null)
                _photonView = GetComponent<PhotonView>();
            
            if (!_animator)
                Debug.LogError("<color=yellow>" + this + "</color> Doesn't contain <color=red>Animator</color>");

            if (!_unitConfig)
            {
                Debug.LogError("<color=yellow>" + this + "</color> Doesn't contain <color=red>Unit Config</color>");
                return;
            }

            if (_unitConfig.PreloadCount == 0)
                Debug.LogError("<color=yellow>" + this + "</color> Preload count can't be equal zero");

            if (_unitConfig.PreloadCount < _unitConfig.MaxCount)
                Debug.LogError("<color=yellow>" + this + "</color> Preload count can't be less than Max Count");
        }

        private void CastSpawnAbilities()
        {
            foreach (var skill in UnitConfig.Abilities)
            {
                if (skill.Type != AbilityType.Spawn)
                    continue;

                skill.Cast(this);
            }
        }

        private void CastDeathAbilities()
        {
            foreach (var skill in UnitConfig.Abilities)
                skill.CastDeath(this);

            foreach (var effect in _effects.ToArray())
            {
                effect.CastAfterDeath(this);
                UpdateEffect(effect, 0, Time.deltaTime);
            }
        }

        private void CastAfterDeathAbilities()
        {
            foreach (var skill in UnitConfig.Abilities)
                skill.CastAfterDeath(this);
        }

        public void SetShieldDamage(int damageDelta)
        {
            if (!_uiInfo.isActiveAndEnabled)
                _uiInfo.Show();

            ShieldChanged?.Invoke(damageDelta);
        }

        public void StartMoveUnit()
        {
            _moveCoroutine = StartCoroutine(MoveUnit(false));
        }

        public void StartForcedMoveUnit()
        {
            if (_moveCoroutine != null) StopCoroutine(_moveCoroutine);
            _moveCoroutine = StartCoroutine(MoveUnit(true));
        }

        public void StopMoveUnit()
        {
            if (_moveCoroutine != null) StopCoroutine(_moveCoroutine);
            MovementState(false);
            IsMoving = false;
        }

        private IEnumerator MoveUnit(bool isForced)
        {
            MovementState(true);
            IsMoving = true;

            var startPos = transform.position;
            var emptyCellElement = GameController.Instance.GameMachine.Cells[PredictedPositionX, (int) CurrentLine]
                .EmptyGridCellElement();
            var cellPos = emptyCellElement != null
                ? emptyCellElement.transform.position
                : GameController.Instance.GameMachine.Cells[PredictedPositionX, (int) CurrentLine].transform.position;
            
            var endPos = new Vector3(cellPos.x, startPos.y, startPos.z);
            CurrentCellX = PredictedPositionX;
            float progress = 0;
            while (progress < 1)
            {
                progress += Time.deltaTime * (isForced ? 20 : Speed) /
                            GameController.Instance.GameMachine.SpeedDowngrade;
                transform.position = Vector3.Lerp(startPos, endPos, progress);
                yield return null;
            }

            IsMoving = false;
        }

        private IEnumerator ImmortalTimeCounter()
        {
            while (_immortalTimer > 0)
            {
                _immortalTimer -= Time.deltaTime;
                yield return null;
            }
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting && PhotonNetwork.IsMasterClient)
            {
                stream.SendNext(Health);
            }
            else if (stream.IsReading)
            {
                Health = (int) stream.ReceiveNext();
            }
        }
    }
}