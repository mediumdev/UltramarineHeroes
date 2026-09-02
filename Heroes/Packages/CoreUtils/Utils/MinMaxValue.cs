using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Packages.CoreUtils.Utils
{
    [Serializable]
    public class SquishFloat
    {
        [SerializeField] private float _min;
        [SerializeField] private float _max;

        public float Value => Random.Range(_min, _max);

        public static float operator +(float c1, SquishFloat c2)
        {
            return c1 + c2.Value;
        }
    }

    [Serializable]
    public class SquishInt
    {
        [SerializeField] private int _min;
        [SerializeField] private int _max;

        public int Value => Random.Range(_min, _max + 1);

        public static float operator +(float c1, SquishInt c2)
        {
            return c1 + c2.Value;
        }
    }
}