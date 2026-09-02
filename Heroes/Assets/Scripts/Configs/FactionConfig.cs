using System;
using System.Collections.Generic;
using CoreConfigs.Configs;
using Enums;
using Structs;
using UnityEngine;
using UnityEngine.Serialization;

namespace Configs
{
    [Serializable]
    public struct UnitWithLimits
    {
        public UnitConfig config;
        public int limit;
        public int generationAmount;
        public int generationSeconds;
    }
    
    [Serializable]
    public struct FactionProgressStruct
    {
        public CurrencyWithCount[] cost;
        public List<UnitWithLimits> unitsData;
        public int towerHealth;
    }
    
    public class FactionConfig : ConfigBase
    {
        [SerializeField] private UnitConfig[] _factionUnits;
        [SerializeField] private FactionProgressStruct[] _factionProgress;
        [SerializeField] private GameObject _towerObject;
        [SerializeField] private GameObject _townObject;
        [SerializeField] private LineType _factionType;
        [SerializeField] private Sprite _factionIcon;
        [SerializeField] private Sprite _factionBackground;
        [SerializeField] private string _factionName;
        [Header("Active Ability")]
        [SerializeField] private AbilityConfig _activeAbilityConfig;
        [SerializeField] private int _activeAbilityRange = 1;
        [SerializeField] private int _activeAbilityCooldown;
        [SerializeField] private int _activeAbilityManaCost;
        [Header("Passive Ability")]
        [SerializeField] private AbilityConfig _passiveAbilityConfig;
        [SerializeField] private int _passiveAbilityRange = 1;
        [SerializeField] private int _passiveAbilityCooldown;

        public UnitConfig[] FactionUnits => _factionUnits;
        public GameObject TowerObject => _towerObject;
        public GameObject TownObject => _townObject;
        public LineType FactionType => _factionType;
        public Sprite FactionIcon => _factionIcon;
        public Sprite FactionBackground => _factionBackground;
        public string FactionName => _factionName;
        public AbilityConfig ActiveAbilityConfig => _activeAbilityConfig;
        public int ActiveAbilityRange => _activeAbilityRange;
        public int ActiveAbilityCooldown => _activeAbilityCooldown;
        public int ActiveAbilityManaCost => _activeAbilityManaCost;
        public AbilityConfig PassiveAbilityConfig => _passiveAbilityConfig;
        public int PassiveAbilityRange => _passiveAbilityRange;
        public int PassiveAbilityCooldown => _passiveAbilityCooldown;
        public FactionProgressStruct[] FactionProgress => _factionProgress;

#if UNITY_EDITOR
        [UnityEditor.MenuItem("Assets/Create/Configs/FactionConfig")]
        private static void Create()
        {
            CreateAsset<FactionConfig>();
        }
        
        [UnityEditor.MenuItem("Tools/Configs/Set tower HP")]
        private static void SetTowerHealth()
        {
            foreach (var config in ConfigLibrary.EditorInstance.LoadAll<FactionConfig>())
            {
                for (var i = 0; i < config.FactionProgress.Length; i++)
                {
                    config.FactionProgress[i].towerHealth = 1200 + i * 200;
                }
            }
        }
#endif
    }
}
