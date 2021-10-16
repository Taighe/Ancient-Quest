using Assets.Scripts.Events;
using Assets.Scripts.Globals;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : Projectile
{
    public override void OnHit(Collider hit, out int layer)
    {
        base.OnHit(hit, out layer);
        if (layer == (int)Layers.Player)
        {
            GameEvents.Instance.OnDamaged(new DamagedEventArgs(gameObject, hit.gameObject, Strength));
            Death();
            TriggerReset();
            return;
        }
    }
}
