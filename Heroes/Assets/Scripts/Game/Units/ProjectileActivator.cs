using System;
using UnityEngine;

namespace Game.Units
{
    public class ProjectileActivator : MonoBehaviour
    {
        public Action ProjectileSpawnMoment;

        public void SpawnProjectile()
        {
            ProjectileSpawnMoment?.Invoke();
        }
    }
}