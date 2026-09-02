using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Configs;
using Configs.Abilities;
using Configs.Effects;
using DG.Tweening;
using Enums;
using Game.Controllers;
using Game.Summons;
using Game.Units;
using Photon.Pun;
using SoundPool;
using Structs;
using UnityEngine;
using Utils;
using Utils.SaveManager;

namespace Game.Battle
{
    public class GameMachine : MonoBehaviour
    {
        [SerializeField] private int _verticalCellCount = 3;
        [SerializeField] private int _horizontalCellCount = 21;
        [SerializeField] private GridCell _cellPrefab;
        [SerializeField] private float _speedDowngrade = 0.5f;
        [SerializeField] private int _damageBoostMultiplier = 3;
        [SerializeField] private List<LineTransform> _lineTransforms;
        [SerializeField] private float[] _manaTicks;
        [SerializeField] private GameRewardController _gameRewardController;
        [SerializeField] private Tutorial_Battle_Scenario _battleTutorial;

        private int _tutorialTickCounter = 0;
        private bool _playerDamageBoost = false;
        
        private bool _cavalryAttack;
        private GridCell[,] _cells;

        public GameRewardController GameRewardController => _gameRewardController;
        public int HorizontalCellCount => _horizontalCellCount;
        public GridCell[,] Cells => _cells;
        public float SpeedDowngrade => _speedDowngrade;
        public float[] ManaTicks => _manaTicks;
        public bool IsBattleActive { get; set; }
        public bool IsPaused { get; set; }

        // if True: Состояние истощения запасов юнитов игрока / противника в колоде
        public bool PossiblePlayerArmyLost { get; set; }
        public bool PossibleEnemyArmyLost { get; set; }
        public event Action<bool> OnArmyLost;

        private void Awake()
        {
            GameController.Instance.GameMachine = this;
            CreateGrid();
        }

        private void OnEnable()
        {
            PossibleEnemyArmyLost = false;
            PossiblePlayerArmyLost = false;

            GameController.Instance.ControllerTickEvent += OnGameTick;
        }

        private void OnDisable()
        {
            GameController.Instance.ControllerTickEvent -= OnGameTick;
        }

        private void CreateGrid()
        {
            _cells = new GridCell[_horizontalCellCount, _verticalCellCount];

            for (var y = 0; y < _verticalCellCount; y++)
            {
                for (var x = 0; x < _horizontalCellCount; x++)
                {
                    CreateCell(x, y);
                }
            }
        }

        private void CreateCell(int x, int y)
        {
            var cell = _cells[x, y] = Instantiate(_cellPrefab, transform);
            cell.transform.localPosition = new Vector3(x / 2f, -(y * 2f), 0f);
            cell.name = gameObject.name + " (" + x + " ; " + y + ")";
        }

        public void GameStop()
        {
            if (!IsBattleActive) return;

            IsBattleActive = false;
            foreach (var unit in UnitCollection.Instance.GlobalUnitCollection)
            {
                unit.CanAttack = false;
                unit.MovementState(false);
            }
        }

        public void GamePause()
        {
            IsPaused = true;
            GameStop();
        }
        
        public void GameContinue()
        {
            if (IsBattleActive) return;

            IsBattleActive = true;
            foreach (var unit in UnitCollection.Instance.GlobalUnitCollection)
            {
                unit.CanAttack = true;
                unit.MovementState(true);
            }

            GameController.Instance.ContinueTick();
            IsPaused = false;
        }
        
        public void OnUpdate()
        {
            if (!PhotonNetwork.IsMasterClient)
                return;

            if (IsBattleActive)
            {
                CastAllPassiveTowerAbilities();
                SearchEnemy();
                Attack();
                PredictMove();
                CheckObstacles();
                Move();
            }
        }

        private void OnGameTick()
        {
            if (!PhotonNetwork.IsMasterClient)
                return;

            ApplyEffects();
            CastAllAvailableBuffs();

            if (_tutorialTickCounter <= 1)
            {
                if (_tutorialTickCounter == 1 && !SaveManagerSafe.GetValue(SavedDataManager.FirstBattleEndedKey, false))
                {
                    if (_battleTutorial != null)
                        _battleTutorial.enabled = true;
                }
                _tutorialTickCounter++;
            }
        }

