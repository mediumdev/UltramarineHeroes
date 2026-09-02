using Configs;

namespace Utils
{
    public static class StringUtils
    {
        public static string DeckFactionsToString(DeckConfig deckConfig)
        {
            var factionString = string.Empty;
            foreach (var config in deckConfig.Units)
                factionString += $"{config.faction.Uid};";

            return factionString;
        }
        
        public static string DeckCollectionToString(DeckConfig deckConfig)
        {
            var collectionString = string.Empty;

            foreach (var config in deckConfig.Units)
            foreach (var unit in config.units)
                collectionString += $"{unit.Uid};";

            return collectionString;
        }
    }
}