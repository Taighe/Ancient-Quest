using Assets.Scripts.Events;
using Assets.Scripts.Globals;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Game Properties")]
    [Range(0, 9999)]
    public int HP = 1;
    [Range(0, 9999)]
    public int Strength = 1;
    public Vector3 CollisionBounds = new Vector3(1,1,1);

    private void Update()
    {
        // Collision logic
        Collider hit = DetectCollision();
        if (hit != null)
        {
            var dir = (hit.ClosestPointOnBounds(transform.position) - transform.position).normalized;
            if (dir.y >= 0.8f && dir.y <= 1.0f)
            {
                gameObject.SetActive(false);
                GameEvents.Instance.OnHit(new DamagedEventArgs(hit.gameObject, gameObject, 0));
            }
            else
            {
                GameEvents.Instance.OnDamaged(new DamagedEventArgs(gameObject, hit.gameObject, Strength));
            }
        }
    }

    private Collider DetectCollision()
    {
        var colliders = Physics.OverlapBox(transform.position, new Vector3(CollisionBounds.x * 0.5f, CollisionBounds.y * 0.6f, CollisionBounds.z), transform.rotation, 1 << (int)Layers.Kinematic);
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
    public void OnDrawGizmos()
    {
        Handles.color = Color.green;
        Handles.DrawWireCube(transform.position, CollisionBounds);
    }
#endif
}