        private void ApplyEffects()
        {
            foreach (var unit in UnitCollection.Instance.GlobalUnitCollection)
                unit.EffectsApplyAll();
        }

        private void SearchEnemy()
        {
            foreach (var unit in UnitCollection.Instance.GlobalUnitCollection)
            {
                if (unit.IsStunned) continue;

                unit.Target = null;

                SearchByLine(unit, (int) unit.CurrentLine);

                if (!unit.Target)
                {
                    for (var i = 0; i < 2; i++)
                    {
                        foreach (var setupProperty in unit.UnitConfig.SetupProperties)
                        {
                            if (setupProperty.SetupType == unit.CurrentLine)
                            {
                                foreach (var attackLineType in setupProperty.AttackLineType)
                                {
                                    if (attackLineType != unit.CurrentLine && (int) attackLineType == i)
                                    {
                                        SearchByLine(unit, i);
                                    }
                                }
                            }
                        }
                    }
                }

                if (!unit.Target)
                    SearchTower(unit);

                unit.CanAttack = unit.Target;
            }
        }

        private const int OFFSET = 1;

        private void SearchByLine(UnitController source, int posY)
        {
            for (var i = 0; i < source.UnitConfig.AttackRange + OFFSET; i++)
            {
                if (source.PlayerType == PlayerType.Player)
                {
                    if (source.CurrentCellX + i >= _horizontalCellCount) continue;
                    SearchByCell(source, posY, i);
                    if (source.Target != null) break;
                }
                else if (source.PlayerType == PlayerType.Enemy)
                {
                    if (source.CurrentCellX - i < 0) continue;
                    SearchByCell(source, posY, -i);
                    if (source.Target != null) break;
                }
            }
        }

        private void SearchByCell(UnitController source, int posY, int cell)
        {
            var checkedCell = source.CurrentCellX + cell;
            if (!_cells[checkedCell, posY].ContainsUnit()) return;
            foreach (var unit in _cells[checkedCell, posY].CellUnitCollection)
            {
                if (unit.PlayerType == source.PlayerType || unit.Health <= 0) continue;
                source.Target = unit;
                break;
            }
        }

        public List<UnitController> Search(UnitController unit, int range, bool searchSamePlayer = false)
        {
            var targets = new List<UnitController>();

            for (var i = 0; i < range; i++)
            {
                if (unit.PlayerType == PlayerType.Enemy || unit.PlayerType == PlayerType.Player && searchSamePlayer)
                    if (unit.CurrentCellX - i < 0)
                        break;

                if (unit.PlayerType == PlayerType.Player || unit.PlayerType == PlayerType.Enemy && searchSamePlayer)
                    if (unit.CurrentCellX + i >= _horizontalCellCount)
                        break;

                var cell = _cells[unit.CurrentCellX + (unit.PlayerType == PlayerType.Player ? i : -i),
                    (int) unit.CurrentLine];
                if (!cell.ContainsUnit()) continue;
                foreach (var target in cell.CellUnitCollection)
                {
                    if (target == null || target.Health <= 0) continue;
                    var isSamePlayer = unit.PlayerType == target.PlayerType;
                    if (isSamePlayer != searchSamePlayer) continue;

                    targets.Add(target);
                }
            }

            return targets;
        }

        public List<UnitController> SearchUnitsFromTower(PlayerType unitType, int range, int sourceCellX,
            int currentLine)
        {
            var targets = new List<UnitController>();

            for (var i = 0; i < range; i++)
            {
                if (unitType == PlayerType.Enemy && sourceCellX + i >= _horizontalCellCount) break;
                if (unitType == PlayerType.Player && sourceCellX - i < 0) break;

                var cell = _cells[sourceCellX + (unitType == PlayerType.Player ? -i : i), currentLine];
                if (!cell.ContainsUnit()) continue;
                foreach (var target in cell.CellUnitCollection)
                {
                    if (target == null || target.Health <= 0) continue;
                    if (unitType != target.PlayerType) continue;

                    targets.Add(target);
                }
            }

            return targets;
        }

