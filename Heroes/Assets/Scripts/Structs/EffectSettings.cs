using System;
using System.Collections.Generic;
using UnityEngine;

namespace Structs
{
    [Serializable]
    public class EffectSettings
    {
        [SerializeField] protected List<ValueWithType> _health;
        [SerializeField] protected List<ValueWithType> _speed;
        [SerializeField] protected List<ValueWithType> _damage;
    }
}