using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Enums;
using Game.Controllers;
using Game.Units;
using Packages.CoreUtils.Utils;
using UnityEngine;
using Utils;
using Random = UnityEngine.Random;

namespace Game.Ai
{
    public class NormalBot : SimpleBot
    {
        private List<UnitController> _botUnits;
        private List<UnitController> _playerUnits;
        private LineType[] _lines;

        private bool _randomized = false;
        private int[] _botAiUnitRequiredCount = new int[3];
        private int[] _botAiUnitRequiredCost = new int[3];
        private bool _isDefenceTactics;
        private PlayerType _flagOwner;
        private float _manaPercentageBoost;

        private int TowerLevelTotal => PlayerFactionsController.Instance.GetFactionProgressSum();
        private int TowerLevelSelected => PlayerFactionsController.Instance.GetFactionProgressSum(SavedDataManager.GetPlayerDeckUids());

        [Header("Bot cheats")]
        [SerializeField, Tooltip("Процентный бонус ко времени накопления единицы маны"), Range(-1f, 2f)]
        private float _manaPercentageBoostBase = 0.8f;
        [SerializeField, Tooltip("Процентный бонус ко времени восстановления способности"), Range(0f, 10f)]
        private float _abilitiesTimerBoost;
        [SerializeField, Tooltip ("Каждый N-ый спавн бесплатный, при 0 не использует чит"), Range(0, 20)]
        private int _freeSpawn;
        private int _spawnsCounter;

        [Header("Прогрессия сложности бота")] 
        [SerializeField, Tooltip("Множитель суммарного уровня башен")] 
        private float _kLevelTotal = 0.1f;
        [SerializeField, Tooltip("Множитель уровня выбранных башен")] 
        private float _kLevelSelected = 0.9f;
        [SerializeField, Tooltip("Общий понижающий коэффициент")] 
        private float _kGeneral = 7f;
        
        public bool FreeSummon { get; private set; }
        public float ManaPercentageBoost => _manaPercentageBoost;

        private void Awake()
        {
            _manaPercentageBoost = ((TowerLevelTotal * _kLevelTotal + TowerLevelSelected * _kLevelSelected) / _kGeneral) + _manaPercentageBoostBase;
            Debug.Log($"Bot Mana Percentage Boost Calculated = {_manaPercentageBoost}\n" +
                      $"Bot Mana Percentage Boost Base = {_manaPercentageBoostBase}\n" +
                      $"Level total = {TowerLevelTotal}\n" +
                      $"Level selected = {TowerLevelSelected}");
        }
        private void RandomizeParameters()
        {
            // Рассчет параметры сложности бота
            
            
            // Random parameters of towers for each line
            _lines = new[] {LineType.Air, LineType.Ground, LineType.Underground};
           
            foreach (var line in _lines)
            {
                var tower = GameController.Instance.Enemy.GetTower(line);
                var ability = tower.Faction.ActiveAbilityConfig;
                
                _botAiUnitRequiredCount[(int)line] = Random.Range(ability.BotAiUnitLowerBoundCounter, ability.BotAiUnitUpperBoundCounter + 1);
                _botAiUnitRequiredCost[(int)line] = Random.Range(ability.BotAiUnitLowerBoundOfMinimumCost, ability.BotAiUnitUpperBoundOfMinimumCost + 1);
                
                if (_abilitiesTimerBoost != 0)
                    tower.SetTimerBoost(_abilitiesTimerBoost);
            }

            _isDefenceTactics = Random.Range(1f, 1f) >= .5f;
            Debug.Log($"Bot is on defense tactics: {_isDefenceTactics}");
            
            _randomized = true;
            _spawnsCounter = 1;
        }