        private void SearchTower(UnitController source)
        {
            if (source.IsAttack || source.IsStunned) return;
            if ((source.PlayerType == PlayerType.Player &&
                 _horizontalCellCount - source.CurrentCellX <= source.UnitConfig.AttackRange) ||
                (source.PlayerType == PlayerType.Enemy && source.CurrentCellX <= source.UnitConfig.AttackRange))
            {
                source.StopMoveUnit();
                StartCoroutine(DoTowerDamage(source));
            }
        }

        private Transform GetLineZone(PlayerType player, LineType line)
        {
            return _lineTransforms.FirstOrDefault(x => x.player == player && x.line == line).transform;
        }

        private void PredictMove()
        {
            foreach (var unit in UnitCollection.Instance.GlobalUnitCollection)
            {
                if (unit.IsStunned || unit.CanAttack || unit.IsAttack || unit.Target || unit.Speed <= 0) continue;
                switch (unit.PlayerType)
                {
                    case PlayerType.Player when unit.PredictedPositionX < _horizontalCellCount - 1 &&
                                                unit.CurrentCellX + unit.UnitConfig.AttackRange < _horizontalCellCount:
                        PredictNextCell(unit, 1);
                        break;
                    case PlayerType.Enemy when unit.PredictedPositionX > 0 &&
                                               unit.CurrentCellX - unit.UnitConfig.AttackRange > 0:
                        PredictNextCell(unit, -1);
                        break;
                    default:
                        unit.MovementState(false);
                        break;
                }
            }
        }

        private void PredictNextCell(UnitController source, int direction)
        {
            source.PredictedPositionX = source.CurrentCellX + direction;
        }

        private void Move()
        {
            foreach (var unit in UnitCollection.Instance.GlobalUnitCollection)
            {
                if (unit.IsStunned || unit.IsMoving || unit.CanAttack || unit.IsAttack || unit.Target ||
                    unit.Health <= 0 || unit.Speed <= 0) continue;
                MoveUnit(unit);
            }
        }

        private void CastAllAvailableBuffs()
        {
            foreach (var unit in UnitCollection.Instance.GlobalUnitCollection)
            {
                if (unit.Health <= 0) continue;
                CastBuffAbilities(unit);
            }
        }

        private void CheckObstacles()
        {
            var tempCollection = new Collection<UnitController>();

            foreach (var unit in UnitCollection.Instance.GlobalUnitCollection)
            {
                var currentCell = _cells[unit.CurrentCellX, (int) unit.CurrentLine];
                if (currentCell.ContainsObstacle(unit)) tempCollection.Add(unit);
            }

            foreach (var unit in tempCollection)
            {
                var currentCell = _cells[unit.CurrentCellX, (int) unit.CurrentLine];
                var obstacle = currentCell.GetObstacle(unit);
                if (obstacle != null && unit != null) obstacle.Impact(unit);
            }
        }

        public void AddObstacle(Obstacle summon, int cellX, int cellY)
        {
            if (cellX < 0 || cellX > HorizontalCellCount) return;
            Cells[cellX, cellY].AddToCellObstacleCollection(summon);
        }

        public void RemoveObstacle(Obstacle summon, int cellX, int cellY)
        {
            if (cellX < 0 || cellX > HorizontalCellCount) return;
            Cells[cellX, cellY].RemoveFromCellObstacleCollection(summon);
        }

        private void MoveUnit(UnitController source)
        {
            if (source == null) return;
            if (!_cells[source.PredictedPositionX, (int) source.CurrentLine].CanAddNewUnit(source.UnitConfig))
            {
                source.StopMoveUnit();
                return;
            }


            if (source.LastAction != ActionType.Movement) source.MovementStarted = DateTime.Now;
            _cells[source.CurrentCellX, (int) source.CurrentLine].RemoveFromCellUnitCollection(source);
            _cells[source.PredictedPositionX, (int) source.CurrentLine].AddToCellUnitCollection(source);

            source.StartMoveUnit();

            CastMovementAbilities(source);
        }

