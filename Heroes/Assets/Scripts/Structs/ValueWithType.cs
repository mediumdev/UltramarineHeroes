using System;
using Enums;

namespace Structs
{
    [Serializable]
    public struct ValueWithType
    {
        public PlayerType type;
        public float value;
        public bool isPercentValue;
    }
}