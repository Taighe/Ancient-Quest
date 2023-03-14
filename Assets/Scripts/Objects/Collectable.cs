using AQEngine.Events;
using AQEngine.Globals;
using AQEngine.Level;
using AQEngine.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AQEngine.Objects
{
    [SelectionBase]
    public class Collectable : Object3D
    {
        public bool IsPersistant
        {
            get
            {
                return PersistantId >= 0;
            }
        }

        [Header("Game Properties")]
        public int Score;
        public int HP;
        public int Lives;
        public PowerUps PowerUpType = PowerUps.None;
        public int PersistantId;

        public override void Awake()
        {
            base.Awake();
        }

        public override void Start()
        {
            base.CollisionBounds = _collider.bounds.size;
            if (IsPersistant)
            {
                bool collected = LevelProperties.GetInstance().HasBeenCollected(this);
                gameObject.SetActive(!collected);
            }
        }

        public override void GameUpdate()
        {
            if (NotActiveWhenFarFromCamera())
                return;

            Collider hit;
            if (DetectCollisonCast(gameObject.GetInstanceID(), (int)Layers.Player, out hit))
            {
                GameEvents.Instance.Player_OnCollect(new CollectEventArgs(this));
                OnCollected();
            }
        }

        public virtual void OnCollected()
        {
            if (IsPersistant)
                LevelProperties.GetInstance().UpdatePersistantCollectable(PersistantId, true);

            gameObject.SetActive(false);
        }

#if UNITY_EDITOR
        public override void OnDrawGizmos()
        {
            base.OnDrawGizmos();
        }
#endif
    }
}