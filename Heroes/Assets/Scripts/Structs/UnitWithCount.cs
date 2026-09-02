using System;
using Configs;

namespace Structs
{
    [Serializable]
    public struct UnitWithCount
    {
        public UnitConfig unit;
        public int count;
    }
}