        protected override IEnumerator BotRoutine()
        {
            if (!_randomized) RandomizeParameters();
            yield return new WaitForSeconds(Random.Range(_maxSpawnTime, _minSpawnTime));

            if (!GameController.Instance.GameMachine.IsPaused)
            {
                var linesSortedByPower = new List<LineType>();
                var lineMenace = 0;
                
                foreach (var line in _lines)
                {
                    var playerUnits = UnitCollection.Instance.GetUnits(line, PlayerType.Player);
                    var botUnits = UnitCollection.Instance.GetUnits(line, PlayerType.Enemy);
                    _botUnits = botUnits;
                    _playerUnits = playerUnits;
                    
                    CheckAbility(line);
                    
                    var botUnitsInHand = GetLineUnits(line);
                    if (botUnitsInHand.Count < 1) continue;
                    
                    _flagOwner = GameController.Instance.FlagsController.GetFlagOwnerOnLine(line);
                    
                    if (linesSortedByPower.Count == 0)
                    {
                        linesSortedByPower.Add(line);
                        lineMenace = ComparePower();
                    }
                    else
                    {
                        if (lineMenace < ComparePower())
                        {
                            linesSortedByPower.Insert(0, line);
                            lineMenace = ComparePower();
                        }
                        else
                        {
                            linesSortedByPower.Add(line);
                        }
                    }
                }
                
                if (linesSortedByPower.Count > 0)
                {
                    LineType selectedLine;
                    if (lineMenace <= 0)
                    {
                        linesSortedByPower.Shuffle();
                        selectedLine = linesSortedByPower[0];
                    }
                    else
                    {
                        selectedLine = _isDefenceTactics
                            ? linesSortedByPower[0]
                            : linesSortedByPower[linesSortedByPower.Count - 1];
                    }
                    
                    if (_freeSpawn != 0)
                    {
                        FreeSummon = _spawnsCounter == _freeSpawn;
                        _spawnsCounter = _spawnsCounter == _freeSpawn ? 1 : _spawnsCounter + 1;
                    }
                    SpawnRandom(GetLineUnits(selectedLine), selectedLine);
                }
                
                if (GameController.Instance.GameMachine.IsBattleActive)
                    _botRoutine = StartCoroutine(BotRoutine());
            }
            else
                _botRoutine = StartCoroutine(BotRoutine());
        }

        private int ComparePower()
        {
            var playerPower = 0;
            var botPower = 0;
            foreach (var playerUnit in _playerUnits)
            {
                playerPower += playerUnit.UnitConfig.Cost;
            }
            
            foreach (var botUnit in _botUnits)
            {
                botPower += botUnit.UnitConfig.Cost;
            }

            if (_flagOwner == PlayerType.Enemy)
            {
                playerPower += 0;
            }
            else if (_flagOwner == PlayerType.None)
            {
                playerPower += 1;
            }
            else if (_flagOwner == PlayerType.Player)
            {
                playerPower += 3;
            }

            return playerPower - botPower;
        }

        private void CheckAbility(LineType line)
        {
            var tower = GameController.Instance.Enemy.GetTower(line);
            if (tower.ActiveAbilityCooldown < tower.Faction.ActiveAbilityCooldown
            || GameController.Instance.Enemy.Mana < tower.Faction.ActiveAbilityManaCost) return;

            var cellX = GameController.Instance.GameMachine.HorizontalCellCount - 2;
            
            var playerTargets = new List<UnitController>();
            if (_playerUnits.Count > 0)
                playerTargets = GameController.Instance.GameMachine.SearchUnitsFromTower(PlayerType.Player, tower.Faction.ActiveAbilityRange, cellX, (int) line);
            var playerTargetsCount = playerTargets.Count;

            var botTargets = new List<UnitController>();
            if (_botUnits.Count > 0)
                botTargets = GameController.Instance.GameMachine.SearchUnitsFromTower(PlayerType.Enemy, tower.Faction.ActiveAbilityRange, cellX, (int) line);
            var botTargetsCount = botTargets.Count;
            

            switch (tower.Faction.ActiveAbilityConfig.BotAiType)
            {
                case BotAiType.EnemyAll:
                    if (playerTargetsCount < _botAiUnitRequiredCount[(int)line]) return;
                    break;
                case BotAiType.EnemyTarget:
                    var nearest = playerTargets.OrderBy(x => Vector3.Distance(tower.transform.position, x.transform.position)).FirstOrDefault();
                    if (nearest == null) return;
                    if (nearest.UnitConfig.Cost < _botAiUnitRequiredCost[(int)line]) return;
                    break;
                case BotAiType.SelfAll:
                    if (botTargetsCount < _botAiUnitRequiredCost[(int)line]) return;
                    var checker = false;
                    foreach (var botUnit in botTargets)
                    {
                        if (botUnit.Health < botUnit.MaxHealth) checker = true;
                    }
                    if (!checker) return;
                    break;
            }
           
            tower.Cast(PlayerType.Enemy, GameController.Instance.Enemy);
            // CastAbility(tower.Faction.ActiveAbilityRange, line);
            // tower.StartActiveAbilityCooldown();
        }
        

        private void CastAbility(int range, LineType line)
        {
            GameController.Instance.GameMachine.CastTowerAbility(
                GameController.Instance.GameMachine.HorizontalCellCount - 1 - range, 
                GameController.Instance.GameMachine.HorizontalCellCount - 1,  
                (int) line,
                GameController.Instance.Enemy,
                true);
        }
    }
}