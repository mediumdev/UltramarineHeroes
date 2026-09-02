using System.Collections.Generic;
using CoreConfigs.Configs;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Configs
{
    public class FactionDeckConfig : ConfigBase
    {
        [SerializeField] private bool _useShuffle = true;
        [SerializeField] private FactionConfig[] _airFactions;
        [SerializeField] private FactionConfig[] _groundFactions;
        [SerializeField] private FactionConfig[] _undergroundFactions;
        private FactionConfig _airFaction;
        private FactionConfig _groundFaction;
        private FactionConfig _undergroundFaction;
        public List<FactionConfig> FactionsList { get; private set; }

        public void RandomizeFactions()
        {
            if (_useShuffle)
            {
                _airFaction = _airFactions[Random.Range(0, _airFactions.Length)];
                _groundFaction = _groundFactions[Random.Range(0, _groundFactions.Length)];
                _undergroundFaction = _undergroundFactions[Random.Range(0, _undergroundFactions.Length)];
            }
            else
            {
                _airFaction = _airFactions[0];
                _groundFaction = _groundFactions[0];
                _undergroundFaction = _undergroundFactions[0];
            }

            FactionsList = new List<FactionConfig> {_airFaction, _groundFaction, _undergroundFaction};
        }

#if UNITY_EDITOR
        [UnityEditor.MenuItem("Assets/Create/Configs/FactionDeckConfig")]
        private static void Create()
        {
            CreateAsset<FactionDeckConfig>();
        }
#endif
    }
}
