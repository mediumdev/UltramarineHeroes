using Enums;
using Game.Units;

namespace Game.Summons
{
    public interface ISummon
    {
        PlayerType PlayerType { get; }
        void Impact(UnitController unit);
    }
}
