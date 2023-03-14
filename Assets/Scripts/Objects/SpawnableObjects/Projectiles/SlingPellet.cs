using AQEngine.Events;
using AQEngine.Globals;
using UnityEngine;

namespace AQEngine.Objects.SpawnableObjects
{
    public class SlingPellet : Projectile
    {
        public override void OnHit(Collider hit, out int layer)
        {
            base.OnHit(hit, out layer);
            if (layer == (int)Layers.Kinematic || layer == (int)Layers.Object)
            {
                if (hit.CompareTag("Enemy Bullet"))
                    return;

                GameEvents.Instance.OnDamaged(new DamagedEventArgs(gameObject, hit.gameObject, Strength));
                Death();
                DeSpawn();
            }
        }
    }
}