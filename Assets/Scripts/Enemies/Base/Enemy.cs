using Assets.Scripts.Events;
using Assets.Scripts.Globals;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class Enemy : Object3D
{
    [Header("Game Properties")]
    [Range(0, 9999)]
    public int HP = 1;
    [Range(0, 9999)]
    public int Strength = 1;
    public new Vector3 CollisionBounds = new Vector3(1, 1, 1);

    public override void Awake()
    {
        base.Awake();
        base.CollisionBounds = CollisionBounds;

        GameEvents.Instance.Damaged += Instance_Damaged;
    }

    public override void GameUpdate()
    {
        // Collision logic
        Collider hit = DetectCollision();
        if (hit != null)
        {
            var dir = (hit.ClosestPointOnBounds(transform.position) - transform.position).normalized;
            if (dir.y >= 0.8f && dir.y <= 1.0f)
            {
                if(HP > 0 && !IsInvulnerable)
                {
                    GameEvents.Instance.OnHit(new DamagedEventArgs(hit.gameObject, gameObject, 0));
                    Damaged(1);
                }
            }
            else
            {
                GameEvents.Instance.OnDamaged(new DamagedEventArgs(gameObject, hit.gameObject, Strength));
            }
        }

        base.GameUpdate();
    }

    protected override void Flash(bool visible)
    {
        _animator.Animator.gameObject.SetActive(visible);
    }

    protected override void OnDamaged(int damage)
    {
        HP -= 1;
        if(HP <= 0)
            gameObject.SetActive(false);
    }

    private Collider DetectCollision()
    {
        var colliders = Physics.OverlapBox(transform.position, new Vector3(CollisionBounds.x * 0.5f, CollisionBounds.y * 0.6f, CollisionBounds.z), transform.rotation, (int)Layers.Player);
        foreach(var c in colliders)
        {
            if(c.CompareTag("Player"))
            {
                return c;
            }
        }

        return null;
    }

#if UNITY_EDITOR
    public override void OnDrawGizmos()
    {
        Handles.color = Color.blue;
        Handles.DrawWireCube(transform.position, CollisionBounds);
    }
#endif
}