        public void ForcedMoveUnit(UnitController source)
        {
            if (source == null) return;

            source.StopMoveUnit();
            source.StartForcedMoveUnit();
        }

        private bool CheckDistance(UnitController source)
        {
            return (source.PlayerType == PlayerType.Player &&
                    source.Target.CurrentCellX - source.CurrentCellX <= source.UnitConfig.AttackRange) ||
                   (source.PlayerType == PlayerType.Enemy
                    && source.CurrentCellX - source.Target.CurrentCellX <= source.UnitConfig.AttackRange);
        }

        private void Attack()
        {
            var attackers = new Collection<UnitController>();
            foreach (var unit in UnitCollection.Instance.GlobalUnitCollection)
            {
                if (!unit.IsStunned && unit.CanAttack && unit.Target != null && !unit.IsAttack &&
                    unit.Target.Health > 0 && unit.AttackSpeedMultiplier > 0 && !unit.IsMoving)
                {
                    //if (CheckDistance(unit))
                    unit.StopMoveUnit();
                    attackers.Add(unit);
                }
            }

            foreach (var attacker in attackers)
            {
                StartCoroutine(DoDamage(attacker));
            }
        }

        private IEnumerator DoDamage(UnitController source)
        {
            source.IsAttack = true;
            if (source.Target == null || source == null || source.Target.Health <= 0 || source.Health <= 0)
            {
                source.IsAttack = false;
                yield break;
            }

            _cavalryAttack = false;
            if (source.UnitConfig.Abilities.Any(x => x.GetType() == typeof(CavalryAttackAbilityConfig)) &&
                source.Speed >= 15)
            {
                source.PlayCavalryAttackAnimation();
                _cavalryAttack = true;
                yield return new WaitForSeconds(source.CavalryAttackTime);
            }
            else
            {
                source.PlayAttackPrecastAnimation();
                yield return new WaitForSeconds(source.PrecastTime);

                if (source.Target == null || source == null || source.Target.Health <= 0 || source.Health <= 0)
                {
                    source.IsAttack = false;
                    yield break;
                }

                source.PlayAttackAnimation();
                yield return new WaitForSeconds(source.AttackTime);
            }

            if (source.Target == null || source == null || source.Target.Health <= 0
                || (source.Health <= 0 && source.UnitConfig.AttackType != AttackType.Range))
            {
                source.IsAttack = false;
                yield break;
            }

            CastDamageAbilities(source);

            var damage = CalculateDamage(source);

            foreach (var effect in source.Target.GetEffects(typeof(SpikeEffectConfig)).ToArray())
            {
                if (effect.Value <= 0) continue;
                source.Health -= (int) (damage * effect.Value / 100);
                if (source.Health <= 0) yield break;
            }

            if (source.UnitConfig.AttackFx != null)
                source.UnitConfig.AttackFx.Create(source.Target.transform, source.UnitConfig.AttackFxPosition,
                    Vector3.one, Vector3.zero);

            source.Target.TakeDamage(damage);

            source.PlayReloadAnimation();
            yield return new WaitForSeconds(source.ReloadTime);

            source.IsAttack = false;
            if (_cavalryAttack)
            {
                source.Speed = source.MaxSpeed;
                source.Damage = source.MaxDamage;
            }
        }

        private IEnumerator DoTowerDamage(UnitController source)
        {
            source.IsAttack = true;

            var controller = source.PlayerType == PlayerType.Player
                ? GameController.Instance.Enemy
                : GameController.Instance.Player;

            if (source.UnitConfig.Abilities.Any(x => x.GetType() == typeof(CavalryAttackAbilityConfig)) &&
                source.Speed >= 15)
            {
                source.PlayCavalryAttackAnimation();
                yield return new WaitForSeconds(source.CavalryAttackTime);
                
                CastCavalryDamageAbility(source);
                _cavalryAttack = true;
            }
            else
            {
                source.PlayAttackPrecastAnimation(true);
                yield return new WaitForSeconds(source.PrecastTime);
                
                source.PlayAttackAnimation(true);
                yield return new WaitForSeconds(source.AttackTime);
            }
            
            source.PlayTowerDamagedAnimation();
            
            var damage = -CalculateDamage(source, true);
            controller.ChangeHitPoints(damage);

            source.PlayReloadAnimation();
            yield return new WaitForSeconds(source.ReloadTime);

            source.IsAttack = false;
            if (_cavalryAttack)
            {
                source.Speed = source.MaxSpeed;
                source.Damage = source.MaxDamage;
            } 
        }

