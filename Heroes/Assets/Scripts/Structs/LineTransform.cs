using System;
using Enums;
using UnityEngine;

namespace Structs
{
    [Serializable]
    public struct LineTransform
    {
        public PlayerType player;
        public LineType line;
        public Transform transform;
    }
}