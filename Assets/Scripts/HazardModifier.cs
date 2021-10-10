using Assets.Scripts.Events;
using Assets.Scripts.Globals;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class HazardModifier : MonoBehaviour
{
    [Range(0, 100)]
    public int Damage = 1;
    public LayerMask CollisionMask;

    private Collider _collider;

    public void Awake()
    {
        _collider = GetComponent<Collider>();
        GameEvents.Instance.Hit += Instance_Hit;
        gameObject.layer = (int)LayersIndex.Hazard;
    }

    private void Instance_Hit(object sender, DamagedEventArgs e)
    {
        if (LayerHelper.LayerMask(e.Defender.layer) == CollisionMask.value)
        {
            GameEvents.Instance.OnDamaged(new DamagedEventArgs(gameObject, e.Defender, Damage));
        }
    }
}