        private int CalculateDamage(UnitController source, bool tower = false)
        {
            var damage = source.Damage;
            
            if (_playerDamageBoost)
                damage *= _damageBoostMultiplier;

            if (!tower)
            {
                var damageModifiers = source.UnitConfig.Abilities
                    .Where(x => x.GetType() == typeof(DamageToTagAbilityConfig))
                    .Cast<DamageToTagAbilityConfig>();
                var damageModifierSum = damageModifiers
                    .Where(x => x.Targets.Intersect(source.Target.UnitConfig.UnitTags).Any())
                    .Sum(x => x.DamageModifier);
                if (damageModifierSum != 0f)
                    damage = (int) (damage * (1 + damageModifierSum));
            }
            
            return damage;
        }

        public void AddNewUnit(UnitController source)
        {
            _cells[source.CurrentCellX, (int) source.CurrentLine].AddToCellUnitCollection(source);
        }

        public void RemoveDeadUnit(UnitController source)
        {
            _cells[source.CurrentCellX, (int) source.CurrentLine].RemoveFromCellUnitCollection(source);

            if (PossiblePlayerArmyLost && source.PlayerType == PlayerType.Player
                || PossibleEnemyArmyLost && source.PlayerType == PlayerType.Enemy)
                CheckArmyLoss(source.PlayerType == PlayerType.Player);
        }

        private void CheckArmyLoss(bool isPlayer)
        {
            bool armyLost = true;
            for (int i = 0; i < 3; i++)
            {
                var units = UnitCollection.Instance.GetUnits((LineType) i,
                    isPlayer ? PlayerType.Player : PlayerType.Enemy);
                if (units.Count > 0)
                {
                    armyLost = false;
                    break;
                }
            }

            if (armyLost)
            {
                if (isPlayer)
                    GameStop();
                else
                    _playerDamageBoost = true;

                OnArmyLost?.Invoke(isPlayer);
            }
        }

        private void CastDamageAbilities(UnitController unit)
        {
            foreach (var skill in unit.UnitConfig.Abilities)
            {
                if (skill.Type != AbilityType.Damage
                || skill.GetType() == typeof(CavalryAttackAbilityConfig) && !_cavalryAttack)
                    continue;

                skill.Cast(unit);
                if (skill.CustomAnimation) unit.PlayAttackAbilityAnimation();
            }
        }

        private void CastCavalryDamageAbility(UnitController unit)
        {
            foreach (var skill in unit.UnitConfig.Abilities)
            {
                if (skill.Type == AbilityType.Damage && skill.GetType() == typeof(CavalryAttackAbilityConfig))
                {
                    skill.Cast(unit);
                    if (skill.CustomAnimation) unit.PlayAttackAbilityAnimation();
                }
            }
        }

        private void CastMovementAbilities(UnitController unit)
        {
            foreach (var skill in unit.UnitConfig.Abilities)
            {
                if (skill.Type != AbilityType.Movement)
                    continue;

                skill.Cast(unit);
            }
        }

        private void CastBuffAbilities(UnitController unit)
        {
            foreach (var skill in unit.UnitConfig.Abilities)
            {
                if (skill.Type != AbilityType.Buff)
                    continue;
                
                skill.Cast(unit);
            }
        }

