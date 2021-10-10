using Assets.Scripts.Events;
using Assets.Scripts.Globals;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class Collectable : Object3D
{
    [Header("Game Properties")]
    public int Score;
    public int HP;
    public int Lives;
    public PowerUps PowerUpType = PowerUps.None;
    public override void Start()
    {
        base.CollisionBounds = _collider.bounds.size;
    }

    public override void GameUpdate()
    {
        if (NotActiveWhenFarFromCamera())
            return;

        Collider hit;
        if (DetectCollisonCast(gameObject.GetInstanceID(), (int)Layers.Player, out hit))
        {
            GameEvents.Instance.Player_OnCollect(new CollectEventArgs(this));
            gameObject.SetActive(false);
        }
    }

    public virtual void OnCollected() 
    {

    }

#if UNITY_EDITOR
    public override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
    }
#endif
}