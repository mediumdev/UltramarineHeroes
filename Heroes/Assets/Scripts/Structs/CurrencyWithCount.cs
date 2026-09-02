using System;
using Configs;

namespace Structs
{
    [Serializable]
    public struct CurrencyWithCount
    {
        public CurrencyConfig currency;
        public int count;
    }
}