        private void CastAllPassiveTowerAbilities()
        {
            var controller = GameController.Instance.Player;
            for (var i = 0; i < controller.Towers.Count; i++)
            {
                CastTowerAbility(0, controller.Towers[i].Faction.PassiveAbilityRange, i, controller, false);
            }

            controller = GameController.Instance.Enemy;
            for (var i = 0; i < controller.Towers.Count; i++)
            {
                CastTowerAbility(_horizontalCellCount - 1 - controller.Towers[i].Faction.PassiveAbilityRange,
                    _horizontalCellCount - 1, i, controller, false);
            }
        }

        public void CastTowerAbility(int minX, int maxX, int currentY, PlayerController controller,
            bool isActiveAbility)
        {
            var tower = controller.Towers[currentY];
            var ability = isActiveAbility
                ? tower.Faction.ActiveAbilityConfig
                : tower.Faction.PassiveAbilityConfig;
            SoundManager.Instance.Play(ability.TowerAbilitySound);
            if (ability.IntervalCount > 0)
            {
                var seq = DOTween.Sequence();
                seq.AppendCallback(() =>
                    CastAbility(minX, maxX, currentY, controller, isActiveAbility, ability, tower));
                seq.AppendInterval(ability.IntervalDuration);
                seq.SetLoops(ability.IntervalCount, LoopType.Restart);
            }
            else
            {
                CastAbility(minX, maxX, currentY, controller, isActiveAbility, ability, tower);
            }

            if (isActiveAbility == false && tower.Faction.PassiveAbilityCooldown > 0)
                tower.StartPassiveAbilityCooldown();
        }

        private void CastAbility(int minX, int maxX, int currentY, PlayerController controller, bool isActiveAbility,
            AbilityConfig ability, Tower tower)
        {
            if (!isActiveAbility && tower.PassiveAbilityCooldown < tower.Faction.PassiveAbilityCooldown) return;
            
            var affectedUnits = new Collection<UnitController>();
            for (var j = minX; j < maxX; j++)
            {
                if (!_cells[j, currentY].ContainsUnit()) continue;
                foreach (var unit in _cells[j, currentY].CellUnitCollection)
                {
                    switch (ability.TargetType)
                    {
                        case AbilityTargetType.All:
                            if (unit.Health > 0) affectedUnits.Add(unit);
                            break;
                        case AbilityTargetType.Enemy:
                        case AbilityTargetType.EnemyCloser:
                        {
                            if (unit.PlayerType != controller.PlayerType && unit.Health > 0) affectedUnits.Add(unit);
                            break;
                        }
                        case AbilityTargetType.Self:
                        {
                            if (unit.PlayerType == controller.PlayerType && unit.Health > 0) affectedUnits.Add(unit);
                            break;
                        }
                    }
                }
            }

            if (isActiveAbility)
            {
                var zoneTransform =
                    GameController.Instance.GameMachine.GetLineZone(controller.PlayerType, tower.LineType);
                var rotateFx = controller.PlayerType == PlayerType.Enemy;
                Debug.Log($"Cast {controller.PlayerType}'s {ability.Name} with rotateFx = {rotateFx}");
                ability.StartFx(zoneTransform, rotateFx);

                var price = tower.Faction.ActiveAbilityManaCost;
                controller.ChangeMana(-(price > 0 ? price : 0));
            }

            switch (ability.TargetType)
            {
                case AbilityTargetType.NoTarget:
                    ability.CastNoTarget(controller, (LineType) currentY);
                    break;
                case AbilityTargetType.EnemyCloser:
                {
                    if (affectedUnits.Count > 0)
                    {
                        ability.Cast(GetCloserUnit(affectedUnits, tower));
                        if (isActiveAbility == false) 
                            tower.StartPassiveAbilityCooldown();
                    }
                    break;
                }
                case AbilityTargetType.Enemy:
                case AbilityTargetType.Self:
                case AbilityTargetType.All:
                default:
                    foreach (var unit in affectedUnits)
                        ability.Cast(unit);
                    break;
            }
            
            controller.TowerAbilityIsCasting = false;
        }

        private UnitController GetCloserUnit(Collection<UnitController> unitControllers, Tower tower)
        {
            var nearest = unitControllers.OrderBy(x => Vector3.Distance(tower.transform.position, x.transform.position))
                .FirstOrDefault();

            return nearest;
        }
    }
}