using System.Collections.Generic;
using System.Linq;
using CoreUtils.Utils;
using Enums;
using Game.Units;

namespace Game.Controllers
{
    public class UnitCollection : MonoSingleton<UnitCollection>
    {
        public List<UnitController> GlobalUnitCollection = new List<UnitController>();

        public List<UnitController> GetUnits(LineType line, PlayerType type)
        {
            return GlobalUnitCollection.Where(x => x.CurrentLine == line && x.PlayerType == type).ToList();
        }
    }
}